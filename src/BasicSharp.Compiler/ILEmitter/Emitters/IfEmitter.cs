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
    public class IfEmitter : ExpressionEmitter<IfStatement> 
    {
        public IfEmitter(CompilationBag compilationBag, ILocalIndexer localIndexer)
            : base(compilationBag, localIndexer) { }
        
        public override Tuple<Type, List<TacUnit>> GenerateWithType(IfStatement node, string labelPrefix = "IL_", int index = 0)
        {
            var rand = new Random();
            var result = new List<TacUnit>();

            var conditionEmitter = TacEmitterFactory.GetEmitterFor(node.Condition, compilationBag, localIndexer);
            var condition = conditionEmitter.GenerateTypeTac(node.Condition, labelPrefix, index);
            
            result.AddRange(condition.Item2);
            index = result.GetNextLabel().Item2;

            var brF = new TacUnit
            {
                LabelPrefix = labelPrefix,
                LabelIndex = index++,
                Op = OpCodes.Brfalse
            };

            var blockEmitter = new BlockEmitter(compilationBag, localIndexer);
            var then = blockEmitter.Generate(node.Then, labelPrefix, index);

            var thenLastTac = then.Last();
            brF.Value = GetLabel(labelPrefix, thenLastTac.LabelIndex + 2);

            result.Add(brF);
            result.AddRange(then);

            var endThenBr = new TacUnit
            {
                LabelPrefix = labelPrefix,
                LabelIndex = result.GetNextLabel().Item2,
                Op = OpCodes.Br
            };

            if (node.Else != null) {
                result.Add(endThenBr);

                var _else = blockEmitter.Generate(node.Else, labelPrefix, result.GetNextLabel().Item2);
                result.AddRange(_else);
            }

            result.Add(new TacUnit
            {
                LabelPrefix = labelPrefix,
                LabelIndex = result.GetNextLabel().Item2,
                Op = OpCodes.Nop
            });

            endThenBr.Value = GetLabel(labelPrefix, result.Last().LabelIndex);

            return new Tuple<Type,List<TacUnit>>(null, result);
        }
    }
}
