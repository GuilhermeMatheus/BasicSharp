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
    public class ReturnStatementEmitter : ExpressionEmitter<ReturnStatement>
    {
        public ReturnStatementEmitter(CompilationBag compilationBag, ILocalIndexer localIndexer)
            : base(compilationBag, localIndexer) { }

        public override Tuple<Type, List<TacUnit>> GenerateWithType(ReturnStatement node, string labelPrefix = "IL_", int index = 0)
        {
            var result = new List<TacUnit>();

            if (node.Expression != null)
                result = TacEmitterFactory.GenerateWithNode(node.Expression, compilationBag, localIndexer, labelPrefix, index).Item2;

            var label = result.GetNextLabel();

            result.Add(new TacUnit
                {
                    LabelPrefix = label != null ? label.Item1 : labelPrefix,
                    LabelIndex = label != null ? label.Item2 : index,
                    Op = OpCodes.Ret
                });

            return new Tuple<Type, List<TacUnit>>(null, result);
        }
    }
}
