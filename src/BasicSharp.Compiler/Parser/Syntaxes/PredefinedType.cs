using BasicSharp.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicSharp.Compiler.Parser;
using System.Collections;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class PredefinedType : SyntaxNode
    {
        public TokenInfo TypeToken { get; internal set; }
        public ArrayRankSpecifier ArraySpecifier { get; internal set; }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            yield return TypeToken;

            if (ArraySpecifier != null)
                foreach (var item in ArraySpecifier.Tokens)
                    yield return item;
        }

        public override IEnumerable GetChilds()
        {
            yield return TypeToken;
            yield return ArraySpecifier;
        }
    }
}
