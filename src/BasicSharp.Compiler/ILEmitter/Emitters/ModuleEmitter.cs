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
    public class ModuleEmitter : Emitter<ModuleDeclaration>
    {
        #region consts
        const string MODULE_NAME = "<MODULE_NAME>";
        const string CLASS_HEADER = @"
.class public auto ansi abstract sealed beforefieldinit <MODULE_NAME>
    extends [mscorlib]System.Object {";

        const string CTOR_HEADER = @"
    .method private hidebysig specialname rtspecialname static 
    void .cctor () cil managed {";

        #endregion

        public ModuleEmitter(CompilationBag compilationBag)
            : base(compilationBag) { }

        public override void BuildString(StringBuilder builder, ModuleDeclaration node)
        {
            var classHeader = CLASS_HEADER.Replace(MODULE_NAME, node.Name.StringValue);
            builder.AppendLine(classHeader);

            var fieldEmitter = new FieldEmitter(compilationBag);
            var fields = node.Members.Where(m => m.GetType() == typeof(FieldDeclaration))
                                     .Cast<FieldDeclaration>();

            builder.AppendLine();

            foreach (var item in fields)
                fieldEmitter.BuildString(builder, item);

            builder.AppendLine();

            buildConstructor(builder, fields, node.Name.StringValue);

            var methods = node.Members.Where(m => m.GetType() == typeof(MethodDeclaration))
                                      .Cast<MethodDeclaration>();
            
            var methodEmitter = new MethodEmitter(compilationBag);
            foreach (var item in methods)
                methodEmitter.BuildString(builder, item);

            builder.AppendLine("}"); //CloseBrace de ClassHeader
        }

        void buildConstructor(StringBuilder builder, IEnumerable<FieldDeclaration> fields, string className)
        {
            var hasDeclrAssignment = fields.SelectMany(f => f.Declaration.Declarators)
                                           .Where(d => d.Assignment != null)
                                           .Any();

            if (!hasDeclrAssignment)
                return;

            builder.AppendLine(CTOR_HEADER);

            var i = 0;
            foreach (var item in fields)
            {
                var declarators = item.Declaration.Declarators.Where(d => d.Assignment != null);
                if (!declarators.Any())
                    continue;

                var type = item.Declaration.Type.GetCLRType();
                var load_opcode = type.GetOpCodeToPushValue();
                var msil_typeName = type.GetMsilTypeName();
                var stsfld_opcode = OpCodes.Stsfld;

                foreach (var d in declarators)
                {
                    var load_value = Convert.ChangeType(d.Assignment.GetLiteralExpressionValue(), type);
                    
                    if (type == typeof(bool))
                        load_value = ((bool)load_value) ? "1" : "0";

                    var load_TAC = new TacUnit
                    {
                        LabelPrefix = "IL_",
                        LabelIndex = ++i,
                        Op = load_opcode,
                        Value = load_value.ToString()
                    };

                    builder.AppendLine(load_TAC.ToString());

                    var stsfld_value = string.Format("{0} {1}::{2}", msil_typeName, className, d.Identifier.StringValue);
                    var stsfld_TAC = new TacUnit
                    {
                        LabelPrefix = "IL_",
                        LabelIndex = ++i,
                        Op = stsfld_opcode,
                        Value = stsfld_value
                    };
                    builder.AppendLine(stsfld_TAC.ToString());
                }
            }

            var ret = new TacUnit
            {
                LabelPrefix = "IL_",
                LabelIndex = ++i,
                Op = OpCodes.Ret,
            };
            builder.AppendLine(ret.ToString());

            builder.AppendLine("}"); //CloseBrace de CtorHeader
        }

    }
}
