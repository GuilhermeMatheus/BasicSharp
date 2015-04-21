using BasicSharp.Compiler.Analyzer;
using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicSharp.Compiler.ILEmitter
{
    public class ImplementsEmitter : Emitter<ImplementsDirective>
    {
        List<string> alreadyAppendedAssemblies = new List<string>();

        const string FILE_NAME = "FILE_NAME";
        const string EXTERN_FORMAT = ".assembly extern FILE_NAME {  }";

        public ImplementsEmitter(CompilationBag compilationBag)
            : base(compilationBag) { }

        public override void BuildString(StringBuilder builder, ImplementsDirective node)
        {
            var type = string.Concat(node.FullClassNameTokens.Select(t => t.StringValue));

            //Podemos  fazer isto com segurança, pois, o ImplementsDirectiveAnalyzer
            //garante a existência de apenas um assembly com este tipo definido.
            var assembly = compilationBag.GetAssembliesForClass(type).First();
            var fileName = assembly.GetName().Name;

            if (alreadyAppendedAssemblies.Contains(fileName))
                return;
            else
            {
                alreadyAppendedAssemblies.Add(fileName);
                compilationBag.AddSessionType(assembly.GetType(type));
            }

            var externDirective = EXTERN_FORMAT.Replace(FILE_NAME, fileName);

            builder.AppendLine(externDirective);
        }
    }
}
