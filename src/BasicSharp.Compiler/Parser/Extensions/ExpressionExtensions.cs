using BasicSharp.Compiler.Lexer;
using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.Parser.Extensions
{
    public static class ExpressionExtensions
    {
        public static int EvaluateTreeValueAsInt(this Expression expression)
        {
            var binary = expression as BinaryExpression;
            if (binary != null)
                switch (binary.OperatorToken.Kind)
                {
                    case SyntaxKind.PlusToken:
                        return binary.LeftSide.EvaluateTreeValueAsInt() + binary.RightSide.EvaluateTreeValueAsInt();
                    case SyntaxKind.MinusToken:
                        return binary.LeftSide.EvaluateTreeValueAsInt() - binary.RightSide.EvaluateTreeValueAsInt();
                    case SyntaxKind.AsteriskToken:
                        return binary.LeftSide.EvaluateTreeValueAsInt() * binary.RightSide.EvaluateTreeValueAsInt();
                    case SyntaxKind.SlashToken:
                        return binary.LeftSide.EvaluateTreeValueAsInt() / binary.RightSide.EvaluateTreeValueAsInt();
                    case SyntaxKind.ModOperator:
                        return binary.LeftSide.EvaluateTreeValueAsInt() % binary.RightSide.EvaluateTreeValueAsInt();
                    default:
                        throw new NotImplementedException();
                }

            var literal = expression as LiteralExpression;
            if (literal != null)
                return literal.Value.IntValue;

            var parenthesed = expression as ParenthesedExpression;
            if (parenthesed != null)
                return parenthesed.InnerExpression.EvaluateTreeValueAsInt();

            var unary = expression as UnaryExpression;
            if (unary != null)
                return unary.SignalToken.Kind == 
                    SyntaxKind.PlusToken ? unary.Expression.EvaluateTreeValueAsInt() :
                                          -unary.Expression.EvaluateTreeValueAsInt();

            throw new NotImplementedException();
        }
    }
}
