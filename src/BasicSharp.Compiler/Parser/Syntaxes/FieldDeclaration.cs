using BasicSharp.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public TokenInfo Semicolon { get; internal set; }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            yield return Modifier;

            if (Declaration != null)
                foreach (var item in Declaration.Tokens)
                    yield return item;
            
            yield return Semicolon;
        }
    }
}
