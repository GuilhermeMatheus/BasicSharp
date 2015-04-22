using BasicSharp.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.Parser.Extensions
{
    public static class SyntaxKindExtensions
    {
        public static bool IsModifier(this SyntaxKind syntaxKind)
        {
            return syntaxKind == SyntaxKind.EverybodyKeyword ||
                   syntaxKind == SyntaxKind.MyKeyword;
        }

        public static bool IsTypeNotContextual(this SyntaxKind syntaxKind)
        {
            switch (syntaxKind)
            {
                case SyntaxKind.VoidKeyword:
                case SyntaxKind.BoolKeyword:
                case SyntaxKind.IntKeyword:
                case SyntaxKind.DoubleKeyword:
                case SyntaxKind.ByteKeyword:
                case SyntaxKind.StringKeyword:
                case SyntaxKind.CharKeyword:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsTrivia(this SyntaxKind syntaxKind)
        {
            switch (syntaxKind)
            {
                case SyntaxKind.TabTrivia:
                case SyntaxKind.WhitespaceTrivia:
                case SyntaxKind.EndOfLineTrivia:
                case SyntaxKind.SingleLineCommentTrivia:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsIn(this SyntaxKind syntaxKind, params SyntaxKind[] expecteds)
        {
            foreach (var item in expecteds)
                if (syntaxKind.Equals(item))
                    return true;

            return false;
        }

        public static bool IsAssignmentOperator(this SyntaxKind syntaxKind)
        {
            switch (syntaxKind)
            {
                case SyntaxKind.EqualsToken:
                case SyntaxKind.PlusEqualsToken:
                case SyntaxKind.MinusEqualsToken:
                case SyntaxKind.AsteriskEqualsToken:
                case SyntaxKind.SlashEqualsToken:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsComparatorOperator(this SyntaxKind syntaxKind)
        {
            switch (syntaxKind)
            {
                case SyntaxKind.ExclamationEqualsToken:
                case SyntaxKind.EqualsEqualsOperator:
                case SyntaxKind.MinorOperator:
                case SyntaxKind.MinorEqualsOperator:
                case SyntaxKind.MajorOperator:
                case SyntaxKind.MajorEqualsOperator:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsArithmeticOperator(this SyntaxKind syntaxKind)
        {
            switch (syntaxKind)
            {
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                case SyntaxKind.AsteriskToken:
                case SyntaxKind.SlashToken:
                case SyntaxKind.ModOperator:
                    return true;
                default:
                    return false;
            }
        }
    
        public static bool IsLiteral(this SyntaxKind syntaxKind)
        {
            switch (syntaxKind)
            {
                case SyntaxKind.CharLiteral:
                case SyntaxKind.StringLiteral:
                case SyntaxKind.ByteLiteral:
                case SyntaxKind.DoubleLiteral:
                case SyntaxKind.IntegerLiteral:
                case SyntaxKind.TrueKeyword:
                case SyntaxKind.FalseKeyword:
                case SyntaxKind.NullKeyword:
                    return true;
                default:
                    return false;
            }
        }
    }
}
