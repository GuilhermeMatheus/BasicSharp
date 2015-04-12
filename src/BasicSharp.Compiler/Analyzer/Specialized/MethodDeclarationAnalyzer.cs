using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.Analyzer
{
    public class MethodDeclarationAnalyzer : SpecializedAnalyzer<MethodDeclaration>
    {
        public MethodDeclarationAnalyzer(AnalyzerManager manager) 
            : base (manager) { }

        public override IEnumerable<AnalysisResult> GetAnalysis(MethodDeclaration node)
        {
            throw new NotImplementedException();
        }
    }
}
