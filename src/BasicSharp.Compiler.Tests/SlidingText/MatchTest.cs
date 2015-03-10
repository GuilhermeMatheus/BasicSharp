using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace BasicSharp.Compiler.Tests.SlidingText
{
    using Text = BasicSharp.Compiler.Lexer.SlidingText;

    [TestClass]
    public class MatchTest
    {
        [TestMethod]
        public void AdvanceIfMatches_WhenMatches_MustReturnTrue()
        {
            var text = SlidingTextSources.GetSlidingTextWithFor();
            var desired = "012345";

            var match = text.AdvanceIfMatches(desired);

            match.Should().BeTrue("because the desired string '{0}' matches the SlidingText", desired);
        }

        [TestMethod]
        public void AdvanceIfMatches_WhenMatches_MustAdvance()
        {
            var text = SlidingTextSources.GetSlidingTextWithFor();
            var desired = "012345";

            var match = text.AdvanceIfMatches(desired);
            var peek = text.Peek();

            peek.Should().Be('6', "because the desired string '{0}' matches the SlidingText and the next char is '6'", desired);
        }
    }
}
