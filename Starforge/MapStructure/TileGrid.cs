using System.Text.RegularExpressions;

namespace Starforge.MapStructure {
    public class TileGrid {
        public int this[int x, int y] {
            get => Tiles[x, y];
            set => Tiles[x, y] = value;
        }

        public int[,] Tiles {
            get;
            private set;
        }

        public int Width {
            get;
            private set;
        }

        public int Height {
            get;
            private set;
        }

        public TileGrid(int width, int height) {
            Tiles = new int[Width = width, Height = height];
            FillEmpty();
        }

        public TileGrid(int[,] matrix, int width, int height) {
            Tiles = (int[,])matrix.Clone();
            Width = width;
            Height = height;
        }

        public TileGrid(string matrix, int width, int height) {
            Tiles = new int[Width = width, Height = height];
            FillEmpty();

            Regex splitReg = new Regex("\\r\\n|\\n\\r|\\n|\\r");
            string[] rows = splitReg.Split(matrix);

            for(int i = 0; i < rows.Length; i++) {
                for(int j = 0; j < rows[i].Length; j++) {
                    Tiles[j, i] = rows[i][j];
                }
            }
        }

        public void FillEmpty() {
            for(int i = 0; i < Width; i++) {
                for(int j = 0; j < Height; j++) {
                    Tiles[i, j] = 48;
                }
            }
        }

        public override string ToString() {
            string[] resultArray = new string[Height];

            for(int i = 0; i < Height; i++) {
                for(int j = 0; j < Width; j++) {
                    resultArray[i] += (char)Tiles[j, i];
                }
            }

            return string.Join("\n", resultArray);
        }

        public string ToCSV() {
            string[] resultArray = new string[Height];

            for(int i = 0; i < Height; i++) {
                for(int j = 0; j < Width; j++) {
                    resultArray[i] += Tiles[j, i].ToString();
                    if(j < Width - 1)
                        resultArray[i] += ",";
                }

                while(resultArray[i].EndsWith(",-1")) {
                    resultArray[i] = resultArray[i].Substring(0, resultArray[i].Length - 3);
                }
            }

            return string.Join("\n", resultArray);
        }
    }
}
