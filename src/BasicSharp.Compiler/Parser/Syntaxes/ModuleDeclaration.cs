using BasicSharp.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicSharp.Compiler.Parser;
using System.Collections.ObjectModel;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class ModuleDeclaration : SyntaxNode
    {
        List<ModuleMemberDeclaration> members = new List<ModuleMemberDeclaration>();
        
        public TokenInfo ModuleToken { get; internal set; }
        public TokenInfo Name { get; internal set; }
        public TokenInfo OpenBrace { get; internal set; }
        public ReadOnlyCollection<ModuleMemberDeclaration> Members
        {
            get { return members.AsReadOnly(); }
        }
        public TokenInfo CloseBrace { get; internal set; }

        public void AddMember(ModuleMemberDeclaration member)
        {
            members.Add(member);
        }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            yield return ModuleToken;
            yield return Name;
            yield return OpenBrace;

            foreach (var m in members)
                foreach (var item in m.Tokens)
                    yield return item;
            
            yield return CloseBrace;
        }
    }
}
