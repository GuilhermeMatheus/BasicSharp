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

    }
}
