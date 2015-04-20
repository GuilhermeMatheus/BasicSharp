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
        ArrayRankSpecifier _arraySpecifier;
        public ArrayRankSpecifier ArraySpecifier
        {
            get { return _arraySpecifier; }
            internal set
            {
                if (_arraySpecifier != value)
                {
                    _arraySpecifier = value;
                    Accept(value);
                }
            }
        }

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
