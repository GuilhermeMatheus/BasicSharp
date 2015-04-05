using BasicSharp.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicSharp.Compiler.Parser;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class ArrayRankSpecifier : SyntaxNode
    {
        public TokenInfo OpenBracketToken { get; internal set; }
        public Expression Value { get; internal set; }
        public TokenInfo CloseBracketToken { get; set; }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            yield return OpenBracketToken;
            
            if (Value != null)
                foreach (var item in Value.Tokens)
                    yield return item;

            yield return CloseBracketToken;
        }

        public override System.Collections.IEnumerable GetChilds()
        {
            yield return OpenBracketToken;
            yield return Value;
            yield return CloseBracketToken;
        }
    }
}
