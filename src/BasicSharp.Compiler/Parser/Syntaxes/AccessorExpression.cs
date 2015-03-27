using BasicSharp.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicSharp.Compiler.Parser.Extensions;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class AccessorExpression : Expression
    {
        public TokenInfo Identifier { get; internal set; }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            yield return Identifier;
        }
    }
}
