using BasicSharp.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicSharp.Compiler.Parser.Extensions;
using System.Collections;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class Argument : SyntaxNode
    {
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

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            foreach (var item in Expression.GetTokenEnumerable())
                yield return item;
        }

        public override IEnumerable GetChilds()
        {
            yield return Expression;
        }
    }
}
