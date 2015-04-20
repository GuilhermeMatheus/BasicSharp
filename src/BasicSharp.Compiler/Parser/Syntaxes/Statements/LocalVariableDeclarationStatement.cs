using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicSharp.Compiler.Parser;
using System.Collections.ObjectModel;
using System.Collections;
using BasicSharp.Compiler.Lexer;
using BasicSharp.Compiler.Parser.Extensions;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class LocalVariableDeclarationStatement : Statement
    {
        VariableDeclaration _declaration;
        public VariableDeclaration Declaration
        {
            get { return _declaration; }
            internal set
            {
                if (_declaration != value)
                {
                    _declaration = value;
                    Accept(value);
                }
            }
        }
        public TokenInfo SemicolonToken { get; internal set; }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            foreach (var item in Declaration.GetTokenEnumerable())
                yield return item;

            yield return SemicolonToken;
        }
        public override IEnumerable GetChilds()
        {
            yield return Declaration;
            yield return SemicolonToken;
        }
    }
}
