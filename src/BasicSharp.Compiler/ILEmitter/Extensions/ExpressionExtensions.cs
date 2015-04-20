using BasicSharp.Compiler.Lexer;
using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.ILEmitter.Extensions
{
    public static class ExpressionExtensions
    {
        public static object GetLiteralExpressionValue(this Expression expression)
        {
            var binary = expression as BinaryExpression;
            if (binary != null)
                switch (binary.OperatorToken.Kind)
                {
                    case SyntaxKind.PlusToken:
                        return (double)binary.LeftSide.GetLiteralExpressionValue() + (double)binary.RightSide.GetLiteralExpressionValue();
                    case SyntaxKind.MinusToken:
                        return (double)binary.LeftSide.GetLiteralExpressionValue() - (double)binary.RightSide.GetLiteralExpressionValue();
                    case SyntaxKind.AsteriskToken:
                        return (double)binary.LeftSide.GetLiteralExpressionValue() * (double)binary.RightSide.GetLiteralExpressionValue();
                    case SyntaxKind.SlashToken:
                        return (double)binary.LeftSide.GetLiteralExpressionValue() / (double)binary.RightSide.GetLiteralExpressionValue();
                    case SyntaxKind.ModOperator:
                        return (double)binary.LeftSide.GetLiteralExpressionValue() % (double)binary.RightSide.GetLiteralExpressionValue();
                    default:
                        throw new NotImplementedException();
                }
        
            var literal = expression as LiteralExpression;
            if (literal != null)
                switch (literal.Value.Kind)
                {
                    case SyntaxKind.StringLiteral:
                        return literal.Value.StringValue;
                    case SyntaxKind.CharLiteral:
                        return literal.Value.CharValue;
                    case SyntaxKind.IntegerLiteral:
                        return literal.Value.IntValue;
                    case SyntaxKind.DoubleLiteral:
                        return literal.Value.DoubleValue;
                    case SyntaxKind.ByteLiteral:
                        return literal.Value.ByteValue;
                    case SyntaxKind.FalseKeyword:
                        return false;
                    case SyntaxKind.TrueKeyword:
                        return true;
                    default:
                        break;
                }

            var parenthesed = expression as ParenthesedExpression;
            if (parenthesed != null)
                return parenthesed.InnerExpression.GetLiteralExpressionValue();

            var unary = expression as UnaryExpression;
            if (unary != null)
                return unary.SignalToken.Kind ==
                    SyntaxKind.PlusToken ? unary.Expression.GetLiteralExpressionValue() :
                                          -(double)unary.Expression.GetLiteralExpressionValue();

            if (expression is AssignmentExpression)
                return (expression as AssignmentExpression).Expression.GetLiteralExpressionValue();

            throw new NotImplementedException();
        }
    }
}
