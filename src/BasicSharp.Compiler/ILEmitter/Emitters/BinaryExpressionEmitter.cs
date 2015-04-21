using aex = BasicSharp.Compiler.Analyzer.Extensions;
using BasicSharp.Compiler.Analyzer;
using BasicSharp.Compiler.ILEmitter.Extensions;
using BasicSharp.Compiler.Parser.Extensions;
using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Diagnostics.Contracts;

namespace BasicSharp.Compiler.ILEmitter
{
    public class BinaryExpressionEmitter<T> : ExpressionEmitter<T>
            where T : SyntaxNode
    {
        static readonly Type[] COMPOSITE_TYPES = { 
                                                     typeof(ExclamationEqualsExpression),         //ceq
                                                     typeof(LogicalGreaterOrEqualThanExpression), //clt
                                                     typeof(LogicalLessOrEqualThanExpression)     //cgt
                                                 };

        public BinaryExpressionEmitter(CompilationBag compilationBag, ILocalIndexer localIndexer)
            : base(compilationBag, localIndexer) { }

        public override Tuple<Type, List<TacUnit>> GenerateWithType(T _node, string labelPrefix = "IL_", int index = 0)
        {
            Contract.Assert((typeof(T) == typeof(BinaryExpression) || typeof(BinaryExpression).IsSubclassOf(typeof(T))));

            var node = _node as BinaryExpression;

            var result = new List<TacUnit>();
            
            var left = TacEmitterFactory.GenerateWithNode(node.LeftSide, compilationBag, localIndexer, labelPrefix, index);

            result.AddRange(left.Item2);
            index = result.GetNextLabel().Item2;

            var right = TacEmitterFactory.GenerateWithNode(node.RightSide, compilationBag, localIndexer, labelPrefix, index);

            result.AddRange(right.Item2);
            var label = result.GetNextLabel();
            index = label.Item2;

            var opCode = node.GetOpCodeForBinaryExpression();

            result.Add(new TacUnit
            {
                LabelPrefix = label.Item1,
                LabelIndex = index++,
                Op = opCode
            });

            if (COMPOSITE_TYPES.Contains(node.GetType()))
            {
                result.Add(new TacUnit
                {
                    LabelPrefix = label.Item1,
                    LabelIndex = index++,
                    Op = OpCodes.Ldc_I4_0
                });
                result.Add(new TacUnit
                {
                    LabelPrefix = label.Item1,
                    LabelIndex = index++,
                    Op = OpCodes.Ceq
                });
            }

            Type typeResult;

            if (node.OperatorToken.Kind.IsComparatorOperator())
                typeResult = typeof(bool);
            else
                typeResult = aex.TypeExtensions.GetSuitableType(left.Item1, right.Item1);

            return new Tuple<Type, List<TacUnit>>(typeResult, result);
        }

    }
}
