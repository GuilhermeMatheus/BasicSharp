using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicSharp.Compiler.Analyzer
{
    public class ImplementsDirectiveAnalyzer : SpecializedAnalyzer<ImplementsDirective>
    {
        public ImplementsDirectiveAnalyzer(AnalyzerManager manager)
            :base(manager) { }

        public override IEnumerable<AnalysisResult> GetAnalysis(ImplementsDirective node)
        {
            var fullClassName = string.Concat(node.FullClassNameTokens.Select(t => t.StringValue));

            if (!manager.CompilationBag.ContainsClass(fullClassName))
                return new List<AnalysisResult> { AnalysisResults.CouldNotFoundType(node, fullClassName) };

            var assemblies = manager.CompilationBag.GetAssembliesForClass(fullClassName).ToList();
            if (assemblies.Count > 1)
                return new List<AnalysisResult> { AnalysisResults.AmbiguousImplements(node, fullClassName, assemblies) };
            
            if (!manager.CompilationBag.IsValidClass(fullClassName))
                return new List<AnalysisResult> { AnalysisResults.InvalidType(node, fullClassName) };

            return AnalysisResult.EmptyList;
        }
    }
}
