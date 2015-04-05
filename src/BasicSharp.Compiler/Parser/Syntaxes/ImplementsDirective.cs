using BasicSharp.Compiler.Lexer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class ImplementsDirective : SyntaxNode
    {
        List<TokenInfo> fullClassNameTokens = new List<TokenInfo>();

        public TokenInfo ImplementsToken { get; internal set; }
        public ReadOnlyCollection<TokenInfo> FullClassNameTokens
        {
            get { return fullClassNameTokens.AsReadOnly(); }
        }
        public TokenInfo SemicolonToken { get; internal set; }

        public void AddFullClassNamePart(TokenInfo part)
        {
            fullClassNameTokens.Add(part);
        }

        public override IEnumerable<Lexer.TokenInfo> GetInternalTokens()
        {
            yield return ImplementsToken;
            
            foreach (var item in fullClassNameTokens)
                yield return item;
            
            yield return SemicolonToken;
        }

        public override IEnumerable GetChilds()
        {
            yield return ImplementsToken;

            foreach (var item in FullClassNameTokens)
                yield return item;

            yield return SemicolonToken;
        }
    }
}
