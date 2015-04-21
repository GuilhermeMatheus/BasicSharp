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
    public class MethodInvocationStatementEmitter : ExpressionEmitter<MethodInvocationStatement>
    {
        public MethodInvocationStatementEmitter(CompilationBag compilationBag, ILocalIndexer localIndexer)
            : base(compilationBag, localIndexer) { }

        public override Tuple<Type, List<TacUnit>> GenerateWithType(MethodInvocationStatement node, string labelPrefix = "IL_", int index = 0)
        {
            var expression = TacEmitterFactory.GenerateWithNode(node.MethodInvocation, compilationBag, localIndexer, labelPrefix, index);
            var result = expression.Item2;

            if (expression.Item1 != typeof(void))
            {
                index = result.GetNextLabel().Item2;
                result.Add(new TacUnit
                {
                    LabelPrefix = labelPrefix,
                    LabelIndex = index,
                    Op = OpCodes.Pop
                });
            }
            
            return new Tuple<Type,List<TacUnit>>(null, result);
        }
    }
}
