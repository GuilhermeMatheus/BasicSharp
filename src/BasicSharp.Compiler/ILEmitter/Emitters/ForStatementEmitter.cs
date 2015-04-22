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
    public class ForStatementEmitter : ExpressionEmitter<ForStatement>
    {
        public ForStatementEmitter(CompilationBag compilationBag, ILocalIndexer localIndexer)
            : base(compilationBag, localIndexer) { }

        public override Tuple<Type, List<TacUnit>> GenerateWithType(ForStatement node, string labelPrefix = "IL_", int index = 0)
        {
            Tuple<string, int> label = null;
            var result = new List<TacUnit>();

            var init = TacEmitterFactory.GenerateWithNode(node.Initializer, compilationBag, localIndexer, labelPrefix, index);
            result.AddRange(init.Item2);

            label = result.GetNextLabel() ?? new Tuple<string, int>(labelPrefix, index);

            var condition = TacEmitterFactory.GenerateWithNode(node.Condition, compilationBag, localIndexer, label.Item1, label.Item2);
            result.AddRange(condition.Item2);

            label = result.GetNextLabel() ?? label;

            var brFalse = new TacUnit
            {
                LabelPrefix = label.Item1,
                LabelIndex = label.Item2,
                Op = OpCodes.Brfalse
            };

            result.Add(brFalse);

            label = result.GetNextLabel() ?? label;

            var statements = TacEmitterFactory.GenerateWithNode(node.Block, compilationBag, localIndexer, label.Item1, label.Item2);
            result.AddRange(statements.Item2);

            label = result.GetNextLabel() ?? label;

            var incrementor = TacEmitterFactory.GenerateWithNode(node.Incrementor, compilationBag, localIndexer, label.Item1, label.Item2);
            result.AddRange(incrementor.Item2);

            label = result.GetNextLabel() ?? label;

            var br = new TacUnit
            {
                LabelPrefix = label.Item1,
                LabelIndex = label.Item2,
                Op = OpCodes.Br,
                Value = GetLabel(condition.Item2.First())
            };

            result.Add(br);
            label = result.GetNextLabel() ?? label;

            var nop = new TacUnit
            {
                LabelPrefix = label.Item1,
                LabelIndex = label.Item2,
                Op = OpCodes.Nop
            };

            result.Add(nop);

            brFalse.Value = GetLabel(nop);

            return new Tuple<Type,List<TacUnit>>(null, result);
        }
    }
}
