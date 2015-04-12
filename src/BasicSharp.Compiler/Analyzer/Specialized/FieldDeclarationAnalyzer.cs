using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicSharp.Compiler.Analyzer
{
    public class FieldDeclarationAnalyzer : SpecializedAnalyzer<FieldDeclaration>
    {
        public FieldDeclarationAnalyzer(AnalyzerManager manager)
            : base(manager) { }

        public override IEnumerable<AnalysisResult> GetAnalysis(FieldDeclaration node)
        {
            var type = node.Declaration.Type;
            
            foreach (var item in node.Declaration.Declarators.Where(d => d.Assignment != null))
            {
                var aExpression = new ExpressionAnalyzer(manager, item.Assignment.Expression, onlyLiterals: true, expectedType: type);
                foreach (var a in aExpression.GetAnalysis(item))
                    yield return a;
            }
        }
    }
}
