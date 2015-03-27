using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class AssignmentExpression : Expression
    {

        public override IEnumerable<Lexer.TokenInfo> GetInternalTokens()
        {
            yield break;
        }

    }
}
