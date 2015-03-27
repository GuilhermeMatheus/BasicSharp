using BasicSharp.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicSharp.Compiler.Parser.Extensions;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public static class VariableDeclaration
    {
        public static VariableDeclaration<TD> WithDeclarator<TD>(VariableDeclarator<TD> declarator)
            where TD : AssignmentExpression
        {
            var result = new VariableDeclaration<TD>();
            result.AddDeclarator(declarator);

            return result;
        }
    }

    public class VariableDeclaration<T> : SyntaxNode 
        where T : AssignmentExpression
    {
        List<VariableDeclarator<T>> declarators = new List<VariableDeclarator<T>>();

        public PredefinedType Type { get; internal set; }
        public ReadOnlyCollection<VariableDeclarator<T>> Declarators
        {
            get { return new ReadOnlyCollection<VariableDeclarator<T>>(declarators); }
        }

        public void AddDeclarator(VariableDeclarator<T> declarator)
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
    }
}
