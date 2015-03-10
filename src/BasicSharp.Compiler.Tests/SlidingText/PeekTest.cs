using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BasicSharp.Utils;
using FluentAssertions;
using System.IO;
using System.Text;

namespace BasicSharp.Compiler.Tests.SlidingText
{
    using Text = BasicSharp.Compiler.Lexer.SlidingText;
    
    [TestClass]
    public class PeekTest
    {
        [TestMethod]
        public void JumpUntil_WithChar_IndexValidation()
        {
            var source = "0123456789 for";
            var text = SlidingTextSources.GetSlidingTextWithFor();

            var jumpsToFor = text.JumpUntil('f');

            var expected = source.IndexOf('f');
            jumpsToFor.Should().Be(expected, "because de index of the beggining of '{0}' is {1}", 'f', expected);
        }

        [TestMethod]
        public void Peek_WithJumps_MustAlwaysReturnTheSameValue()
        {
            var text = SlidingTextSources.GetSlidingTextWithFor();

            text.Peek(2).Should().Be(text.Peek(2), "because we dont dont changed the index of the SlidingText");
            text.Peek(0).Should().Be(text.Peek(), "because the calls must be equivalent");

            text.Next(3);
            text.Peek(1).Should().Be(text.Peek(1), "because we dont dont changed the index of the SlidingText after calling Next()");
        }

        [TestMethod]
        public void JumpUntilAndPeek_ShouldBeEquivalent()
        {
            var text = SlidingTextSources.GetSlidingTextWithFor();

            var indexToF = text.JumpUntil('f');
            text.Reset(0);
            text.Peek(indexToF).Should().Be('f', "because de index must be the same");
        }

        
    }
}
