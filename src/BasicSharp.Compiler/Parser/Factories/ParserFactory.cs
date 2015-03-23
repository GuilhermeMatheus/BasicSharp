using BasicSharp.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.Parser
{
    public static class ParserFactory
    {
        public static Parser FromString(string source)
        {
            var lxr = LexerFactory.FromString(source);
            return new Parser(lxr);
        }
    }
}
