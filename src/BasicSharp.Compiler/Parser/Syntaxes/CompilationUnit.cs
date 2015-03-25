using BasicSharp.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using BasicSharp.Compiler.Parser;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class CompilationUnit : SyntaxNode
    {
        List<ImplementsDirective> implementsDirectives = new List<ImplementsDirective>();

        public ReadOnlyCollection<ImplementsDirective> ImplementsDirectives
        {
            get { return implementsDirectives.AsReadOnly(); }
        }
        public ModuleDeclaration Module { get; internal set; }
        
        public void AddImplementsDirective(ImplementsDirective currImplDir)
        {
            implementsDirectives.Add(currImplDir);
        }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            foreach (var impl in implementsDirectives)
                foreach (var item in impl.Tokens)
                    yield return item;

            foreach (var item in Module.Tokens)
                yield return item;
        }
    }
}
