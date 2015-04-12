using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicSharp.Compiler.Analyzer
{
    public abstract class SpecializedAnalyzer<T> 
        where T : SyntaxNode
    {
        protected AnalyzerManager manager;
        
        public SpecializedAnalyzer(AnalyzerManager manager)
        {
            this.manager = manager;
        }

        public abstract IEnumerable<AnalysisResult> GetAnalysis(T node);

        protected IEnumerable<AnalysisResult> AnalysisFor<U>(List<U> syntaxes)
            where U : SyntaxNode
        {
            var aU = AnalyzerFactory.GetAnalyzerFor<U>(manager);

            foreach (var item in syntaxes)
                foreach (var a in aU.GetAnalysis(item))
                    yield return a;
        }
    }
}