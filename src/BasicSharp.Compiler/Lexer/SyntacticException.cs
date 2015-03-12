using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.Lexer
{
    public class SyntacticException : Exception
    {
        public SlidingText TextSource { get; private set; }
        public string Symbol { get; private set; }
        public SyntaxKind MalformedSyntax { get; private set; }
        public SyntaxKind[] ExpectedSyntaxes { get; private set; }
        public string[] ExpectedStrings { get; private set; }

        SyntacticException() { }

        public static SyntacticException SymbolNotExpected(SlidingText textSource, string symbol, SyntaxKind malformedSyntax, params SyntaxKind[] expectedSyntaxes)
        {
            var message =string.Format("Symbol '{0}' not expected in chain of type {1}. ", symbol, malformedSyntax);

            if (expectedSyntaxes != null && expectedSyntaxes.Any())
                message += string.Format("Expected {2}", string.Join(", ", expectedSyntaxes));

            return new SyntacticException
                {
                    TextSource = textSource,
                    Symbol = symbol,
                    MalformedSyntax = malformedSyntax,
                    ExpectedSyntaxes = expectedSyntaxes
                };
        }

        public static SyntacticException SymbolNotExpected(SlidingText textSource, string symbol, SyntaxKind malformedSyntax, params string[] expectedStrings)
        {
            var message = string.Format("Symbol '{0}' not expected in chain of type {1}. ", symbol, malformedSyntax);

            if (expectedStrings != null && expectedStrings.Any())
                message += string.Format("Expected {2}", string.Join(", ", expectedStrings));

            return new SyntacticException
            {
                TextSource = textSource,
                Symbol = symbol,
                MalformedSyntax = malformedSyntax,
                ExpectedStrings = expectedStrings
            };
        }
    }
}
