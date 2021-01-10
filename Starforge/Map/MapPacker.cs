using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Starforge.Map {
    /// <summary>
    /// Used to parse binary maps and encode maps into the binary format.
    /// </summary>
    public static class MapPacker {
        // Used for RLE
        private static StringBuilder Builder = new StringBuilder();
        private static byte[] RLEBytes = new byte[32768];

        // You may be wondering what the point of using a Dictionary rather than a List here is.
        // After all, we are just storing the strings and their indices, right?
        // Somehow, using a Dictionary is 5x faster. So we will use a Dictionary (:
        private static string[] ReadLookup;
        private static Dictionary<string, short> WriteLookup;
        private static short WriteLookupCounter = 0;

        private static BinaryWriter Writer;
        private static BinaryReader Reader;

        /// <summary>
        /// Decodes a run-length-encoded string from the map file.
        /// </summary>
        /// <param name="rle">The RLE string to decode.</param>
        /// <returns>The decoded string.</returns>
        public static string DecodeRLE(byte[] rle) {
            Builder.Clear();
            for (int i = 0; i < rle.Length; i += 2) Builder.Append((char)rle[i + 1], rle[i]);

            return Builder.ToString();
        }

        /// <summary>
        /// Encodes a string to a run-length-encoded format for the binary format.
        /// </summary>
        /// <param name="input">The input string to encode.</param>
        /// <returns>The RLE-encoded input string.</returns>
        public static unsafe byte[] EncodeRLE(string rle) {
            int pos = 0;
            byte[] arr = RLEBytes;

            fixed (byte* ptr = &arr[0]) {
                for (int i = 0; i < rle.Length; i++) {
                    byte cv = (byte)rle[i];
                    byte c = 1;

                    while (i + 1 < rle.Length && c < byte.MaxValue && rle[i + 1] == cv) {
                        c++;
                        i++;
                    }

                    ptr[pos++] = c;
                    ptr[pos++] = cv;
                }

                byte[] res = new byte[pos];
                fixed (byte* resptr = &res[0]) {
                    for (int i = 0; i < pos; i++) resptr[i] = ptr[i];
                }

                return res;
            }
        }

        /// <summary>
        /// Reads a map from a map binary file.
        /// </summary>
        /// <param name="reader">The BinaryReader reading from the map binary file.</param>
        /// <returns>The parsed map.</returns>
        public static MapElement ReadMapBinary(BinaryReader reader) {
            if (reader.ReadString() != "CELESTE MAP") throw new InvalidDataException("Map does not start with CELESTE MAP header");
            string package = reader.ReadString();

            ReadLookup = new string[reader.ReadInt16()];
            for (int i = 0; i < ReadLookup.Length; i++) ReadLookup[i] = reader.ReadString();

            Reader = reader;

            MapElement el = ReadMapElement();
            el.Package = package;
            return el;
        }

        /// <summary>
        /// Reads a map element from the binary map file.
        /// </summary>
        /// <returns>The parsed MapElement.</returns>
        public static MapElement ReadMapElement() {
            MapElement el = new MapElement();
            el.Name = ReadLookup[Reader.ReadInt16()];
            byte attrCount = Reader.ReadByte();

            for (int i = 0; i < attrCount; i++) {
                string attrName = ReadLookup[Reader.ReadInt16()];
                object attrVal = null;

                ValueType type = (ValueType)Reader.ReadByte();

                switch (type) {
                case ValueType.Boolean:
                    attrVal = Reader.ReadBoolean();
                    break;
                case ValueType.Byte:
                    attrVal = (int)Reader.ReadByte();
                    break;
                case ValueType.Short:
                    attrVal = (int)Reader.ReadInt16();
                    break;
                case ValueType.Integer:
                    attrVal = Reader.ReadInt32();
                    break;
                case ValueType.Float:
                    attrVal = Reader.ReadSingle();
                    break;
                case ValueType.Lookup:
                    attrVal = ReadLookup[Reader.ReadInt16()];
                    break;
                case ValueType.String:
                    attrVal = Reader.ReadString();
                    break;
                case ValueType.RLE:
                    attrVal = DecodeRLE(Reader.ReadBytes(Reader.ReadInt16()));
                    break;
                }

                el.Attributes.Add(attrName, attrVal);
            }

            short childCount = Reader.ReadInt16();
            for (int i = 0; i < childCount; i++) el.Children.Add(ReadMapElement());

            return el;
        }

        /// <summary>
        /// Writes a map to a binary file.
        /// </summary>
        /// <param name="writer">The BinaryWriter used to write the result to.</param>
        /// <param name="el">The map to write.</param>
        public static void WriteMapBinary(BinaryWriter writer, MapElement el) {
            // Create string lookup table
            WriteLookup = new Dictionary<string, short>();
            WriteLookupCounter = 0;
            CreateLookupTable(el);

            Writer = writer;
            writer.Write("CELESTE MAP");
            writer.Write(el.Package);
            writer.Write((short)WriteLookup.Count);
            foreach (string str in WriteLookup.Keys) writer.Write(str);

            WriteMapElement(el);
            writer.Flush();
        }

        /// <summary>
        /// Writes a map element to a binary file.
        /// </summary>
        /// <param name="el">The MapElement to write.</param>
        public static void WriteMapElement(MapElement el) {
            byte attrCount = (byte)el.Attributes.Count;
            short childCount = (short)el.Children.Count;

            WriteLookupValue(el.Name, false);
            Writer.Write(attrCount);

            foreach (KeyValuePair<string, object> pair in el.Attributes) {
                WriteLookupValue(pair.Key, false);

                if (pair.Key == "innerText") {
                    if (el.Name == "solids" || el.Name == "bg") {
                        byte[] res = EncodeRLE(pair.Value.ToString());

                        Writer.Write((byte)ValueType.RLE);
                        Writer.Write((short)res.Length);
                        Writer.Write(res);
                    } else {
                        Writer.Write((byte)ValueType.String);
                        Writer.Write(pair.Value.ToString());
                    }
                } else {
                    if (pair.Value is bool) {
                        Writer.Write((byte)ValueType.Boolean);
                        Writer.Write((bool)pair.Value);
                    } else if (pair.Value is int) {
                        CompressNumber((int)pair.Value);
                    } else if (pair.Value is float) {
                        float val = (float)pair.Value;

                        // Compress to a byte/short/int if whole
                        if (val % 1 == 0) {
                            CompressNumber((int)val);
                        } else {
                            Writer.Write((byte)ValueType.Float);
                            Writer.Write((float)pair.Value);
                        }
                    } else {
                        WriteLookupValue(pair.Value.ToString(), true);
                    }
                }
            }

            Writer.Write(childCount);
            foreach (MapElement child in el.Children) WriteMapElement(child);
        }

        public static void CreateLookupTable(MapElement el) {
            if (!WriteLookup.ContainsKey(el.Name)) WriteLookup.Add(el.Name, WriteLookupCounter++);

            foreach (KeyValuePair<string, object> pair in el.Attributes) {
                // Add attribute keys
                if (!WriteLookup.ContainsKey(pair.Key) && pair.Key != null) WriteLookup.Add(pair.Key, WriteLookupCounter++);

                // Add attribute values
                if (pair.Value is string && pair.Key != "innerText" && !WriteLookup.ContainsKey(pair.Value.ToString()) && pair.Key != null) WriteLookup.Add(pair.Value.ToString(), WriteLookupCounter++);
            }

            foreach (MapElement child in el.Children) CreateLookupTable(child);
        }

        public static void CompressNumber(int num) {
            if (num >= 0 && num <= byte.MaxValue) {
                Writer.Write((byte)ValueType.Byte);
                Writer.Write((byte)num);
            } else if (num >= short.MinValue && num <= short.MaxValue) {
                Writer.Write((byte)ValueType.Short);
                Writer.Write((short)num);
            } else {
                Writer.Write((byte)ValueType.Integer);
                Writer.Write(num);
            }
        }

        public static void WriteLookupValue(string value, bool writeType) {
            if (writeType) Writer.Write((byte)ValueType.Lookup);
            short lval = WriteLookup[value];

            if (lval < 0) throw new KeyNotFoundException($"Could not find lookup value {value}");
            Writer.Write(lval);
        }
    }

    /// <summary>
    /// Represents an encoded value type in the binary format.
    /// </summary>
    public enum ValueType : byte {
        Boolean = 0,
        Byte = 1,
        Short = 2,
        Integer = 3,
        Float = 4,
        Lookup = 5,
        String = 6,
        RLE = 7
    }
}
