using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicSharp.Compiler.Analizer
{
    public abstract class Analyzer<T> 
        where T : SyntaxNode
    {
        protected CompilationBag compilationBag;
        public T Node { get; internal set; }
        
        public Analyzer(CompilationBag compilationBag, T node)
        {
            this.compilationBag = compilationBag;
            this.Node = node;
        }

        public abstract AnalysisResult GetAnalysis();

    }
}
