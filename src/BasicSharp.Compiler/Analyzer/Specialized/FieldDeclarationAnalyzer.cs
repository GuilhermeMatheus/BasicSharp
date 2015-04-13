using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicSharp.Compiler.Parser.Extensions;
using BasicSharp.Compiler.Analyzer.Extensions;

namespace BasicSharp.Compiler.Analyzer
{
    public class FieldDeclarationAnalyzer : SpecializedAnalyzer<FieldDeclaration>
    {
        public FieldDeclarationAnalyzer(AnalyzerManager manager)
            : base(manager) { }

        public override IEnumerable<AnalysisResult> GetAnalysis(FieldDeclaration node)
        {
            var rType = node.Declaration.Type.GetCLRType();
            
            foreach (var item in node.Declaration.Declarators.Where(d => d.Assignment != null))
            {
                var aExpression = new ExpressionAnalyzer(manager, onlyLiterals: true);
                var expression = item.Assignment.Expression;

                foreach (var a in aExpression.GetAnalysis(expression))
                    yield return a;

                var lType = aExpression.Type;
                if (TypeExtensions.GetSuitableType(lType, rType) != rType && lType != null)
                    yield return AnalysisResults.InvalidConversion(item, rType, lType);
            }
        }
    }
}
