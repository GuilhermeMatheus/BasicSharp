using BasicSharp.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicSharp.Compiler.Parser.Extensions;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class ReturnStatement : Statement
    {
        public TokenInfo ReturnToken { get; internal set; }
        public Expression Expression { get; internal set; }
        public TokenInfo SemicolonToken { get; internal set; }


        public override System.Collections.IEnumerable GetChilds()
        {
            yield return ReturnToken;
            yield return Expression;
            yield return SemicolonToken;
        }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            yield return ReturnToken;

            foreach (var item in Expression.GetTokenEnumerable())
                yield return item;
            
            yield return SemicolonToken;
        }
    }
}
