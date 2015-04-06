using BasicSharp.Compiler.Lexer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicSharp.Compiler.Parser.Extensions;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class Parameter : SyntaxNode
    {
        public PredefinedType Type { get; internal set; }
        public TokenInfo Identifier { get; internal set; }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            foreach (var item in Type.GetTokenEnumerable())
                yield return item;

            yield return Identifier;
        }

        public override IEnumerable GetChilds()
        {
            yield return Type;
            yield return Identifier;
        }
    }
}
