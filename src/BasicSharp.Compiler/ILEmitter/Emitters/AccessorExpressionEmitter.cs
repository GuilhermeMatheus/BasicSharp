using BasicSharp.Compiler.Analyzer;
using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using BasicSharp.Compiler.ILEmitter.Extensions;

namespace BasicSharp.Compiler.ILEmitter
{
    public class AccessorExpressionEmitter : ExpressionEmitter<AccessorExpression>
    {
        //IL_0001: ldsfld bool C::b - Static Field
        //IL_0001: ldloc.1          - Local Variable
        const string LOAD_FIELD_FORMAT = "{0} {1}::{2}";

        public AccessorExpressionEmitter(CompilationBag compilationBag, ILocalIndexer localIndexer)
            : base(compilationBag, localIndexer) { }

        public override Tuple<Type, List<TacUnit>> GenerateWithType(AccessorExpression node, string labelPrefix = "IL_", int index = 0)
        {
            Type typeResult;
            var result = new List<TacUnit>();

            var name = node.Identifier.StringValue;
            var localInfo = localIndexer.GetLocalInfo(name);

            var tac = new TacUnit
                {
                    LabelPrefix = labelPrefix,
                    LabelIndex = index++,
                };

            if (localInfo != null)
            {
                typeResult = localInfo.Variable.ClrType;
                tac.Op = OpCodes.Ldloc;
                tac.Value = localInfo.Index.ToString();
            }
            else
            {
                var var = compilationBag.GetField(name);
                var memberName = compilationBag.CompilationUnit.Module.Name.StringValue;
                typeResult = var.ClrType;

                tac.Op = OpCodes.Ldsfld;
                tac.Value = string.Format(LOAD_FIELD_FORMAT, typeResult.GetMsilTypeName(), memberName, name);
            }
            
            result.Add(tac);
            return new Tuple<Type, List<TacUnit>>(typeResult, result);
        }
    }
}
