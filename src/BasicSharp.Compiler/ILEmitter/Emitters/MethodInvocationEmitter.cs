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
    public class MethodInvocationEmitter : ExpressionEmitter<MethodInvocationExpression>
    {
        const string INTERNAL_CALL_VALUE = "{0} {1}::{2}({3})";
        const string EXTERNAL_CALL_VALUE = "{0} [{1}]{2}::{3}({4})";

        public MethodInvocationEmitter(CompilationBag compilationBag, ILocalIndexer localIndexer)
            : base(compilationBag, localIndexer) { }

        public override Tuple<Type, List<TacUnit>> GenerateWithType(MethodInvocationExpression node, string labelPrefix = "IL_", int index = 0)
        {
            Tuple<string, int> label = new Tuple<string, int>(labelPrefix, index);

            var result = new List<TacUnit>();
            var argsType = new List<Type>();

            foreach (var item in node.Arguments.Arguments)
            {
                var expressionEmitter = TacEmitterFactory.GetEmitterFor(item.Expression, compilationBag, localIndexer);
                var typeAndTac = expressionEmitter.GenerateTypeTac(item.Expression, label.Item1, label.Item2);
                
                argsType.Add(typeAndTac.Item1);
                result.AddRange(typeAndTac.Item2);

                label = result.GetNextLabel() ?? label;
            }

            var internalDef = compilationBag.GetInternalMethodDefinition(node);

            Type returnType;
            string value;
            var _params = string.Join(", ", from item in argsType select item.GetMsilTypeName());


            if (internalDef != null)
            {
                returnType = internalDef.ReturnType.GetCLRType();
                var msilType = returnType.GetMsilTypeName();
                var sourceType = compilationBag.CompilationUnit.Module.Name.StringValue;

                value = string.Format(INTERNAL_CALL_VALUE, msilType, sourceType, node.MethodName.StringValue, _params);
            }
            else
            {
                //Isto é seguro, por conta dos analisadores.
                var method = compilationBag.GetExternalMethodsFromSession(node, argsType).First();
                returnType = method.ReturnType;
                var msilType = returnType.GetMsilTypeName();
                var sourceAssembly = method.ReflectedType.Assembly.GetName().Name;
                var sourceType = method.ReflectedType.FullName;

                value = string.Format(EXTERNAL_CALL_VALUE, msilType, sourceAssembly, sourceType, node.MethodName.StringValue, _params);
            }

            var call = new TacUnit
            {
                LabelPrefix = label.Item1,
                LabelIndex = label.Item2,
                Op = OpCodes.Call,
                Value = value
            };

            result.Add(call);
            return new Tuple<Type,List<TacUnit>>(returnType, result);
        }
    }
}
