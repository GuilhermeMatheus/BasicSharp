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
        public VariableDeclaration Declaration { get; internal set; }
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
