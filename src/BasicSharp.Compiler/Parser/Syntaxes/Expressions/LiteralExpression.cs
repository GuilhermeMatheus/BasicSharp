using BasicSharp.Compiler.Lexer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class LiteralExpression : Expression
    {
        public TokenInfo Value { get; internal set; }

        public override IEnumerable<Lexer.TokenInfo> GetInternalTokens()
        {
            yield return Value;
        }

        public override IEnumerable GetChilds()
        {
            yield return Value;
        }
    }
}
