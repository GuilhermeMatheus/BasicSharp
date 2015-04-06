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
    public class BooleanExpressionsTest
    {
        [TestMethod]
        public void GetSyntax_WithAllStatements_ShouldReturnTheCorrectAST()
        {
            var source = @"my int m()
                           {
                               while(1) 
                               {
                                   if(1)
                                   {
                                       for(1; 1; 1)
                                       {
                                       }
                                       return 1;
                                   }
                                   else
                                   {
                                       if(1)
                                       {
                                            break;
                                       }
                                   }
                               }
                               return;
                           }";

            var parser = ParserFactory.FromString(source);
            var b = parser.GetSyntax();

            b.ToString();
        }

        [TestMethod]
        public void GetSyntax_WithFor_ShouldReturnTheCorrectAST()
        {
            var source = "my void m() { for(int i = 0; i < 10; i++) { } }";
            var parser = ParserFactory.FromString(source);
            var b = parser.GetSyntax();

            b.ToString();
        }

        [TestMethod]
        public void GetSyntax_WithIf_ShouldReturnTheCorrectAST()
        {
            var source = "my void m() { if(1) { } else { } }";
            var parser = ParserFactory.FromString(source);
            var b = parser.GetSyntax();

            b.ToString();
        }

        [TestMethod]
        public void GetSyntax_WithWhile_ShouldReturnTheCorrectAST()
        {
            var source = "my void m() { while(1) { } }";
            var parser = ParserFactory.FromString(source);
            var b = parser.GetSyntax();

            b.ToString();
        }
    }
}
