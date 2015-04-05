using BasicSharp.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicSharp.Compiler.Parser.Extensions;
using System.Collections;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class ParenthesedExpression : Expression
    {
        public TokenInfo OpenParenToken { get; internal set; }
        public Expression InnerExpression { get; internal set; }
        public TokenInfo CloseParenToken { get; internal set; }

        public override IEnumerable<Lexer.TokenInfo> GetInternalTokens()
        {
            yield return OpenParenToken;

            foreach (var item in InnerExpression.GetTokenEnumerable())
                yield return item;

            yield return CloseParenToken;
        }

        public override IEnumerable GetChilds()
        {
            yield return OpenParenToken;
            yield return InnerExpression;
            yield return CloseParenToken;
        }
    }
}
