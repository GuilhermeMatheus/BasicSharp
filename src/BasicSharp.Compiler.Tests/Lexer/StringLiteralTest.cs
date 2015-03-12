using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BasicSharp.Compiler.Tests.SlidingText;
using BasicSharp.Compiler.Lexer;

using lxr = BasicSharp.Compiler.Lexer;

namespace BasicSharp.Compiler.Tests.Lexer
{
    [TestClass]
    public class StringLiteralTest
    {
        [TestMethod]
        public void GetTokens_WhenReadString_MustReturnStringToken()
        {
            var text = SlidingTextSources.GetSlidingTextWith("\"comment here\"");
            var lexer = new lxr.Lexer(text);

            var expected = new TokenInfo
            {
                Begin = 0,
                End = 14,
                StringValue = "\"comment here\"",
                Kind = SyntaxKind.StringLiteral
            };

            lexer.GetTokens().First().Should().Be(expected);
        }
    }
}
