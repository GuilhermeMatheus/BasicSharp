using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicSharp.Compiler.Parser;
using System.Collections.ObjectModel;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class LocalVariableDeclaration : SyntaxNode
    {
        List<VariableDeclarator<AssignmentExpression>> declarators = new List<VariableDeclarator<AssignmentExpression>>();

        public PredefinedType Type { get; internal set; }

        public ReadOnlyCollection<VariableDeclarator<AssignmentExpression>> Declarators
        {
            get { return declarators.AsReadOnly(); }
        }

        public override IEnumerable<Lexer.TokenInfo> GetInternalTokens()
        {
            if (Type != null)
                foreach (var item in Type.Tokens)
                    yield return item;

            foreach (var decl in declarators)
                foreach (var item in decl.Tokens)
                    yield return item;
        }
    }
}
