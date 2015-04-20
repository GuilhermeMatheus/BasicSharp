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
        Expression _expression;
        public Expression Expression
        {
            get { return _expression; }
            internal set
            {
                if (_expression != value)
                {
                    _expression = value;
                    Accept(value);
                }
            }
        }

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
