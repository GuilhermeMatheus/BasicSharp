using BasicSharp.Compiler.Analyzer;
using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace BasicSharp.Compiler.ILEmitter
{
    public class ForEmitter : TacEmitter<ForStatement>
    {
        public ForEmitter(CompilationBag compilationBag, ILocalIndexer localIndexer)
            : base(compilationBag, localIndexer) { }

        public override List<TacUnit> Generate(ForStatement node, string labelPrefix = "IL_", int index = 0)
        {
            var result = new List<TacUnit>();

            var expressionEmitter = TacEmitterFactory.GetEmitterFor(node.Initializer, compilationBag, localIndexer);
            var initializer = expressionEmitter.GenerateTypeTac(node.Initializer, labelPrefix, index);
            result.AddRange(initializer.Item2);

            index = result.Last().LabelIndex + 1;

            var block = TacEmitterFactory.GenerateWithNode(node.Block, compilationBag, localIndexer, labelPrefix, index);
            result.AddRange(block.Item2);

            index = result.Last().LabelIndex + 1;

            var incrementor = expressionEmitter.GenerateTypeTac(node.Incrementor, labelPrefix, index);
            result.AddRange(incrementor.Item2);

            index = result.Last().LabelIndex + 1;

            var condition = expressionEmitter.GenerateTypeTac(node.Condition, labelPrefix, index);
            result.AddRange(condition.Item2);

            index = result.Last().LabelIndex + 1;

            var brTrue = new TacUnit
            {
                LabelPrefix = labelPrefix,
                LabelIndex = index,
                Op = OpCodes.Brtrue,
                Value = GetLabel(initializer.Item2.First().LabelPrefix, initializer.Item2.First().LabelIndex)
            };

            result.Add(brTrue);

            return result;
        }
    }
}
