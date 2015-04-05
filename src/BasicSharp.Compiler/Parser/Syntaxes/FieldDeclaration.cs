using BasicSharp.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicSharp.Compiler.Parser.Extensions;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class FieldDeclaration : ModuleMemberDeclaration
    {
        public override bool IsPublic
        {
            get { return Modifier.Kind == SyntaxKind.EverybodyKeyword; }
        }

        public TokenInfo Modifier { get; internal set; }

        public VariableDeclaration Declaration { get; internal set; }

        public TokenInfo SemicolonToken { get; internal set; }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            yield return Modifier;

            foreach (var item in Declaration.GetTokenEnumerable())
                yield return item;
            
            yield return SemicolonToken;
        }

        public override System.Collections.IEnumerable GetChilds()
        {
            yield return Modifier;

            yield return Declaration;

            yield return SemicolonToken;
        }
    }
}
