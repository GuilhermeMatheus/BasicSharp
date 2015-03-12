using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BasicSharp.Compiler.Tests.SlidingText;
using System.Linq;
using FluentAssertions;
using BasicSharp.Compiler.Lexer;

using lxr = BasicSharp.Compiler.Lexer;

namespace BasicSharp.Compiler.Tests.Lexer
{
    [TestClass]
    public class NumericLiteralTest
    {
        [TestMethod]
        public void GetTokens_WhenReadFloat_MustReturnFloatToken()
        {
            var text = SlidingTextSources.GetSlidingTextWith("123.123");
            var lexer = new lxr.Lexer(text);

            var expected = new TokenInfo
            {
                Begin = 0,
                End = 7,
                DoubleValue = 123.123,
                StringValue = "123.123",
                Kind = SyntaxKind.DoubleLiteral
            };
            
            lexer.GetTokens().First().Should().Be(expected, "because thats whats the chain \"123.123\" means");
        }

        [TestMethod]
        public void GetTokens_WhenReadByte_MustReturnByteToken()
        {
            var text = SlidingTextSources.GetSlidingTextWith("0b01000100");
            var lexer = new lxr.Lexer(text);

            var expected = new TokenInfo
            {
                Begin = 0,
                End = 10,
                ByteValue = 68,
                StringValue = "0b01000100",
                Kind = SyntaxKind.ByteLiteral
            };

            lexer.GetTokens().First().Should().Be(expected, "because thats whats the chain \"0b01000100\" means");
        }
    }
}
