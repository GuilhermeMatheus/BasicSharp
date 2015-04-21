using BasicSharp.Compiler.Analyzer;
using BasicSharp.Compiler.Parser.Extensions;
using BasicSharp.Compiler.Parser.Syntaxes;
using BasicSharp.Compiler.ILEmitter.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;

namespace BasicSharp.Compiler.ILEmitter
{
    public class MethodEmitter : Emitter<MethodDeclaration>, ILocalIndexer
    {
        #region consts
        const string ACCESSOR_MODIFIER = "<ACCESSOR_MODIFIER>";
        const string PARAMETERS = "<PARAMETERS>";
        const string NAME = "<NAME>";
        const string METHOD_HEADER = @"
.method <ACCESSOR_MODIFIER> hidebysig static 
        void <NAME> (<PARAMETERS>
                      ) cil managed 
    {";

        const string LOCAL_SPECIFIER = "    [{0}] {1}";
        #endregion

        Dictionary<string, LocalInfo> indexByLocalInit = new Dictionary<string, LocalInfo>();

        public MethodEmitter(CompilationBag compilationBag)
            : base(compilationBag) { }

        public LocalInfo GetLocalInfo(string name)
        {
            if (indexByLocalInit.ContainsKey(name))
                return indexByLocalInit[name];
            
            return null;
        }

        public override void BuildString(StringBuilder builder, MethodDeclaration node)
        {
            var _params = node.ParameterList.Parameters.Select(p => string.Format("{0} {1}", p.Type.GetCLRType().GetMsilTypeName(), p.Identifier.StringValue));
            var methodHeader = METHOD_HEADER.Replace(NAME, node.Identifier.StringValue)
                                            .Replace(ACCESSOR_MODIFIER, node.IsPublic ? "public" : "private")
                                            .Replace(PARAMETERS, string.Join(",\n                     ", _params));

            builder.AppendLine(methodHeader);

            if (node.Identifier.StringValue.Equals("Main"))
                builder.AppendLine("		.entrypoint");

            var locals = node.Childs.OfType<VariableDeclaration>();
            writeLocals(builder, locals);
            builder.AppendLine();

            var blockTac = TacEmitterFactory.GenerateWithNode(node.Block, compilationBag, this, "IL_", 0);
            blockTac.Item2.Add(new TacUnit
                {
                    LabelPrefix = blockTac.Item2.GetNextLabel().Item1,
                    LabelIndex = blockTac.Item2.GetNextLabel().Item2,
                    Op = OpCodes.Ret
                });

            foreach (var item in blockTac.Item2)
                builder.AppendLine(item.ToString());

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
                var type = item.Type.GetCLRType();
                foreach (var decl in item.Declarators)
                {
                    localsInit.Add(string.Format(LOCAL_SPECIFIER, i++, type.GetMsilTypeName()));

                    var var = new Variable
                    {
                        ClrType = type,
                        Definition = decl,
                        Name = decl.Identifier.StringValue,
                        LocalInitIndex = i
                    };

                    indexByLocalInit.Add(decl.Identifier.StringValue, new LocalInfo { Index = i, Variable = var });
                }
            }

            builder.AppendLine();
            builder.Append(string.Join(",\n", localsInit));
            builder.AppendLine();

            builder.AppendLine(")");
        }


        
    }
}
