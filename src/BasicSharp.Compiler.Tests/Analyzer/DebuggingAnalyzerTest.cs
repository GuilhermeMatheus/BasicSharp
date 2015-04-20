using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BasicSharp.Compiler.Tests.SlidingText;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Collections;
using BasicSharp.Compiler.Lexer;
using BasicSharp.Compiler.Parser;
using System.Collections.Generic;
using BasicSharp.Compiler.Parser.Syntaxes;
using BasicSharp.Compiler.Analyzer;

namespace BasicSharp.Compiler.Tests.Lexer
{
	[TestClass]
	public class DebuggingAnalyzerTest
	{
		[TestMethod]
		public void AnalysisResultsTests()
		{
			var source = @"
implements System.Console;
implements System.String;
implements Eu.Nao.Existo;

module helloProgram
{ 
	my bool b1 = true & false;
	my bool b2 = true & ""terceiro"";
	my string hello = ""Hello World"";
	my int hello = ""primeiro"";
	my int n = ""segundo"" + ""terceiro"";
	my char c = 'c';
	my void Main()
	{
		WriteLine(hello);
		metodoQueNaoExiste();
		times = ""nao permitido"";
	}
}";

			var mscorlib = @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\mscorlib.dll";
			var project = new MemoryProject 
			{ 
				Name = "TestProject", 
				AssembliesAddress = new List<string> { mscorlib }, 
				Source = source 
			};
			
			var syntax = ParserFactory.FromString(source).GetSyntax() as CompilationUnit;
            var bag = new CompilationBag(project, syntax);
			var result = bag.Analyzer.GetAnalysisForCompilationUnit();

			result.ToString();

		}

	}
}
