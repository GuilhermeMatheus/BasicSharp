using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Utils
{
    public static class StringExtensions
    {

        public static MemoryStream GetMemoryStream(this string source)
        {
            var inBytes = Encoding.Default.GetBytes(source);
            var memoryStream = new MemoryStream(inBytes);

            return memoryStream;
        }
    }
}
