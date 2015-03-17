using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BasicSharp.Compiler.Tests.SlidingText;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Collections;
using BasicSharp.Compiler.Lexer;

namespace BasicSharp.Compiler.Tests.Lexer
{
    [TestClass]
    public class BooleanTest
    {
        [TestMethod]
        public void GetTokens_WhenReadTrue_MustReturnTrueTokenValue()
        {
            var lexer = LexerFactory.FromString("true");
            var token = lexer.GetTokens().First();

            token.Kind.Should().Be(SyntaxKind.TrueKeyword);
            token.BooleanValue.Should().BeTrue();
        }

        [TestMethod]
        public void GetTokens_WhenReadFalse_MustReturnFalseTokenValue()
        {
            var lexer = LexerFactory.FromString("false");
            var token = lexer.GetTokens().First();

            token.Kind.Should().Be(SyntaxKind.FalseKeyword);
            token.BooleanValue.Should().BeFalse();
        }
    }
}
