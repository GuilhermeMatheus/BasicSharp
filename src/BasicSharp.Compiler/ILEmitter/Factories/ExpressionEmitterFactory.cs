using BasicSharp.Compiler.Analyzer;
using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.ILEmitter
{
    public static class StatementEmitterFactory
    {
        public static Emitter<T> GetEmitterFor<T>(T node, CompilationBag compilationBag, ILocalIndexer localIndexer)
            where T : Statement
        {
            throw new NotImplementedException();
        }
    }

    public static class ExpressionEmitterFactory
    {
        public static IExpressionEmitter GetEmitterFor<T>(T node, CompilationBag compilationBag, ILocalIndexer localIndexer)
            where T : Expression
        {
            if (node.GetType() == typeof(MethodInvocationExpression))
                return new MethodInvocationEmitter(compilationBag, localIndexer);

            if (node.GetType() == typeof(AccessorExpression))
                return new AccessorExpressionEmitter(compilationBag, localIndexer);

            if (node.GetType() == typeof(LiteralExpression))
                return new LiteralExpressionEmitter(compilationBag, localIndexer);

            if (node.GetType() == typeof(BinaryExpression) || node.GetType().IsSubclassOf(typeof(BinaryExpression)))
                return new BinaryExpressionEmitter<T>(compilationBag, localIndexer);

            throw null;
        }

        public static Tuple<Type, List<TacUnit>> GenerateWithType<T>(T node, CompilationBag compilationBag, ILocalIndexer localIndexer, string labelPrefix, int index)
            where T : Expression
        {
            var emitter = GetEmitterFor(node, compilationBag, localIndexer);
            return emitter.GenerateWithType(node, labelPrefix, index);
        }

    }
}
