using BasicSharp.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using BasicSharp.Compiler.Parser;
using BasicSharp.Compiler.Parser.Extensions;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class CompilationUnit : SyntaxNode
    {
        List<ImplementsDirective> implementsDirectives = new List<ImplementsDirective>();

        public ReadOnlyCollection<ImplementsDirective> ImplementsDirectives
        {
            get { return implementsDirectives.AsReadOnly(); }
        }
        ModuleDeclaration _module;
        public ModuleDeclaration Module
        {
            get { return _module; }
            internal set
            {
                if (_module != value)
                {
                    _module = value;
                    Accept(value);
                }
            }
        }
        
        public void AddImplementsDirective(ImplementsDirective currImplDir)
        {
            implementsDirectives.Add(currImplDir);
            Accept(currImplDir);
        }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            foreach (var impl in implementsDirectives)
                foreach (var item in impl.GetTokenEnumerable())
                    yield return item;

            foreach (var item in Module.GetTokenEnumerable())
                yield return item;
        }

        public override System.Collections.IEnumerable GetChilds()
        {
            foreach (var impl in implementsDirectives)
                yield return impl;

            yield return Module;
        }
    }
}
