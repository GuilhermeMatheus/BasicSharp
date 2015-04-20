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

            var expressionEmitter = new ExpressionEmitter(compilationBag, localIndexer);
            var initializer = expressionEmitter.Generate(node.Initializer, labelPrefix, index);
            result.AddRange(initializer);

            index = result.Last().LabelIndex + 1;

            var statementEmitter = new StatementEmitter(compilationBag, localIndexer);
            var block = statementEmitter.Generate(node.Block, labelPrefix, index);
            result.AddRange(block);

            index = result.Last().LabelIndex + 1;

            var incrementor = expressionEmitter.Generate(node.Incrementor, labelPrefix, index);
            result.AddRange(incrementor);

            index = result.Last().LabelIndex + 1;

            var condition = expressionEmitter.Generate(node.Condition, labelPrefix, index);
            result.AddRange(condition);

            index = result.Last().LabelIndex + 1;

            var brTrue = new TacUnit
            {
                LabelPrefix = labelPrefix,
                LabelIndex = index,
                Op = OpCodes.Brtrue,
                Value = GetLabel(initializer.First().LabelPrefix, initializer.First().LabelIndex)
            };

            result.Add(brTrue);

            return result;
        }
    }
}
