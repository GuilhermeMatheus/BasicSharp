using BasicSharp.Compiler.Lexer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class BreakStatement : Statement
    {
        public TokenInfo BreakToken { get; internal set; }
        public TokenInfo SemicolonToken { get; internal set; }

        public override IEnumerable GetChilds()
        {
            yield return BreakToken;
            yield return SemicolonToken;
        }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            yield return BreakToken;
            yield return SemicolonToken;
        }
    }
}
