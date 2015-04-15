using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.Analyzer
{
    public static class AnalyzerFactory
    {
        public static SpecializedAnalyzer<T> GetAnalyzerFor<T>(AnalyzerManager manager)
            where T : SyntaxNode
        {
            var t = typeof(T);
            
            if (t == typeof(CompilationUnit))
                return new CompilationUnitAnalyzer(manager) as SpecializedAnalyzer<T>;

            if (t == typeof(ImplementsDirective))
                return new ImplementsDirectiveAnalyzer(manager) as SpecializedAnalyzer<T>;

            if (t == typeof(ModuleDeclaration))
                return new ModuleDeclarationAnalyzer(manager) as SpecializedAnalyzer<T>;

            if (t == typeof(FieldDeclaration))
                return new FieldDeclarationAnalyzer(manager) as SpecializedAnalyzer<T>;

            if (t == typeof(MethodDeclaration))
                return new MethodDeclarationAnalyzer(manager) as SpecializedAnalyzer<T>;

            throw new NotImplementedException();
        }
    }
}