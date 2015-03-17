using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using BasicSharp.Compiler.Lexer;

namespace BasicSharp.Compiler.Tests.SlidingText
{
    using Text = BasicSharp.Compiler.Lexer.SlidingText;

    [TestClass]
    public class MatchTest
    {
        [TestMethod]
        public void AdvanceIfMatches_WhenMatches_MustReturnTrue()
        {
            var text = SlidingTextFactory.FromString("0123456789 for");
            var desired = "012345";

            var match = text.AdvanceIfMatches(desired);

            match.Should().BeTrue("because the desired string '{0}' matches the SlidingText", desired);
        }

        [TestMethod]
        public void AdvanceIfMatches_WhenMatches_MustAdvance()
        {
            var text = SlidingTextFactory.FromString("0123456789 for");
            var desired = "012345";

            var match = text.AdvanceIfMatches(desired);
            var peek = text.Peek();

            peek.Should().Be('6', "because the desired string '{0}' matches the SlidingText and the next char is '6'", desired);
        }
    }
}
