using BasicSharp.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public abstract class SyntaxNode
    {
        List<TokenInfo> trivias = new List<TokenInfo>();

        public ReadOnlyCollection<TokenInfo> Trivias
        {
            get { return new ReadOnlyCollection<TokenInfo>(trivias); }
        }

        public void AddTrivia(TokenInfo trivia)
        {
            trivias.Add(trivia);
        }
    }
}
