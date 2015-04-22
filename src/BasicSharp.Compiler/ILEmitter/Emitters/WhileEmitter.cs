using BasicSharp.Compiler.Analyzer;
using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using BasicSharp.Compiler.ILEmitter.Extensions;

namespace BasicSharp.Compiler.ILEmitter
{
    public class WhileEmitter : ExpressionEmitter<WhileStatement>
    {
        public WhileEmitter(CompilationBag compilationBag, ILocalIndexer localIndexer)
            : base(compilationBag, localIndexer) { }

        public override Tuple<Type, List<TacUnit>> GenerateWithType(WhileStatement node, string labelPrefix = "IL_", int index = 0)
        {
            var result = new List<TacUnit>();

            var conditionEmitter = TacEmitterFactory.GetEmitterFor(node.Condition, compilationBag, localIndexer);
            var condition = conditionEmitter.GenerateTypeTac(node.Condition, labelPrefix, index);

            result.AddRange(condition.Item2);
            index = result.GetNextLabel().Item2;

            var brFalse = new TacUnit
            {
                LabelPrefix = labelPrefix,
                LabelIndex = index++,
                Op = OpCodes.Brfalse
            };

            var block = TacEmitterFactory.GenerateWithNode(node.Block, compilationBag, localIndexer, labelPrefix, index);
            var nextLabel = block.Item2.GetNextLabel();
            var idxBrCondition = nextLabel != null ? nextLabel.Item2 : index;

            var brCondition = new TacUnit
            {
                LabelPrefix = labelPrefix,
                LabelIndex = idxBrCondition,
                Op = OpCodes.Br,
                Value = GetLabel(condition.Item2.First().LabelPrefix, condition.Item2.First().LabelIndex)
            };

            brFalse.Value = GetLabel(brCondition.LabelPrefix, brCondition.LabelIndex + 1);

            result.Add(brFalse);
            result.AddRange(block.Item2);
            result.Add(brCondition);

            return new Tuple<Type,List<TacUnit>>(null, result);
        }
    }
}
