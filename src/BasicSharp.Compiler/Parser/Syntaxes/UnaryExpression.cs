using BasicSharp.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicSharp.Compiler.Parser.Extensions;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class UnaryExpression : Expression
    {
        public TokenInfo SignalToken { get; set; }
        public Expression Expression { get; internal set; }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            yield return SignalToken;
            foreach (var item in Expression.GetTokenEnumerable())
                yield return item;
        }
    }
}
