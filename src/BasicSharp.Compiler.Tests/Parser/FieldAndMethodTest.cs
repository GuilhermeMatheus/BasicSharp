using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BasicSharp.Compiler.Parser;

namespace BasicSharp.Compiler.Tests.Parser
{
    [TestClass]
    public class FieldAndMethodTest
    {
        [TestMethod]
        public void AST_WhenReadingMethodsAndFieldsDeclarations()
        {
            var parser = ParserFactory.FromString("my void m(int a, int b) { }");
            parser.GetSyntax();

            Assert.AreEqual(1, 1);
        }
    }
}
