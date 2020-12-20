using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starforge.Map {
    /// <summary>
    /// Contains a two-dimensional grid of 16-bit integers.
    /// </summary>
    public class TileGrid {
        /// <summary>
        /// The underlying array.
        /// </summary>
        public short[] Map;

        /// <summary>
        /// Safe access to the grid, with bounds checking.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>The value at the given location, or the DefaultValue if the location is out of bounds.</returns>
        public short this[int x, int y] {
            get {
                if (x < 0 || x > Width - 1 || y < 0 || y > Height - 1) return DefaultValue;
                else return Map[x + y * Width];
            }
            set {
                if (!(x < 0 || x > Width - 1 || y < 0 || y > Height - 1)) Map[x + y * Width] = value;
            }
        }

        /// <summary>
        /// The default value to return when attempting to retrieve an out of bounds location.
        /// </summary>
        public short DefaultValue;

        /// <summary>
        /// The width of the grid.
        /// </summary>
        public int Width;

        /// <summary>
        /// The height of the grid.
        /// </summary>
        public int Height;

        /// <summary>
        /// Creates a blank tile grid.
        /// </summary>
        /// <param name="width">The width of the grid.</param>
        /// <param name="height">The height of the grid.</param>
        public TileGrid(int width, int height) {
            Map = new short[(Width = width) * (Height = height)];
            DefaultValue = default;
        }

        /// <summary>
        /// Creates a tile grid from the given string.
        /// </summary>
        /// <param name="grid">A tile grid formatted as text.</param>
        /// <param name="width">The width of the grid.</param>
        /// <param name="height">The height of the grid.</param>
        /// <param name="defaultValue">The value to fill unused tiles with.</param>
        public TileGrid(string grid, int width, int height, short defaultValue = TILE_AIR) {
            Map = new short[(Width = width) * (Height = height)];
            Fill(DefaultValue = defaultValue);

            string[] rows = grid.Split('\n');
            int yInc;
            for (int y = 0; y < rows.Length; y++) {
                yInc = y * width;
                for (int x = 0; x < rows[y].Length; x++) {
                    Map[x + yInc] = (byte)rows[y][x];
                }
            }
        }

        /// <summary>
        /// Creates a tile grid from the given string, with the given element separator.
        /// </summary>
        /// <param name="grid">A tile grid formatted as a string with separator characters.</param>
        /// <param name="separator">The character which separates grid elements.</param>
        /// <param name="width">The width of the grid.</param>
        /// <param name="height">The height of the grid.</param>
        /// <param name="defaultValue">The value to fill unused tiles with.</param>
        public TileGrid(string grid, char separator, int width, int height, short defaultValue = OBJ_AIR) {
            Map = new short[(Width = width) * (Height = height)];
            Fill(DefaultValue = defaultValue);

            if (string.IsNullOrEmpty(grid)) return;

            string[] rows = grid.Split('\n');
            int yInc;
            for (int y = 0; y < rows.Length; y++) {
                yInc = y * width;
                string[] contents = rows[y].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int x = 0; x < contents.Length; x++) {
                    Map[x + yInc] = short.Parse(contents[x]);
                }
            }
        }

        /// <summary>
        /// Fills the grid with the given value.
        /// </summary>
        /// <param name="value">The value to fill the grid with.</param>
        public void Fill(short value) {
            for (int i = 0; i < Map.Length; i++) Map[i] = value;
        }

        /// <summary>
        /// Generates a CSV of the tile grid, where each element is separated by commas and each row is separated by newlines.
        /// </summary>
        /// <returns>The CSV of the tile grid.</returns>
        public string ToCSV() {
            string[] res = new string[Height];
            string line;
            int filled;
            int yInc;

            for (int y = 0; y < Height; y++) {
                filled = 0;
                yInc = y * Width;
                line = string.Empty;
                for (int x = Width - 1; x >= 0; x--) {
                    filled = x + 1;
                    if (Map[x + yInc] != DefaultValue) break;
                }

                if (filled == 1) continue;
                for(int x = 0; x < filled; x++) {
                    line += Map[x + yInc];
                    line += ',';
                }

                res[y] = line;
            }

            return string.Join("\n", res).TrimEnd('\n');
        }

        /// <summary>
        /// Converts the grid to a string.
        /// </summary>
        /// <returns>The tile grid as a string, where each row is separated by a newline.</returns>
        public override string ToString() {
            string[] res = new string[Height];
            string line;
            int filled;
            int yInc;

            for (int y = 0; y < Height; y++) {
                filled = 0;
                yInc = y * Width;
                line = string.Empty;

                for (int x = Width - 1; x >= 0; x--) {
                    filled = x + 1;
                    if (Map[x + yInc] != DefaultValue) break;
                }

                if (filled == 1) continue;
                for (int x = 0; x < filled; x++) {
                    line += (char)Map[x + yInc];
                }

                res[y] = line;
            }

            return string.Join("\n", res).TrimEnd('\n');
        }

        public const short TILE_AIR = 48;
        public const short OBJ_AIR  = -1;
    }
}
