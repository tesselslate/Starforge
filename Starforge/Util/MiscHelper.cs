using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Starforge.Util {
    public static class MiscHelper {
        public static Random Rand = new Random();

        public static T Choose<T>(List<T> toChoose) {
            return toChoose[Rand.Next(toChoose.Count)];
        }

        public static T Choose<T>(int x, int y, List<T> toChoose) {
            return toChoose[(x ^ y * x + y) % toChoose.Count];
        }

        public static byte HexCharToByte(char c) {
            return (byte)"0123456789ABCDEF".IndexOf(char.ToUpper(c));
        }

        public static Color HexToColor(string hex) {
            int index = 0;
            if(hex.Length >= 1 && hex[0] == '#') index = 1;

            if(hex.Length - index >= 6) {
                return new Color(
                    HexCharToByte(hex[index])     * 16 + HexCharToByte(hex[index + 1]),
                    HexCharToByte(hex[index + 2]) * 16 + HexCharToByte(hex[index + 3]),
                    HexCharToByte(hex[index + 4]) * 16 + HexCharToByte(hex[index + 5])
                );
            } else {
                int hexNum;
                if(int.TryParse(hex.Substring(index), out hexNum)) {
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

        public static int[,] ReadCSV(string csv, int width, int height) {
            int[,] array = new int[width, height];

            for(int i = 0; i < width; i++) {
                for(int j = 0; j < height; j++) {
                    array[i, j] = -1;
                }
            }

            string[] array2 = csv.Split(new char[] {
                '\n'
            });

            int index = 0;
            while(index < height && index < array2.Length) {
                string[] array3 = array2[index].Split(new char[]
                {
                    ','
                }, StringSplitOptions.RemoveEmptyEntries);

                int widthIndex = 0;
                while(widthIndex < width && widthIndex < array3.Length) {
                    array[widthIndex, index] = Convert.ToInt32(array3[widthIndex]);
                    widthIndex++;
                }
                index++;
            }

            return array;
        }
    }
}
