﻿using BasicSharp.Compiler.Analyzer;
using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace BasicSharp.Compiler.ILEmitter
{
    public class IfEmitter : TacEmitter<IfStatement> 
    {
        public IfEmitter(CompilationBag compilationBag, ILocalIndexer localIndexer)
            : base(compilationBag, localIndexer) { }
        
        public override List<TacUnit> Generate(IfStatement node, string labelPrefix = "IL_", int index = 0)
        {
            var rand = new Random();
            var result = new List<TacUnit>();

            var conditionEmitter = new ExpressionEmitter(compilationBag, localIndexer);
            var condition = conditionEmitter.Generate(node.Condition, labelPrefix, index);
            
            result.AddRange(condition);
            index = condition.Last().LabelIndex + 1;

            var brF = new TacUnit
            {
                LabelPrefix = labelPrefix,
                LabelIndex = index,
                Op = OpCodes.Brfalse
            };

            var statementEmitter = new StatementEmitter(compilationBag, localIndexer);
            var then = statementEmitter.Generate(node.Then, index: index);

            var thenLastTac = then.Last();
            brF.Value = GetLabel("IL_", thenLastTac.LabelIndex + 1);

            result.Add(brF);
            result.AddRange(then);

            if (node.Else != null)
            {
                var _else = statementEmitter.Generate(node.Else, index: brF.LabelIndex + 1);
                result.AddRange(_else);
            }

            return result;
        }
    }
}
