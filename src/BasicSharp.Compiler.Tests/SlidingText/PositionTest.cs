using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace BasicSharp.Compiler.Tests.SlidingText
{
    [TestClass]
    public class PositionTest
    {
        [TestMethod]
        public void Position_WhenJustPeeking_ShouldBeZero()
        {
            var source = "123.123.123";
            var text = SlidingTextSources.GetSlidingTextWith(source);

            for (int i = 0; i < source.Length; i++)
                text.Peek(i);

            text.Offset.Should().Be(0, "because Next has never been called");
        }

        [TestMethod]
        public void Position_WhenCallingNext_ShouldBeCorrect()
        {
            var source = "123.123.123";
            var text = SlidingTextSources.GetSlidingTextWith(source);

            text.Next(5);

            text.Offset.Should().Be(5, "because Next has been called by 5 times");
        }
    }
}
