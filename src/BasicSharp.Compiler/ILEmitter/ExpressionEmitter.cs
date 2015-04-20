using BasicSharp.Compiler.Analyzer;
using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.ILEmitter
{
    public class ExpressionEmitter : TacEmitter<Expression>
    {
        public ExpressionEmitter(CompilationBag compilationBag, ILocalIndexer localIndexer)
            : base(compilationBag, localIndexer) { }

        public override List<TacUnit> Generate(Expression node, string labelPrefix = "IL_", int index = 0)
        {
            var result = new List<TacUnit>();

            if (node is MethodInvocationExpression)
            {
                var methodInvocation = new MethodInvocationEmitter(compilationBag, localIndexer);
            }

            return result;
        }
    }
}
