using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.Lexer.Extensions
{
    public class SyntacticExceptions
    {
        public static SyntacticException TooManyCharactersInCharLiteral(SlidingText textSource, TokenInfo malformedToken)
        {
            return new SyntacticException(string.Format("Too many characters in character literal at line {0}, column {1}.", textSource.Line, textSource.Column));
        }

        public static SyntacticException EmptyCharLiteral(SlidingText textSource, TokenInfo malformedToken)
        {
            return new SyntacticException(string.Format("Empty character literal at line {0}, column {1}.", textSource.Line, textSource.Column));
        }

        public static SyntacticException SymbolNotExpected(SlidingText textSource, string symbol, TokenInfo malformedToken, params SyntaxKind[] expectedSyntaxes)
        {
            var ret = SymbolNotExpected(textSource, symbol, malformedToken, expectedSyntaxes.Select(x => x.ToString()).ToArray());
            ret.ExpectedSyntaxes = expectedSyntaxes;

            return ret;
        }

        public static SyntacticException SymbolNotExpected(SlidingText textSource, string symbol, TokenInfo malformedToken, params string[] expectedStrings)
        {
            var chainType = malformedToken != null ? malformedToken.Kind : SyntaxKind.None;
            var message = string.Format("Symbol '{0}' not expected in chain of type {1} at line {2}, column {3}.", symbol, chainType, textSource.Line, textSource.Column);

            if (expectedStrings != null && expectedStrings.Any())
                message += string.Format(" Expected {0}", string.Join(", ", expectedStrings));

            return new SyntacticException(message)
            {
                TextSource = textSource,
                Symbol = symbol,
                MalformedToken = malformedToken,
                ExpectedStrings = expectedStrings
            };
        }
    }
}
