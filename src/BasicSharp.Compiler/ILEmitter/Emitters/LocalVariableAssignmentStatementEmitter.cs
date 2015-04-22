using BasicSharp.Compiler.Analyzer;
using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicSharp.Compiler.ILEmitter.Extensions;
using BasicSharp.Compiler.Lexer;
using System.Reflection.Emit;

namespace BasicSharp.Compiler.ILEmitter
{
    public class LocalVariableAssignmentStatementEmitter : ExpressionEmitter<LocalVariableAssignmentStatement>
    {
        public LocalVariableAssignmentStatementEmitter(CompilationBag compilationBag, ILocalIndexer localIndexer)
            : base(compilationBag, localIndexer) { }

        public override Tuple<Type, List<TacUnit>> GenerateWithType(LocalVariableAssignmentStatement node, string labelPrefix = "IL_", int index = 0)
        {
            return TacEmitterFactory.GenerateWithNode(node.Declarator, compilationBag, localIndexer, labelPrefix, index);
        }
    }
}
