using BasicSharp.Compiler.Analyzer;
using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicSharp.Compiler.ILEmitter.Extensions;

namespace BasicSharp.Compiler.ILEmitter
{
    public class BlockEmitter : ExpressionEmitter<BlockStatement>
    {
        public BlockEmitter(CompilationBag compilationBag, ILocalIndexer localIndexer)
            : base(compilationBag, localIndexer) { }

        public override Tuple<Type, List<TacUnit>> GenerateWithType(BlockStatement node, string labelPrefix = "IL_", int index = 0)
        {
            var result = new List<TacUnit>();

            foreach (var item in node.Statements)
            {
                var itemEmitter = TacEmitterFactory.GenerateWithNode(item, compilationBag, localIndexer, labelPrefix, index);
                result.AddRange(itemEmitter.Item2);
                index = result.GetNextLabel().Item2;
            }

            return new Tuple<Type, List<TacUnit>>(null, result);
        }
    }
}
