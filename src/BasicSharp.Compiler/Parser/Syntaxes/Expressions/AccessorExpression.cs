using BasicSharp.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicSharp.Compiler.Parser.Extensions;
using System.Collections;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class AccessorExpression : Expression
    {
        public TokenInfo Identifier { get; internal set; }
        public BracketedArgument BracketedArgument { get; internal set; }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            yield return Identifier;
            foreach (var item in BracketedArgument.GetTokenEnumerable())
                yield return item;
        }

        public override IEnumerable GetChilds()
        {
            yield return Identifier;
            yield return BracketedArgument;
        }
    }
}
