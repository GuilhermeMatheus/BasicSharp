using BasicSharp.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicSharp.Compiler.Parser.Extensions;
using System.Collections;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public abstract class BinaryExpression : Expression
    {
        Expression _leftSide;
        public Expression LeftSide
        {
            get { return _leftSide; }
            internal set
            {
                if (_leftSide != value)
                {
                    _leftSide = value;
                    Accept(value);
                }
            }
        }
        public TokenInfo OperatorToken { get; set; }
        Expression _rightSide;
        public Expression RightSide
        {
            get { return _rightSide; }
            internal set
            {
                if (_rightSide != value)
                {
                    _rightSide = value;
                    Accept(value);
                }
            }
        }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            foreach (var item in LeftSide.GetTokenEnumerable())
                yield return item;

            yield return OperatorToken;

            foreach (var item in RightSide.GetTokenEnumerable())
                yield return item;
        }

        public override IEnumerable GetChilds()
        {
            yield return LeftSide;
            yield return OperatorToken;
            yield return RightSide;
        }
    }
}
