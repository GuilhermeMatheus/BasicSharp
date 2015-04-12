using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicSharp.Compiler.Analyzer
{
    public class CompilationUnitAnalyzer : SpecializedAnalyzer<CompilationUnit>
    {
        public CompilationUnitAnalyzer(AnalyzerManager manager)
            : base(manager) { }

        public override IEnumerable<AnalysisResult> GetAnalysis(CompilationUnit node)
        {
            var aImplements = AnalyzerFactory.GetAnalyzerFor<ImplementsDirective>(manager);

            var resImplements = from item in node.ImplementsDirectives
                                select aImplements.GetAnalysis(item);

            foreach (var item in resImplements.SelectMany(i => i))
                yield return item;

            var aModule = AnalyzerFactory.GetAnalyzerFor<ModuleDeclaration>(manager);
            foreach (var item in aModule.GetAnalysis(node.Module))
                yield return item;
        }

        
        IEnumerable<AnalysisResult> validateNames()
        {

            //foreach (var item in Node.modu)
            //{
                
            //}


            yield break;
        }
    }
}
