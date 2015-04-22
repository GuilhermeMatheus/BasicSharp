using BasicSharp.Compiler.Analyzer;
using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicSharp.Compiler.ILEmitter
{
    public class ParenthesedExpressionEmitter : ExpressionEmitter<ParenthesedExpression>
    {
        public ParenthesedExpressionEmitter(CompilationBag compilationBag, ILocalIndexer localIndexer)
            : base(compilationBag, localIndexer) { }

        public override Tuple<Type, List<TacUnit>> GenerateWithType(ParenthesedExpression node, string labelPrefix = "IL_", int index = 0)
        {
            return TacEmitterFactory.GenerateWithNode(node.InnerExpression, compilationBag, localIndexer, labelPrefix, index);
        }
    }
}
