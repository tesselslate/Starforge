﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Starforge.Map {
    /// <summary>
    /// Used to parse binary maps and encode maps into the binary format.
    /// </summary>
    public static class MapPacker {
        public static Dictionary<string, short> LookupKeys;
        public static short LookupCounter;
        public static string[] Lookup;

        static MapPacker() {
            LookupKeys = new Dictionary<string, short>();
            LookupCounter = 0;
        }

        /// <summary>
        /// Adds a string to the lookup table.
        /// </summary>
        /// <param name="value">The string to add to the lookup table.</param>
        public static void AddLookupValue(string value) {
            if (value != null && !LookupKeys.ContainsKey(value)) {
                LookupKeys.Add(value, LookupCounter++);
            }
        }

        /// <summary>
        /// Creates a string lookup table for all the values in the specified MapElement.
        /// </summary>
        /// <param name="el">The MapElement to add looukp values for.</param>
        public static void CreateLookupTable(MapElement el) {
            AddLookupValue(el.Name);
            foreach (KeyValuePair<string, object> pair in el.Attributes) {
                AddLookupValue(pair.Key);

                // If the key/value pair is for tile information - don't create lookup values for the inner text.
                if ((el.Name == "solids" || el.Name == "bg" || el.Name == "objtiles") && pair.Key == "innerText") {
                    break;
                }

                // Add value to lookup if it's a string.
                if (pair.Value is string) {
                    AddLookupValue(pair.Value.ToString());
                }
            }

            // Add children elements' values to the lookup.
            foreach (MapElement child in el.Children) {
                CreateLookupTable(child);
            }
        }

        /// <summary>
        /// Decodes a run-length encoded string from the binary map format.
        /// </summary>
        /// <param name="value">A byte array containing the run-length encoded string.</param>
        /// <returns>The decoded string.</returns>
        public static string DecodeRLE(byte[] value) {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < value.Length; i += 2) {
                builder.Append((char)value[i + 1], value[i]);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Encodes a string to the run-length format for map packing.
        /// </summary>
        /// <param name="value">The string to encode.</param>
        /// <returns>An array of bytes containing the length-encoded string.</returns>
        public static byte[] EncodeRLE(string value) {
            List<byte> list = new List<byte>();
            for (int i = 0; i < value.Length; i++) {
                byte b = 1;
                char c = value[i];
                while (i + 1 < value.Length && value[i + 1] == c && b < 255) {
                    b += 1;
                    i++;
                }

                list.Add(b);
                list.Add((byte)c);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Parses an attribute value.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <param name="type">Outputs the type of the value.</param>
        /// <param name="result">The parsed result.</param>
        public static void ParseValue(string value, out ValueType type, out object result) {
            // This method may seem like it is useless, and that it could just return the type.
            // However, doing it this way lets us "compress" certain types down:
            // e.g. int -> short if (value < short.MaxValue)
            // or   int -> byte  if (value < byte.MaxValue)
            // This can result in fairly sizable space savings, at the cost of some performance while encoding the map.

            if (bool.TryParse(value, out bool oBool)) {
                type = ValueType.Boolean;
                result = oBool;
            } else if (byte.TryParse(value, out byte oByte)) {
                type = ValueType.Byte;
                result = oByte;
            } else if (short.TryParse(value, out short oShort)) {
                type = ValueType.Short;
                result = oShort;
            } else if (int.TryParse(value, out int oInt)) {
                type = ValueType.Int;
                result = oInt;
            } else if (float.TryParse(value, out float oFloat)) {
                type = ValueType.Float;
                result = oFloat;
            } else {
                type = ValueType.Lookup;
                result = value;
            }
        }

        public static MapElement ReadMapBinary(BinaryReader reader) {
            if (reader.ReadString() != "CELESTE MAP") {
                throw new InvalidDataException("Invalid map header.");
            }

            // Read metadata - map package and amount of lookup keys
            string package = reader.ReadString();
            short keysCounter = reader.ReadInt16();

            Lookup = new string[keysCounter];
            for (int i = 0; i < keysCounter; i++) {
                Lookup[i] = reader.ReadString();
            }

            MapElement el = ReadMapElement(reader);
            el.Package = package;

            return el;
        }

        /// <summary>
        /// Parses a map element from the binary format.
        /// </summary>
        /// <param name="reader">The BinaryReader to read the map data from.</param>
        /// <returns>The parsed map element.</returns>
        public static MapElement ReadMapElement(BinaryReader reader) {
            MapElement el = new MapElement();
            el.Name = Lookup[reader.ReadInt16()];

            byte attrCount = reader.ReadByte();
            for (int i = 0; i < attrCount; i++) {
                short loc = reader.ReadInt16();
                string key = Lookup[loc];
                ValueType type = (ValueType)reader.ReadByte();
                object value = null;

                switch (type) {
                case ValueType.Boolean:
                    value = reader.ReadBoolean();
                    break;
                case ValueType.Byte:
                    value = Convert.ToInt32(reader.ReadByte());
                    break;
                case ValueType.Short:
                    value = Convert.ToInt32(reader.ReadInt16());
                    break;
                case ValueType.Int:
                    value = reader.ReadInt32();
                    break;
                case ValueType.Float:
                    value = reader.ReadSingle();
                    break;
                case ValueType.Lookup:
                    value = Lookup[reader.ReadInt16()];
                    break;
                case ValueType.String:
                    value = reader.ReadString();
                    break;
                case ValueType.RLE:
                    // RLE strings start with a short (int16) containing the length.
                    value = DecodeRLE(reader.ReadBytes(reader.ReadInt16()));
                    break;
                }

                el.Attributes.Add(key, value);
            }

            short childCount = reader.ReadInt16();
            for (int i = 0; i < childCount; i++) {
                el.Children.Add(ReadMapElement(reader));
            }

            return el;
        }

        /// <summary>
        /// Writes a map element to a binary file.
        /// </summary>
        /// <param name="writer">The BinaryWriter used to write the result to.</param>
        /// <param name="el">The MapElement to write.</param>
        public static void WriteMapBinary(BinaryWriter writer, MapElement el) {
            LookupKeys.Clear();
            LookupCounter = 0;

            CreateLookupTable(el);

            writer.Write("CELESTE MAP");
            writer.Write(el.Package);
            writer.Write((short)LookupKeys.Count);
            foreach (string val in LookupKeys.Keys) {
                writer.Write(val);
            }

            WriteMapElement(writer, el);
            writer.Flush();
        }

        /// <summary>
        /// Converts a map element to its binary representation.
        /// </summary>
        /// <param name="writer">The BinaryWriter used to write the result to.</param>
        /// <param name="el">The MapElement to convert.</param>
        public static void WriteMapElement(BinaryWriter writer, MapElement el) {
            int children = el.Children.Count;
            int attributes = el.Attributes.Keys.Count;

            if (el.Children.Count > short.MaxValue || el.Attributes.Count > byte.MaxValue) {
                throw new InvalidDataException($"Potential corruption in writing element {el.Name}: too many elements/attributes");
            }

            writer.Write(LookupKeys[el.Name]);
            writer.Write((byte)attributes);

            foreach (KeyValuePair<string, object> pair in el.Attributes) {
                writer.Write(LookupKeys[pair.Key]);
                if (pair.Key == "innerText") {
                    if (el.Name == "solids" || el.Name == "bg") {
                        byte[] array = EncodeRLE(el.GetString("innerText"));

                        // RLE strings are preceded by a short containing their length.
                        // If this exceeds the max value of a short, the map will corrupt.
                        // Writing a normal string instead will result in a huge increase in size (likely),
                        // but will prevent the map data from getting corrupted.
                        if (array.Length > short.MaxValue) {
                            writer.Write((byte)ValueType.String);
                            writer.Write(el.GetString("innerText"));
                        } else {
                            writer.Write((byte)ValueType.RLE);
                            writer.Write((short)array.Length);
                            writer.Write(array);
                        }
                    } else {
                        writer.Write((byte)ValueType.String);
                        writer.Write(el.GetString("innerText"));
                    }
                } else {
                    ValueType type;
                    object res;
                    ParseValue(pair.Value.ToString(), out type, out res);

                    writer.Write((byte)type);

                    switch (type) {
                    case ValueType.Boolean:
                        writer.Write((bool)res);
                        break;
                    case ValueType.Byte:
                        writer.Write((byte)res);
                        break;
                    case ValueType.Short:
                        writer.Write((short)res);
                        break;
                    case ValueType.Int:
                        writer.Write((int)res);
                        break;
                    case ValueType.Float:
                        writer.Write((float)res);
                        break;
                    case ValueType.Lookup:
                        writer.Write(LookupKeys[(string)res]);
                        break;
                    }
                }
            }

            writer.Write((short)children);
            foreach (MapElement child in el.Children) {
                WriteMapElement(writer, child);
            }
        }
    }

    /// <summary>
    /// Represents an encoded value type in the binary format.
    /// </summary>
    public enum ValueType : byte {
        Boolean = 0,
        Byte = 1,
        Short = 2,
        Int = 3,
        Float = 4,
        Lookup = 5,
        String = 6,
        RLE = 7
    }
}
