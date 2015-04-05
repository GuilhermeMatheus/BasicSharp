using BasicSharp.Compiler.Lexer;
using BasicSharp.Compiler.Parser;
using BasicSharp.Compiler.Parser.Extensions;
using BasicSharp.Compiler.Parser.Syntaxes;
using FluentAssertions;
using FluentAssertions.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BasicSharp.Compiler.Tests.Parser
{
    [TestClass]
    public class StatementTest
    {
        [TestMethod]
        public void GetSyntax_WithOrAndMajorOperators_ShouldReturnTheCorrectAST()
        {
            var source = "getRandom(0.0, 1.0) > 0.5 | allowLowValues";
            var parser = ParserFactory.FromString(source);
            var b = parser.GetSyntax();

            b.ToString();
        }
    }
}
