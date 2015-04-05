using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BasicSharp.Compiler.Parser;
using BasicSharp.Compiler.Parser.Syntaxes;
using BasicSharp.Compiler.Lexer;
using FluentAssertions;
using FluentAssertions.Collections;

namespace BasicSharp.Compiler.Tests.Parser
{
    [TestClass]
    public class CompilationUnitTest
    {
        [TestMethod]
        public void AST_WhenReadingSimpleCompilationUnit()
        {
            var source = @"implements System.Console;
                           implements System.Convert;

                           module m
                           {
                               my int[] a, b, c;
                               my string s;

                               everybody void Main()
                               {

                               }

                               my void m(int a, int b)
                               {

                               }
                           }";
            
            var parser = ParserFactory.FromString(source);
            var result = parser.GetSyntax();
            var m = result as CompilationUnit;

            result.Should().BeOfType<CompilationUnit>();
            m.AsString().Should().BeEquivalentTo(source, "because AsString() override must be concise");
        }

    }
}
