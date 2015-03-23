using BasicSharp.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class VariableDeclaration : SyntaxNode
    {
        List<VariableDeclarator> declarators = new List<VariableDeclarator>();

        public TokenInfo Type { get; set; }
        public ReadOnlyCollection<VariableDeclarator> Declarators
        {
            get { return new ReadOnlyCollection<VariableDeclarator>(declarators); }
        }

        public void  AddDeclarator(VariableDeclarator declarator)
        {
            this.declarators.Add(declarator);
        }

        public static VariableDeclaration WithDeclarator(VariableDeclarator declarator)
        {
            var result = new VariableDeclaration();
            result.AddDeclarator(declarator);

            return result;
        }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            yield return Type;

            foreach (var decl in declarators)
                foreach (var item in decl.GetInternalTokens())
                    yield return item;
        }
    }
}
