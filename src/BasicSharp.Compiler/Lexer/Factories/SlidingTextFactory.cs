using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicSharp.Utils;

namespace BasicSharp.Compiler.Lexer
{
    public static class SlidingTextFactory
    {
        /// <summary>
        /// Create and returns a new instance of BasicSharp.Compiler.Lexer.SlidingText
        /// with a MemoryStream that contains the source
        /// </summary>
        /// <returns>returns a new instance of BasicSharp.Compiler.Lexer.SlidingText
        /// with a MemoryStream that contains the source</returns>
        public static SlidingText FromString(string source)
        {
            var sourceStream = source.GetMemoryStream();
            var text = new SlidingText(sourceStream);
            return text;
        }
    }
}
