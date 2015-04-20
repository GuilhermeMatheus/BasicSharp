using BasicSharp.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicSharp.Compiler.Parser;
using System.Collections.ObjectModel;
using BasicSharp.Compiler.Parser.Extensions;
using System.Collections;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class ModuleDeclaration : SyntaxNode
    {
        List<ModuleMemberDeclaration> members = new List<ModuleMemberDeclaration>();
        
        public TokenInfo ModuleToken { get; internal set; }
        public TokenInfo Name { get; internal set; }
        public TokenInfo OpenBraceToken { get; internal set; }
        public ReadOnlyCollection<ModuleMemberDeclaration> Members
        {
            get { return members.AsReadOnly(); }
        }
        public TokenInfo CloseBraceToken { get; internal set; }

        public void AddMember(ModuleMemberDeclaration member)
        {
            members.Add(member);
            Accept(member);
        }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            yield return ModuleToken;
            yield return Name;
            yield return OpenBraceToken;

            foreach (var m in members)
                foreach (var item in m.GetTokenEnumerable())
                    yield return item;
            
            yield return CloseBraceToken;
        }

        public override IEnumerable GetChilds()
        {
            yield return ModuleToken;
            yield return Name;
            yield return OpenBraceToken;

            foreach (var m in members)
                yield return m;

            yield return CloseBraceToken;
        }
    }
}
