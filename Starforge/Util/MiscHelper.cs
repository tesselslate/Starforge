using Microsoft.Xna.Framework;
using Starforge.Map;
using System;
using System.Collections.Generic;

namespace Starforge.Util {
    /// <summary>
    /// Contains various miscellaneous functions.
    /// </summary>
    public static class MiscHelper {
        public static Random Rand;
        public static byte[] RandBytes;

        static MiscHelper() {
            Rand = new Random();

            RandBytes = new byte[65536];
            Rand.NextBytes(RandBytes);
        }

        public static float Angle(Vector2 from, Vector2 to) {
            return (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
        }

        public static Vector2 AngleToVector(float angleRadians, float length) {
            return new Vector2((float)Math.Cos(angleRadians) * length, (float)Math.Sin(angleRadians) * length);
        }

        public static string CleanCamelCase(string str) {
            List<string> words = new List<string>();
            string word = "";

            foreach (char c in str) {
                if (char.IsUpper(c)) {
                    if (!string.IsNullOrEmpty(word)) words.Add(char.ToUpper(word[0]) + word.Substring(1));
                    word = $"{c}";
                } else {
                    word += c;
                }
            }

            words.Add(char.ToUpper(word[0]) + word.Substring(1));

            return string.Join(" ", words.ToArray());
        }

        public static Room CloneRoom(Room room) {
            return Room.Decode(room.Encode(), room.Parent);
        }

        public static string ColorToHex(Color c) {
            return c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        public static byte HexCharToByte(char c) {
            return (byte)"0123456789ABCDEF".IndexOf(char.ToUpper(c));
        }

        public static Color HexToColor(string hex) {
            int index = 0;
            if (hex.Length >= 1 && hex[0] == '#') index = 1;

            if (hex.Length - index >= 6) {
                return new Color(
                    HexCharToByte(hex[index]) * 16 + HexCharToByte(hex[index + 1]),
                    HexCharToByte(hex[index + 2]) * 16 + HexCharToByte(hex[index + 3]),
                    HexCharToByte(hex[index + 4]) * 16 + HexCharToByte(hex[index + 5])
                );
            } else {
                int hexNum;
                if (int.TryParse(hex.Substring(index), out hexNum)) {
                    return HexToColor(hexNum);
                } else {
                    return Color.White;
                }
            }
        }

        public static Color HexToColor(int hex) {
            Color res = default;
            res.A = byte.MaxValue;
            res.R = (byte)(hex >> 16);
            res.G = (byte)(hex >> 8);
            res.B = (byte)hex;
            return res;
        }

        public static int RandInt(int seed, int max) {
            return RandBytes[seed % ushort.MaxValue] % max;
        }

        public static System.Numerics.Vector3 ColorToVect3(Color c) {
            return new System.Numerics.Vector3((float)c.R / 255, (float)c.G / 255, (float)c.B / 255);
        }

        public static Color Vect3ToColor(System.Numerics.Vector3 v) {
            return new Color(v.X, v.Y, v.Z);
        }

        // Returns a Rectangle that is centered around x and y with width w and height h
        public static Rectangle RectangleCentered(int x, int y, int w, int h) {
            return new Rectangle(x - w / 2, y - h / 2, w, h);
        }

        public static Rectangle RectangleCentered(float x, float y, int w, int h) {
            return RectangleCentered((int)x, (int)y, w, h);
        }

        public static Rectangle RectangleCentered(Vector2 pos, int w, int h) {
            return RectangleCentered(pos.X, pos.Y, w, h);
        }

        // Copies a Dictionary, copying every element
        public static Dictionary<TKey, object> CloneDictionary<TKey>(Dictionary<TKey, object> original) {
            Dictionary<TKey, object> ret = new Dictionary<TKey, object>(original.Count, original.Comparer);
            foreach (KeyValuePair<TKey, object> entry in original) {
                ret.Add(entry.Key, entry.Value);
            }
            return ret;
        }
    }
}
