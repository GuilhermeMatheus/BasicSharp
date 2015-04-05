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
    public class BinaryExpression : Expression
    {
        public Expression LeftSide { get; internal set; }
        public TokenInfo OperatorToken { get; set; }
        public Expression RightSide { get; internal set; }

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
