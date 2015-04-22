using BasicSharp.Compiler.Analyzer;
using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicSharp.Compiler.ILEmitter.Extensions;
using System.Reflection.Emit;

namespace BasicSharp.Compiler.ILEmitter
{
    public class UnaryExpressionEmitter : ExpressionEmitter<UnaryExpression>
    {
        public UnaryExpressionEmitter(CompilationBag compilationBag, ILocalIndexer localIndexer)
            : base(compilationBag, localIndexer) { }

        public override Tuple<Type, List<TacUnit>> GenerateWithType(UnaryExpression node, string labelPrefix = "IL_", int index = 0)
        {
            var result = TacEmitterFactory.GenerateWithNode(node.Expression, compilationBag, localIndexer, labelPrefix, index);
            var label = result.Item2.GetNextLabel();

            var neg = new TacUnit
            {
                LabelPrefix = label.Item1,
                LabelIndex = label.Item2,
                Op = OpCodes.Neg
            };

            result.Item2.Add(neg);

            return result;
        }
    }
}
