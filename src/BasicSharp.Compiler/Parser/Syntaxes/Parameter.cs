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
        public TokenInfo Type { get; internal set; }
        public TokenInfo Identifier { get; internal set; }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            yield return Type;
            yield return Identifier;
        }
    }
}
