using BasicSharp.Compiler.Lexer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicSharp.Compiler.Parser.Extensions;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class BracketedArgument : SyntaxNode
    {
        public TokenInfo OpenBracketToken { get; internal set; }
        public Expression ArgumentExpression { get; internal set; }
        public TokenInfo CloseBracketToken { get; internal set; }

        public override IEnumerable GetChilds()
        {
            yield return OpenBracketToken;
            yield return ArgumentExpression;
            yield return CloseBracketToken;
        }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            yield return OpenBracketToken;
            foreach (var item in ArgumentExpression.GetTokenEnumerable())
                yield return item;
            yield return CloseBracketToken;
        }
    }
}
