﻿using BasicSharp.Compiler.Analyzer;
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
        const string RET_TYPE = "<RET_TYPE>";
        const string METHOD_HEADER = @"
.method <ACCESSOR_MODIFIER> hidebysig static 
        <RET_TYPE> <NAME> (<PARAMETERS>
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
            indexByLocalInit.Clear();

            var _params = new List<string>();
            
            foreach (var item in node.ParameterList.Parameters)
            {
                var clrType = item.Type.GetCLRType();
                var type = clrType.GetMsilTypeName();
                var name = item.Identifier.StringValue;

                indexByLocalInit.Add(name, new LocalInfo
                {
                    IsArgument = true,
                    Variable = new Variable
                    {
                        Definition = item,
                        Name = name,
                        ClrType = clrType
                    }
                });

                _params.Add(string.Format("{0} {1}", type, name));
            }

            var retType = node.ReturnType.GetCLRType().GetMsilTypeName();

            var methodHeader = METHOD_HEADER.Replace(NAME, node.Identifier.StringValue)
                                            .Replace(ACCESSOR_MODIFIER, node.IsPublic ? "public" : "private")
                                            .Replace(PARAMETERS, string.Join(",\n                     ", _params))
                                            .Replace(RET_TYPE, retType);

            builder.AppendLine(methodHeader);

            if (node.Identifier.StringValue.Equals("Main"))
                builder.AppendLine("		.entrypoint");
            
            writeLocals(builder, node);
            builder.AppendLine();

            var blockTac = TacEmitterFactory.GenerateWithNode(node.Block, compilationBag, this, "IL_", 0);
            var label = blockTac.Item2.GetNextLabel() ?? new Tuple<string, int>("IL_", 0);
            var lastTac = blockTac.Item2.LastOrDefault();

            var blockReturns = lastTac != null && lastTac.Op == OpCodes.Ret;

            if (!blockReturns)
                blockTac.Item2.Add(new TacUnit
                    {
                        LabelPrefix = label.Item1,
                        LabelIndex = label.Item2,
                        Op = OpCodes.Ret
                    });

            foreach (var item in blockTac.Item2)
                builder.AppendLine(item.ToString());

            builder.AppendLine("}");
        }

        void writeLocals(StringBuilder builder, MethodDeclaration node)
        {
            var locals = node.FindAll().OfType<VariableDeclaration>();

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
                    localsInit.Add(string.Format(LOCAL_SPECIFIER, i, type.GetMsilTypeName()));

                    var var = new Variable
                    {
                        ClrType = type,
                        Definition = decl,
                        Name = decl.Identifier.StringValue,
                        LocalInitIndex = i
                    };

                    indexByLocalInit.Add(decl.Identifier.StringValue, new LocalInfo { Index = i++, Variable = var });
                }
            }

            builder.AppendLine();
            builder.Append(string.Join(",\n", localsInit));
            builder.AppendLine();

            builder.AppendLine(")");
        }
    }
}