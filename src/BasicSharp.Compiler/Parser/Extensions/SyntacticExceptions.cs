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
            if (expected.Count() == 1)
                return ExpectedTokenNotFound(expected[0], malformedNode);

            var lastToken = malformedNode.Tokens.Last();
            var msg = "Expected {0} at line {1} and column {2}";
            msg = string.Format(msg, string.Join(" or ", expected), lastToken.Line, lastToken.EndColumn);

            return new SyntacticException(msg, malformedNode, expected);
        }
        
        static SyntacticException ExpectedTokenNotFound(SyntaxKind expected, SyntaxNode malformedNode)
        {
            var lastToken = malformedNode.Tokens.Last();

            var msg = "'{0}' not foundc in SyntaxNode of type {3}";
            msg = string.Format(msg, expected, lastToken.Line, lastToken.EndColumn, malformedNode.DisplayMember);

            return new SyntacticException(msg, malformedNode, expected);
        }
    }
}
