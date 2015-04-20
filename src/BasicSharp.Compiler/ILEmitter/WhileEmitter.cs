using BasicSharp.Compiler.Analyzer;
using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace BasicSharp.Compiler.ILEmitter
{
    public class WhileEmitter : TacEmitter<WhileStatement>
    {
        public WhileEmitter(CompilationBag compilationBag, ILocalIndexer localIndexer)
            : base(compilationBag, localIndexer) { }

        public override List<TacUnit> Generate(WhileStatement node, string labelPrefix = "IL_", int index = 0)
        {
            var result = new List<TacUnit>();

            var conditionEmitter = new ExpressionEmitter(compilationBag, localIndexer);
            var condition = conditionEmitter.Generate(node.Condition, labelPrefix, index);

            result.AddRange(condition);

            var brFalse = new TacUnit
            {
                LabelPrefix = labelPrefix,
                LabelIndex = condition.Last().LabelIndex + 1,
                Op = OpCodes.Brfalse
            };

            var blockEmitter = new StatementEmitter(compilationBag, localIndexer);
            var block = blockEmitter.Generate(node.Block, labelPrefix, brFalse.LabelIndex + 1);

            var brCondition = new TacUnit
            {
                LabelPrefix = labelPrefix,
                LabelIndex = block.Last().LabelIndex + 1,
                Op = OpCodes.Br,
                Value = GetLabel(condition.First().LabelPrefix, condition.First().LabelIndex)
            };

            brFalse.Value = GetLabel(brCondition.LabelPrefix, brCondition.LabelIndex + 1);

            result.Add(brFalse);
            result.AddRange(block);
            result.Add(brCondition);

            return result;
        }
    }
}
