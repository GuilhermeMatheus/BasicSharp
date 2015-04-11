using BasicSharp.Compiler.Lexer;
using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.Parser
{
    public class SyntacticException : Exception
    {
        public SyntaxNode MalformedSyntax { get; set; }
        public SyntaxKind[] ExpectedSyntaxes { get; set; }
        public string[] ExpectedStrings { get; set; }

        SyntacticException(string message, SyntaxNode malformedSyntax)
            : base(message)
        {
            this.MalformedSyntax = malformedSyntax;
        }

        public SyntacticException(string message, SyntaxNode malformedSyntax, params string[] expectedStrings)
            : this(message, malformedSyntax)
        {
            this.ExpectedStrings = expectedStrings;
        }

        public SyntacticException(string message, SyntaxNode malformedSyntax, params SyntaxKind[] expectedSyntaxes)
            : this(message, malformedSyntax)
        {
            this.ExpectedSyntaxes = expectedSyntaxes;
        }
    }
}