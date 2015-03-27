using BasicSharp.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class BinaryExpression : Expression
    {
        public Expression LeftSide { get; internal set; }
        public TokenInfo OperatorToken { get; set; }
        public Expression RightSide { get; internal set; }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            throw new NotImplementedException();
        }
    }
}
