using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyseMemoire
{
    class Pattern
    {
        public byte[] pattern { get; set; }
        public int offset { get; set; }

        public Pattern(byte[] pattern, int offset)
        {
            this.pattern = pattern;
            this.offset = offset;
        }

        public Pattern(string pattern, int offset)
        {
            this.pattern = this.parsePattern(pattern);
            this.offset = offset;
        }

        public byte[] parsePattern(string pattern)
        {
            pattern = new string(pattern.ToCharArray().Where(c2 => !Char.IsWhiteSpace(c2)).ToArray());
            if (pattern.Length % 2 == 1)
                throw new Exception("The binary key cannot have an odd number of digits");

            byte[] arr = new byte[pattern.Length >> 1];

            for (int i = 0; i < pattern.Length >> 1; ++i)
            {
                if( GetHexVal(pattern[i << 1]) ==((byte)'?'))
                    arr[i] = 0x3F;
                else
                    arr[i] = (byte)((GetHexVal(pattern[i << 1]) << 4) + (GetHexVal(pattern[(i << 1) + 1])));
            }
            return arr;
        }

        public static int GetHexVal(char hex)
        {
            if ((byte)hex == '?')
                return (byte)'?';
            int val = (int)hex;
            return val - (val < 58 ? 48 : 55);
        }
    }
}
