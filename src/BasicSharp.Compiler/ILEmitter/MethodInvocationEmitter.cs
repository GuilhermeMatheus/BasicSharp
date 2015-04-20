using BasicSharp.Compiler.Analyzer;
using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicSharp.Compiler.ILEmitter.Extensions;
using System.Reflection.Emit;
using BasicSharp.Compiler.Parser.Extensions;

namespace BasicSharp.Compiler.ILEmitter
{
    public class MethodInvocationEmitter : TacEmitter<MethodInvocationExpression>
    {
        const string CALL_VALUE = "{0} {1}::{2}({3})";

        public MethodInvocationEmitter(CompilationBag compilationBag, ILocalIndexer localIndexer)
            : base(compilationBag, localIndexer) { }

        public override List<TacUnit> Generate(MethodInvocationExpression node, string labelPrefix = "IL_", int index = 0)
        {//IL_0008: call int32 C::i(string, char)
            Tuple<string, int> label = new Tuple<string, int>(labelPrefix, index);

            var expressionEmitter = new ExpressionEmitter(compilationBag, localIndexer);
            var result = new List<TacUnit>();

            foreach (var item in node.Arguments.Arguments)
            {
                result.AddRange(expressionEmitter.Generate(item.Expression, label.Item1, label.Item2));
                label = result.GetNextLabel() ?? label;
            }

            var internalDef = compilationBag.GetInternalMethodDefinition(node);

            string returnType, sourceType, _params, methodName = node.MethodName.StringValue;

            if (internalDef != null)
            {
                returnType = internalDef.ReturnType.GetCLRType().GetMsilTypeName();
                sourceType = compilationBag.CompilationUnit.Module.Name.StringValue;
                _params = string.Join(", ", from item in internalDef.ParameterList.Parameters
                                            select item.Type.GetCLRType().GetMsilTypeName());
            }
            else
            {
                //compilationBag.MethodStubs.Where(m => m.Name)

                _params = sourceType = returnType = null;
            }
            
            var call = new TacUnit
            {
                LabelPrefix = label.Item1,
                LabelIndex = label.Item2,
                Op = OpCodes.Call,
                Value = string.Format(CALL_VALUE, returnType, sourceType, methodName, _params)
            };

            result.Add(call);
            return result;
        }
    }
}
