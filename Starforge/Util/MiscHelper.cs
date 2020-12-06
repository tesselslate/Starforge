using System;

namespace Starforge.Util {
    public static class MiscHelper {
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
