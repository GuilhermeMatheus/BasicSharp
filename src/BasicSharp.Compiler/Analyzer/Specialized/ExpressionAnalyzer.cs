using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.Analyzer
{
    public class ExpressionAnalyzer : SpecializedAnalyzer<Expression>
    {
        public bool OnlyLiterals { get; private set; }
        public PredefinedType Type { get; private set; }
        public PredefinedType ExpectedType { get; private set; }

        public ExpressionAnalyzer(AnalyzerManager manager, Expression node, bool onlyLiterals = false, PredefinedType expectedType = null)
            : base(manager)
        {
            this.OnlyLiterals = onlyLiterals;
            this.ExpectedType = expectedType;
        }

        public override IEnumerable<AnalysisResult> GetAnalysis(Expression node)
        {
            throw new NotImplementedException();
        }
    }
}
