using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class Block : SyntaxNode
    {
        public override IEnumerable<Lexer.TokenInfo> GetInternalTokens()
        {
            yield break;
        }
    }
}
