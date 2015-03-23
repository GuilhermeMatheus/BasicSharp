using BasicSharp.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class Parameter : SyntaxNode
    {
        public TokenInfo Type { get; private set; }
        public TokenInfo Identifier { get; private set; }
    }
}
