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
    public class MethodEmitter : Emitter<MethodDeclaration>, ILocalIndexer
    {
        #region consts
        const string ACCESSOR_MODIFIER = "<ACCESSOR_MODIFIER>";
        const string NAME = "<NAME>";
        const string METHOD_HEADER = @"
.method <ACCESSOR_MODIFIER> hidebysig static 
        void <NAME> () cil managed 
    {";

        const string LOCAL_SPECIFIER = "    [{0}] {1}";
        #endregion

        Dictionary<string, int> indexByLocalInit = new Dictionary<string, int>();

        public MethodEmitter(CompilationBag compilationBag)
            : base(compilationBag) { }

        public int GetLocalIndex(string name)
        {
            if (indexByLocalInit.ContainsKey(name))
                return indexByLocalInit[name];
            
            return -1;
        }

        public override void BuildString(StringBuilder builder, MethodDeclaration node)
        {
            var methodHeader = METHOD_HEADER.Replace(NAME, node.Identifier.StringValue)
                                            .Replace(ACCESSOR_MODIFIER, node.IsPublic ? "public" : "private");
            builder.AppendLine(methodHeader);

            var locals = node.Childs.OfType<VariableDeclaration>();
            writeLocals(builder, locals);
            builder.AppendLine();

            var statements = node.Block.Statements;
            var statEmitter = new StatementEmitter(compilationBag, (ILocalIndexer)this);
            foreach (var item in statements)
                statEmitter.BuildString(builder, item);

            //MethodHeader closeBrace
            builder.AppendLine("}");
        }

        void writeLocals(StringBuilder builder, IEnumerable<VariableDeclaration> locals)
        {
            if (!locals.Any())
                return;

            builder.AppendLine(".locals init (");

            var i = 0;
            var localsInit = new List<string>();
            foreach (var item in locals)
            {
                var type = item.Type.GetCLRType().GetMsilTypeName();
                foreach (var decl in item.Declarators)
                {
                    localsInit.Add(string.Format(LOCAL_SPECIFIER, i++, type));
                    indexByLocalInit.Add(decl.Identifier.StringValue, i);
                    //compilationBag.Analyzer.SetLocalInitIndex(decl, i);
                }
            }

            builder.AppendLine();
            builder.Append(string.Join(",\n", localsInit));
            builder.AppendLine();

            builder.AppendLine(")");
        }
                
    }
}
