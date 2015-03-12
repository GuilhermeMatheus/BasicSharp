using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Utils
{
    public static class CharExtensions
    {
        public static bool IsDigit(this char c)
        {
            return Char.IsDigit(c);
        }

        public static bool IsBinary(this char c)
        {
            return c == '0' || c == '1';
        }

        public static bool IsLineBreak(this char c)
        {
            return c == '\n' || c == '\r';
        }

        public static bool IsCharacter(this char c)
        {
            var asciiValue = (int)c;
            return (c >= 65 && c <= 90) || (c >= 97 && c <= 122);
        }
    }
}
