using BasicSharp.Compiler.Analyzer;
using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicSharp.Compiler.ILEmitter
{
    public abstract class Emitter<T>
        where T : SyntaxNode
    {
        protected CompilationBag compilationBag;

        public Emitter(CompilationBag compilationBag)
        {
            this.compilationBag = compilationBag;
        }

        public abstract void BuildString(StringBuilder builder, T node);
    }
}
