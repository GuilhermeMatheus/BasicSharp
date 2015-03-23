using BasicSharp.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    [DebuggerDisplay("{ToString()}")]
    public abstract class SyntaxNode
    {
        List<TokenInfo> trivias = new List<TokenInfo>();

        public ReadOnlyCollection<TokenInfo> Trivias
        {
            get { return new ReadOnlyCollection<TokenInfo>(trivias); }
        }

        public ReadOnlyCollection<TokenInfo> Tokens
        {
            get { return new ReadOnlyCollection<TokenInfo>(Trivias.Concat(GetInternalTokens()).OrderBy(x => x.Begin).ToList()); }
        }

        public void AddTrivia(TokenInfo trivia)
        {
            trivias.Add(trivia);
        }

        public abstract IEnumerable<TokenInfo> GetInternalTokens();

        public override string ToString()
        {
            var result = string.Empty;

            foreach (var item in Tokens)
                result += item.StringValue;

            return result;
        }
    }
}
