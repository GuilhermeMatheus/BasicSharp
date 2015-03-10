using System;
using BasicSharp.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.Tests.SlidingText
{
    using Text = BasicSharp.Compiler.Lexer.SlidingText;

    internal static class SlidingTextSources
    {
        /// <summary>
        /// Create and returns a new instance of BasicSharp.Compiler.Lexer.SlidingText
        /// with a MemoryStream that represents the string "0123456789 for"
        /// </summary>
        /// <returns>returns a new instance of BasicSharp.Compiler.Lexer.SlidingText
        /// with a MemoryStream that represents the string "0123456789 for"</returns>
        public static Text GetSlidingTextWithFor()
        {
            var source = "0123456789 for";
            var sourceStream = source.GetMemoryStream();
            var text = new Text(sourceStream);
            return text;
        }

        /// <summary>
        /// Create and returns a new instance of BasicSharp.Compiler.Lexer.SlidingText
        /// with a MemoryStream that contains the source
        /// </summary>
        /// <returns>returns a new instance of BasicSharp.Compiler.Lexer.SlidingText
        /// with a MemoryStream that contains the source</returns>
        public static Text GetSlidingTextWith(string source)
        {
            var sourceStream = source.GetMemoryStream();
            var text = new Text(sourceStream);
            return text;
        }


    }
}
