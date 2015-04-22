using BasicSharp.Compiler.Analyzer;
using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.ILEmitter
{
    public static class TacEmitterFactory
    {
        public static ITacEmitter GetEmitterFor<T>(T node, CompilationBag compilationBag, ILocalIndexer localIndexer)
            where T : SyntaxNode
        {
            //Expressions (return type)
            if (node.GetType() == typeof(ParenthesedExpression))
                return new ParenthesedExpressionEmitter(compilationBag, localIndexer);

            if (node.GetType() == typeof(MethodInvocationExpression))
                return new MethodInvocationEmitter(compilationBag, localIndexer);

            if (node.GetType() == typeof(AccessorExpression))
                return new AccessorExpressionEmitter(compilationBag, localIndexer);

            if (node.GetType() == typeof(LiteralExpression))
                return new LiteralExpressionEmitter(compilationBag, localIndexer);

            if (node.GetType() == typeof(BinaryExpression) || node.GetType().IsSubclassOf(typeof(BinaryExpression)))
                return new BinaryExpressionEmitter<T>(compilationBag, localIndexer);

            if (node.GetType() == typeof(VariableAssignmentExpression))
                return new VariableAssignmentExpressionEmitter(compilationBag, localIndexer);

            if (node.GetType() == typeof(UnaryExpression))
                return new UnaryExpressionEmitter(compilationBag, localIndexer);

            //Statements (no return type)
            if (node.GetType() == typeof(BlockStatement))
                return new BlockEmitter(compilationBag, localIndexer);

            if (node.GetType() == typeof(IfStatement))
                return new IfEmitter(compilationBag, localIndexer);

            if (node.GetType() == typeof(WhileStatement))
                return new WhileEmitter(compilationBag, localIndexer);

            if (node.GetType() == typeof(ForStatement))
                return new ForStatementEmitter(compilationBag, localIndexer);

            if (node.GetType() == typeof(MethodInvocationStatement))
                return new MethodInvocationStatementEmitter(compilationBag, localIndexer);

            if(node.GetType() == typeof(LocalVariableDeclarationStatement))
                return new LocalVariableDeclarationStatementEmitter(compilationBag, localIndexer);

            if (node.GetType() == typeof(LocalVariableAssignmentStatement))
                return new LocalVariableAssignmentStatementEmitter(compilationBag, localIndexer);

            if (node.GetType() == typeof(ReturnStatement))
                return new ReturnStatementEmitter(compilationBag, localIndexer);

            throw new NotImplementedException();
        }

        public static Tuple<Type, List<TacUnit>> GenerateWithNode<T>(T node, CompilationBag compilationBag, ILocalIndexer localIndexer, string labelPrefix, int index)
            where T : SyntaxNode
        {
            var emitter = GetEmitterFor(node, compilationBag, localIndexer);
            return emitter.GenerateTypeTac(node, labelPrefix, index);
        }

    }
}
