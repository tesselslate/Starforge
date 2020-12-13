using System.Collections.Generic;
using System.Text;

namespace Starforge.MapStructure.Encoding {
    public static class RunLengthUtil {
        public static byte[] Encode(string value) {
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

        public static string Decode(byte[] value) {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < value.Length; i += 2) {
                builder.Append((char)value[i + 1], (int)value[i]);
            }

            return builder.ToString();
        }
    }
}
