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
    public class FieldAndMethodTest
    {
        [TestMethod]
        public void AST_WhenReadingMethod()
        {
            var source = "my void m(int a, int b) { }";
            var parser = ParserFactory.FromString(source);
            var result = parser.GetSyntax();
            var m = result as MethodDeclaration;

            result.Should().BeOfType<MethodDeclaration>();
            m.Name.Should().Be("m");
            m.Arity.Should().Be(2, "because it has only 2 parameters");
            m.IsPublic.Should().BeFalse("because it's a private method");
            m.ToString().Should().BeEquivalentTo(source, "because ToString() override must be concise");
        }

        [TestMethod]
        public void AST_WhenReadingMultiFieldDeclaration()
        {
            var source = "everybody int a, b, c, d    , e ,    f      ;";
            var parser = ParserFactory.FromString(source);
            var result = parser.GetSyntax();
            var f = result as FieldDeclaration;

            result.Should().BeOfType<FieldDeclaration>();
            f.Declaration.Type.Kind.Should().Be(SyntaxKind.IntKeyword, "because its a Integer field declaration");
            f.Declaration.Declarators.Count.Should().Be(6, "because it has 6 fields declarators");
            f.IsPublic.Should().BeTrue("because it's a public fields");
            f.ToString().Should().BeEquivalentTo(source, "because ToString() override must be concise");
        }
    }
}
