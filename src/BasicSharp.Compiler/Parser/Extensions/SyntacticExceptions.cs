using BasicSharp.Compiler.Lexer;
using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.Parser.Extensions
{
    public static class SyntacticExceptions
    {
        public static SyntacticException ExpectedTokenNotFound(SyntaxNode malformedNode, params SyntaxKind[] expected)
        {
            return ExpectedTokenNotFound(malformedNode, expected.Select(x => x.ToString()).ToArray());
        }
        public static SyntacticException ExpectedTokenNotFound(int line, int endColumn, params SyntaxKind[] expected)
        {
            return ExpectedTokenNotFound(line, endColumn, expected.Select(x => x.ToString()).ToArray());
        }
        public static SyntacticException ExpectedTokenNotFound(SyntaxNode malformedNode, params string[] expected)
        {
            var lastToken = malformedNode.Tokens.Last();
            return ExpectedTokenNotFound(lastToken.Line, lastToken.EndColumn, expected);
        }
        public static SyntacticException ExpectedTokenNotFound(int line, int endColumn, params string[] expected)
        {
            if (expected.Count() == 1)
                return ExpectedTokenNotFound(expected[0], null);

            var msg = "Expected {0} at line {1} and column {2}";
            msg = string.Format(msg, string.Join(" or ", expected), line, endColumn);

            return new SyntacticException(msg, null, expected);
        }
        public static SyntacticException ExpectedTokenNotFound(SyntaxKind expected, SyntaxNode malformedNode)
        {
            return ExpectedTokenNotFound(expected.ToString(), malformedNode);
        }
        public static SyntacticException ExpectedTokenNotFound(string expected, SyntaxNode malformedNode)
        {
            var lastToken = malformedNode.Tokens.Last();
            return ExpectedTokenNotFound(lastToken.Line, lastToken.EndColumn, expected, malformedNode);
        }
        public static SyntacticException ExpectedTokenNotFound(int line, int endColumn, string expected, SyntaxNode malformedNode)
        {
            var msg = "'{0}' not found";

            if (malformedNode != null)
            {
                msg += " in SyntaxNode of type {3}";
                msg = string.Format(msg, expected, line, endColumn, malformedNode.DisplayMember);
            }
            else
            {
                msg = string.Format(msg, expected, line, endColumn);
            }

            return new SyntacticException(msg, malformedNode, expected);
        }
    }
}
