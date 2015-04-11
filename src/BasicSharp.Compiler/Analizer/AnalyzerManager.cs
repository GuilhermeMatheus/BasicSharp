using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.Analizer
{
    public sealed class AnalyzerManager
    {
        List<AnalysisResult> analysisResults = new List<AnalysisResult>();
        public ReadOnlyCollection<AnalysisResult> AnalysisResults
        {
            get { return analysisResults.AsReadOnly(); }
        }

        Analyzer<SyntaxNode> a;

        public AnalyzerManager(CompilationBag compilationBag)
        {

        }
    }
}
