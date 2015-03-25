using BasicSharp.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class Block : SyntaxNode
    {
        public TokenInfo OpenBrace { get; internal set; }
        public TokenInfo CloseBrace { get; internal set; }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            yield return OpenBrace;
            
            
            yield return CloseBrace;
        }
    }
}
