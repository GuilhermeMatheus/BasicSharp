using BasicSharp.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class FieldDeclaration : SyntaxNode
    {
        public TokenInfo Modifier { get; internal set; }

        public VariableDeclarator Declarator { get; internal set; }

        public TokenInfo SemiColon { get; internal set; }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            yield return Modifier;

            if (Declarator != null)
                foreach (var item in Declarator.Tokens)
                    yield return item;
            
            yield return SemiColon;
        }
    }
}
