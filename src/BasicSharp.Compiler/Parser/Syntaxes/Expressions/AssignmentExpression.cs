using BasicSharp.Compiler.Lexer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicSharp.Compiler.Parser.Extensions;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class AssignmentExpression : Expression
    {
        public TokenInfo OperatorToken { get; internal set; }
        public Expression Expression { get; internal set; }

        public override IEnumerable GetChilds()
        {
            yield return OperatorToken;
            yield return Expression;
        }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            yield return OperatorToken;

            foreach (var item in Expression.GetTokenEnumerable())
                yield return item;
        }
    }
}
