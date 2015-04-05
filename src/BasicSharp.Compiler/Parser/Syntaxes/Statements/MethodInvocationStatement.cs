using BasicSharp.Compiler.Lexer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicSharp.Compiler.Parser.Extensions;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class MethodInvocationStatement : Statement
    {
        public MethodInvocationExpression MethodInvocation { get; internal set; }
        public TokenInfo SemicolonToken { get; internal set; }

        public override IEnumerable GetChilds()
        {
            yield return MethodInvocation;
            yield return SemicolonToken;
        }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            foreach (var item in MethodInvocation.GetTokenEnumerable())
                yield return item;

            yield return SemicolonToken;
        }
    }
}
