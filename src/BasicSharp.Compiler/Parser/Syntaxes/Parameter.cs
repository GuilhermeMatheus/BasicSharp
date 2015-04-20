using BasicSharp.Compiler.Lexer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicSharp.Compiler.Parser.Extensions;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class Parameter : SyntaxNode
    {
        PredefinedType _type;
        public PredefinedType Type
        {
            get { return _type; }
            internal set
            {
                if (_type != value)
                {
                    _type = value;
                    Accept(value);
                }
            }
        }
        public TokenInfo Identifier { get; internal set; }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            foreach (var item in Type.GetTokenEnumerable())
                yield return item;

            yield return Identifier;
        }

        public override IEnumerable GetChilds()
        {
            yield return Type;
            yield return Identifier;
        }
    }
}
