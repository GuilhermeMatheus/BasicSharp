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
using BasicSharp.Compiler.ILEmitter;
using System.Globalization;


namespace BasicSharp.Compiler.Tests.Emitter
{
	[TestClass]
	public class DebuggingEmitterTest
	{
		[TestMethod]
		public void EmitterTests()
		{
			var source = @"
implements System.Console;

module helloProgram
{ 
	my int i = 1564654;
	my double d = 1.123;
	my byte b = 255;
	my bool bo = false;
	my string s = ""string"";
	my char c = '0';

	my void Main()
	{
		if (d > 10)
		{
			WriteLine(""maior que 10"");
		}
		else
		{
			WriteLine(""menor que 10"");
		}
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
			var codeGen = new CodeGenerator(bag, syntax);

			var msil = codeGen.Translate();

			msil.ToString();
		}

	}
}
