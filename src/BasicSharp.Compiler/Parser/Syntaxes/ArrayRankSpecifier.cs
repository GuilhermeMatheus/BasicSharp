using BasicSharp.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicSharp.Compiler.Parser;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class ArrayRankSpecifier : SyntaxNode
    {
        public TokenInfo OpenBracketToken { get; internal set; }
        Expression _value;
        public Expression Value
        {
            get { return _value; }
            internal set
            {
                if (_value != value)
                {
                    _value = value;
                    Accept(value);
                }
            }
        }
        public TokenInfo CloseBracketToken { get; set; }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            yield return OpenBracketToken;
            
            if (Value != null)
                foreach (var item in Value.Tokens)
                    yield return item;

            yield return CloseBracketToken;
        }

        public override System.Collections.IEnumerable GetChilds()
        {
            yield return OpenBracketToken;
            yield return Value;
            yield return CloseBracketToken;
        }
    }
}
