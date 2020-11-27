using System;
using System.Collections.Generic;
using System.IO;

// Taken from Celeste.BinaryPacker

namespace Starforge.MapStructure.Encoding {
    public static class MapPacker {
        public static HashSet<string> IgnoreAttributes;
        public static Dictionary<string, short> StringValues;
        public static short StringCounter;
        public static string[] Lookup;

        static MapPacker() {
            IgnoreAttributes = new HashSet<string>{
                "_eid"
            };

            StringValues = new Dictionary<string, short>();
        }

        public static bool ParseValue(string value, out byte type, out object result) {
            bool outBool;
            byte outByte;
            short outShort;
            int outInt;
            float outFloat;

            if(bool.TryParse(value, out outBool)) {
                type = 0;
                result = outBool;
            } else if(byte.TryParse(value, out outByte)) {
                type = 1;
                result = outByte;
            } else if(short.TryParse(value, out outShort)) {
                type = 2;
                result = outShort;
            } else if(int.TryParse(value, out outInt)) {
                type = 3;
                result = outInt;
            } else if(float.TryParse(value, out outFloat)) {
                type = 4;
                result = outFloat;
            } else {
                type = 5;
                result = value;
            }

            return true;
        }

        public static BinaryMapElement ReadMapElement(BinaryReader reader) {
            BinaryMapElement element = new BinaryMapElement();
            Int16 d = reader.ReadInt16();
            element.Name = Lookup[d];

            byte b = reader.ReadByte();
            if(b > 0) {
                element.Attributes = new Dictionary<string, object>();
            }

            for(int i = 0; i < (int)b; i++) {
                Int16 e = reader.ReadInt16();

                string key = Lookup[e];
                byte type = reader.ReadByte();
                object value = null;

                switch(type) {
                    case (byte)ValueTypes.Bool:
                        value = reader.ReadBoolean();
                        break;
                    case (byte)ValueTypes.Byte:
                        value = Convert.ToInt32(reader.ReadByte());
                        break;
                    case (byte)ValueTypes.Short:
                        value = Convert.ToInt32(reader.ReadInt16());
                        break;
                    case (byte)ValueTypes.Int:
                        value = reader.ReadInt32();
                        break;
                    case (byte)ValueTypes.Float:
                        value = reader.ReadSingle();
                        break;
                    case (byte)ValueTypes.LookupString:
                        value = Lookup[reader.ReadInt16()];
                        break;
                    case (byte)ValueTypes.String:
                        value = reader.ReadString();
                        break;
                    case (byte)ValueTypes.RLEString:
                        short count = reader.ReadInt16();
                        value = RunLengthUtil.Decode(reader.ReadBytes(count));
                        break;
                }

                element.Attributes.Add(key, value);
            }

            short children = reader.ReadInt16();
            if(children > 0) {
                element.Children = new List<BinaryMapElement>();
            }
            for(int j = 0; j < children; j++) {
                element.Children.Add(ReadMapElement(reader));
            }

            return element;
        }

        public static BinaryMapElement ReadMapBinary(BinaryReader reader) {
            if(reader.ReadString() != "CELESTE MAP") {
                throw new InvalidDataException("Invalid header");
            }

            string package = reader.ReadString();
            short lookupCounter = reader.ReadInt16();

            Lookup = new string[lookupCounter];
            for(int i = 0; i < lookupCounter; i++) {
                Lookup[i] = reader.ReadString();
            }

            BinaryMapElement element = ReadMapElement(reader);
            element.Package = package;

            return element;
        }

        public static void AddLookupValue(string value) {
            if(value != null && !StringValues.ContainsKey(value)) {
                StringValues.Add(value, StringCounter);
                StringCounter++;
            }
        }

        public static void CreateLookupTable(BinaryMapElement element) {
            AddLookupValue(element.Name);
            foreach(KeyValuePair<string, object> pair in element.Attributes) {
                if(!IgnoreAttributes.Contains(pair.Key)) {
                    if(element.Name == "solids" || element.Name == "bg" && pair.Key == "innerText") {
                        AddLookupValue(pair.Key);
                        return;
                    }

                    AddLookupValue(pair.Key);
                    AddLookupValue(element.Name);
                    if(pair.Value is string) {
                        AddLookupValue(pair.Value.ToString());
                    }
                }
            }
            foreach(BinaryMapElement child in element.Children) {
                CreateLookupTable(child);
            }
        }

        public static void WriteMapElement(BinaryWriter writer, BinaryMapElement element) {
            int children = element.Children.Count;
            int attributes = 0;

            foreach(string key in element.Attributes.Keys) {
                if(!IgnoreAttributes.Contains(key)) {
                    attributes++;
                }
            }

            writer.Write(StringValues[element.Name]);
            writer.Write((byte)attributes);
            foreach(KeyValuePair<string, object> pair in element.Attributes) {
                if(!IgnoreAttributes.Contains(pair.Key)) {
                    if(pair.Key == "innerText") {
                        writer.Write(StringValues["innerText"]);
                        if(element.Name == "solids" || element.Name == "bg") {
                            byte[] array = RunLengthUtil.Encode(element.GetString("innerText"));
                            writer.Write((byte)ValueTypes.RLEString);
                            writer.Write((short)array.Length);
                            writer.Write(array);
                        } else {
                            writer.Write((byte)ValueTypes.String);
                            writer.Write(element.GetString("innerText"));
                        }
                    } else {
                        byte type;
                        object result;
                        ParseValue(pair.Value.ToString(), out type, out result);

                        writer.Write(StringValues[pair.Key]);
                        writer.Write(type);

                        ValueTypes value = (ValueTypes)type;

                        switch(value) {
                            case ValueTypes.Bool:
                                writer.Write((bool)result);
                                break;
                            case ValueTypes.Byte:
                                writer.Write((byte)result);
                                break;
                            case ValueTypes.Short:
                                writer.Write((short)result);
                                break;
                            case ValueTypes.Int:
                                writer.Write((int)result);
                                break;
                            case ValueTypes.Float:
                                writer.Write((float)result);
                                break;
                            case ValueTypes.LookupString:
                                writer.Write(StringValues[(string)result]);
                                break;
                        }
                    }
                }
            }

            writer.Write((short)children);
            foreach(BinaryMapElement child in element.Children) {
                WriteMapElement(writer, child);
            }
        }

        public static void WriteMapBinary(BinaryMapElement element, BinaryWriter writer, string package) {
            StringValues.Clear();
            StringCounter = 0;

            CreateLookupTable(element);
            AddLookupValue("innerText");

            writer.Write("CELESTE MAP");
            writer.Write(package);
            writer.Write((short)StringValues.Count);
            foreach(KeyValuePair<string, short> pair in StringValues) {
                writer.Write(pair.Key);
            }

            WriteMapElement(writer, element);
            writer.Flush();
        }
    }

    public enum ValueTypes : byte {
        Bool = 0,
        Byte = 1,
        Short = 2,
        Int = 3,
        Float = 4,
        LookupString = 5,
        String = 6,
        RLEString = 7
    }
}
