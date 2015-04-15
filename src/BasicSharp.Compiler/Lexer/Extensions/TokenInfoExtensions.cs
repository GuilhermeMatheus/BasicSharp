using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.Lexer.Extensions
{
    public static class TokenInfoExtensions
    {
        public static TokenInfo MakeDoublePlacePartOf(this TokenInfo lessSignificantPart, TokenInfo moreSignificantPart)
        {
            var stringValue = moreSignificantPart.StringValue + "." + lessSignificantPart.StringValue;
            var begin = Math.Min(moreSignificantPart.Begin, lessSignificantPart.Begin);
            var end = begin + stringValue.Length;

            return new TokenInfo
            {
                Kind = SyntaxKind.DoubleLiteral,
                StringValue = stringValue,
                DoubleValue = double.Parse(stringValue, CultureInfo.InvariantCulture),
                Begin = begin,
                End = end,
                IsMalformedToken = lessSignificantPart.IsMalformedToken || moreSignificantPart.IsMalformedToken
            };
        }

        public static Type GetLiteralCLRType(this SyntaxKind type)
        {
            switch (type)
            {
                case SyntaxKind.TrueKeyword:
                case SyntaxKind.FalseKeyword:
                    return typeof(bool);
                case SyntaxKind.StringLiteral:
                    return typeof(string);
                case SyntaxKind.CharLiteral:
                    return typeof(char);
                case SyntaxKind.IntegerLiteral:
                    return typeof(int);
                case SyntaxKind.DoubleLiteral:
                    return typeof(double);
                case SyntaxKind.ByteLiteral:
                    return typeof(byte);
                default:
                    return null;
            }
        }

        public static bool IsLogicalOperator(this SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.AndOperator:
                case SyntaxKind.OrOperator:
                case SyntaxKind.EqualsEqualsOperator:
                case SyntaxKind.ExclamationEqualsToken:
                case SyntaxKind.MinorOperator:
                case SyntaxKind.MinorEqualsOperator:
                case SyntaxKind.MajorOperator:
                case SyntaxKind.MajorEqualsOperator:
                    return true;
                default:
                    return false;
            }
        }
    }
}
