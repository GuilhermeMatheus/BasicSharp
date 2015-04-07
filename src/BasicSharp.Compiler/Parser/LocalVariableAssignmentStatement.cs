using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicSharp.Compiler.Parser.Extensions;
using System.Collections;
using BasicSharp.Compiler.Lexer;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class LocalVariableAssignmentStatement : Statement
    {
        public VariableAssignmentExpression Declarator { get; internal set; }
        public TokenInfo SemicolonToken { get; internal set; }

        public override IEnumerable GetChilds()
        {
            yield return Declarator;
            yield return SemicolonToken;
        }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            foreach (var item in Declarator.GetTokenEnumerable())
                yield return item;

            yield return SemicolonToken;
        }
    }
}
