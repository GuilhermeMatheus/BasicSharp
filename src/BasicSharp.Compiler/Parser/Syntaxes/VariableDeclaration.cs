using BasicSharp.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicSharp.Compiler.Parser.Extensions;
using System.Collections;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class VariableDeclaration : SyntaxNode 
    {
        List<VariableDeclarator> declarators = new List<VariableDeclarator>();

        public PredefinedType Type { get; internal set; }
        public ReadOnlyCollection<VariableDeclarator> Declarators
        {
            get { return new ReadOnlyCollection<VariableDeclarator>(declarators); }
        }

        public void AddDeclarator(VariableDeclarator declarator)
        {
            this.declarators.Add(declarator);
        }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            foreach (var item in Type.GetTokenEnumerable())
                yield return item;

            foreach (var decl in declarators)
                foreach (var item in decl.GetTokenEnumerable())
                    yield return item;
        }

        public static VariableDeclaration WithDeclarator(VariableDeclarator declarator)
        {
            var result = new VariableDeclaration();
            result.AddDeclarator(declarator);

            return result;
        }

        public override IEnumerable GetChilds()
        {
            yield return Type;
            foreach (var item in Declarators)
                yield return item;
        }
    }
}
