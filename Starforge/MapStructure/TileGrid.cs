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
            Empty();
        }

        public TileGrid(int[,] matrix, int width, int height) {
            Tiles = (int[,])matrix.Clone();
            Width = width;
            Height = height;
        }

        public TileGrid(string matrix, int width, int height) {
            Tiles = new int[Width = width, Height = height];
            Empty();

            Regex splitReg = new Regex("\\r\\n|\\n\\r|\\n|\\r");
            string[] rows = splitReg.Split(matrix);

            for (int i = 0; i < rows.Length; i++) {
                for (int j = 0; j < rows[i].Length; j++) {
                    Tiles[j, i] = rows[i][j];
                }
            }
        }

        public void Empty() {
            for (int i = 0; i < Width; i++) {
                for (int j = 0; j < Height; j++) {
                    Tiles[i, j] = '0';
                }
            }
        }

        public void SetTile(int x, int y, int t) {
            if (x < 0 || y < 0 || x > Width - 1 || y > Height - 1) return;
            this[x, y] = t;
        }

        /// <summary>
        /// Returns the tile at x,y. If the tile is outside of the bounds of the TileGrid, 0 is returned.
        /// </summary>
        public int GetTile(int x, int y) {
            if (x < 0 || y < 0 || x > Width - 1 || y > Height - 1) return 0;
            return this[x, y];
        }

        public override string ToString() {
            string[] resultArray = new string[Height];
            int[] filledTiles = new int[Height];

            for (int i = 0; i < Height; i++) {
                resultArray[i] = string.Empty;


                for (int j = Width - 1; j >= 0; j--) {
                    filledTiles[i] = j + 1;
                    if (Tiles[j, i] != 48) break;
                }

                if (filledTiles[i] == 1) continue;

                for (int j = 0; j < filledTiles[i]; j++) {
                    resultArray[i] += (char)Tiles[j, i];
                }
            }

            return string.Join("\n", resultArray);
        }

        public string ToCSV() {
            string[] resultArray = new string[Height];
            int[] filledTiles = new int[Height];

            for (int i = 0; i < Height; i++) {
                resultArray[i] = string.Empty;

                for (int j = Width - 1; j >= 0; j--) {
                    filledTiles[i] = j + 1;
                    if (Tiles[j, i] != -1) {
                        break;
                    }
                }

                if (filledTiles[i] == 1) continue;

                for (int j = 0; j < filledTiles[i]; j++) {
                    resultArray[i] += Tiles[j, i].ToString();
                    if (j < filledTiles[i] - 1)
                        resultArray[i] += ",";
                }
            }

            return string.Join("\n", resultArray);
        }
    }
}