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
    }
}
