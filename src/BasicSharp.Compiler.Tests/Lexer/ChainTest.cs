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
    public class ChainTest
    {
        [TestMethod]
        public void GetTokens_WhenReadForStatementChain_MustReturnCorrectTokenList()
        {
            var lexer = LexerFactory.FromString("for (int i = 0; i < max(); i+=1) { }");

            SyntaxKind[] expectedKinds = { SyntaxKind.ForKeyword, SyntaxKind.WhitespaceTrivia, SyntaxKind.OpenParenToken,
                                           SyntaxKind.IntKeyword, SyntaxKind.WhitespaceTrivia, SyntaxKind.Identifier,
                                           SyntaxKind.WhitespaceTrivia, SyntaxKind.EqualsToken, SyntaxKind.WhitespaceTrivia,
                                           SyntaxKind.IntegerLiteral, SyntaxKind.SemicolonToken, SyntaxKind.WhitespaceTrivia,
                                           SyntaxKind.Identifier, SyntaxKind.WhitespaceTrivia, SyntaxKind.MinorOperator,
                                           SyntaxKind.WhitespaceTrivia, SyntaxKind.Identifier, SyntaxKind.OpenParenToken,
                                           SyntaxKind.CloseParenToken, SyntaxKind.SemicolonToken, SyntaxKind.WhitespaceTrivia,
                                           SyntaxKind.Identifier, SyntaxKind.PlusEqualsToken, SyntaxKind.IntegerLiteral,
                                           SyntaxKind.CloseParenToken, SyntaxKind.WhitespaceTrivia, SyntaxKind.OpenBraceToken,
                                           SyntaxKind.WhitespaceTrivia, SyntaxKind.CloseBraceToken };

            var tokensInList = lexer.GetTokens().ToList();
            tokensInList.Select(x => x.Kind).Should().Equal(expectedKinds);
        }
    }
}
