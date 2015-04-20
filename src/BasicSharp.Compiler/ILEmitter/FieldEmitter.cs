using BasicSharp.Compiler.Analyzer;
using BasicSharp.Compiler.Parser.Extensions;
using BasicSharp.Compiler.Parser.Syntaxes;
using BasicSharp.Compiler.ILEmitter.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicSharp.Compiler.ILEmitter
{
    public class FieldEmitter : Emitter<FieldDeclaration>
    {
        const string FORMAT = ".field {0} static {1} {2}\n";
        
        public FieldEmitter(CompilationBag compilationBag)
            : base(compilationBag) { }

        public override void BuildString(StringBuilder builder, FieldDeclaration node)
        {
            var msilType = node.Declaration.Type.GetCLRType().GetMsilTypeName();
            var accessorModifier = node.IsPublic ? "public" : "private";

            foreach (var item in node.Declaration.Declarators)
                builder.AppendFormat(FORMAT, accessorModifier, msilType, item.Identifier.StringValue);
        }
    }
}
