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
    public class MathExpressionsTest
    {
        [TestMethod]
        public void AST_WhenReadingMultiplicativeAndAdditive_ShouldReturnTheCorrectAST()
        {
            var source = "1*2+3";
            var parser = ParserFactory.FromString(source);
            var result = parser.GetSyntax();

            result.Should().BeOfType<BinaryExpression>();
            
            var b = result as BinaryExpression;

            b.LeftSide.Should().BeOfType<BinaryExpression>();
            b.OperatorToken.Kind.Should().Be(SyntaxKind.PlusToken);
            b.RightSide.Should().BeOfType<LiteralExpression>();

            var leftSide = b.LeftSide as BinaryExpression;

            leftSide.LeftSide.Should().BeOfType<LiteralExpression>();
            leftSide.OperatorToken.Kind.Should().Be(SyntaxKind.AsteriskToken);
            leftSide.RightSide.Should().BeOfType<LiteralExpression>();

            b.EvaluateTreeValueAsInt().ShouldBeEquivalentTo(1 * 2 + 3);
        }

        [TestMethod]
        public void AST_WhenReadingAdditiveAndMultiplicative_ShouldReturnTheCorrectAST()
        {
            var source = "1+2*3";
            var parser = ParserFactory.FromString(source);
            var result = parser.GetSyntax();

            result.Should().BeOfType<BinaryExpression>();

            var b = result as BinaryExpression;

            b.LeftSide.Should().BeOfType<LiteralExpression>();
            b.OperatorToken.Kind.Should().Be(SyntaxKind.PlusToken);
            
            b.RightSide.Should().BeOfType<BinaryExpression>();
            var rightSide = b.RightSide as BinaryExpression;

            rightSide.LeftSide.Should().BeOfType<LiteralExpression>();
            rightSide.OperatorToken.Kind.Should().Be(SyntaxKind.AsteriskToken);
            rightSide.RightSide.Should().BeOfType<LiteralExpression>();

            b.EvaluateTreeValueAsInt().ShouldBeEquivalentTo(1 + 2 * 3);
        }

        [TestMethod]
        public void AST_ComplexMathExpression_Evalutating()
        {
            var source = "1 + (2 + 3) % 5 - 2 * 10 \\ 2";
            var parser = ParserFactory.FromString(source);
            var b = parser.GetSyntax() as BinaryExpression;

            int expected = 1 + (2 + 3) % 5 - 2 * 10 / 2;
            var result = b.EvaluateTreeValueAsInt();

            result.ShouldBeEquivalentTo(expected);
        }


    }
}
