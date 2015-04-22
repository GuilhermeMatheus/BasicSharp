using BasicSharp.Compiler.Analyzer;
using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicSharp.Compiler.Parser.Extensions;
using System.Reflection.Emit;
using BasicSharp.Compiler.ILEmitter.Extensions;

namespace BasicSharp.Compiler.ILEmitter
{
    public class LocalVariableDeclarationStatementEmitter : ExpressionEmitter<LocalVariableDeclarationStatement>
    {
        public LocalVariableDeclarationStatementEmitter(CompilationBag compilationBag, ILocalIndexer localIndexer)
            : base(compilationBag, localIndexer) { }

        public override Tuple<Type, List<TacUnit>> GenerateWithType(LocalVariableDeclarationStatement node, string labelPrefix = "IL_", int index = 0)
        {
            var resultType = node.Declaration.Type.GetCLRType();
            var result = new List<TacUnit>();

            var assignablesVariables = node.Declaration.Declarators.Where(d => d.Assignment != null);
            foreach (var item in assignablesVariables)
            {
                var exprTac = TacEmitterFactory.GenerateWithNode(item.Assignment.Expression, compilationBag, localIndexer, labelPrefix, index);
                result.AddRange(exprTac.Item2);

                var target = localIndexer.GetLocalInfo(item.Identifier.StringValue);
                var label = result.GetNextLabel();

                result.Add(new TacUnit
                    {
                        LabelPrefix = label.Item1,
                        LabelIndex = label.Item2,
                        Op = OpCodes.Stloc,
                        Value = target.Index.ToString()
                    });
            }

            return new Tuple<Type, List<TacUnit>>(resultType, result);
        }
    }
}
