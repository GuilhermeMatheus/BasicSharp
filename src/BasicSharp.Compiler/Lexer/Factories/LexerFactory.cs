using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicSharp.Utils;

namespace BasicSharp.Compiler.Lexer
{
    public static class LexerFactory
    {
        public static Lexer FromString(string source)
        {
            var sourceStream = source.GetMemoryStream();
            var text = new SlidingText(sourceStream);
            return new Lexer(text);
        }
    }
}
