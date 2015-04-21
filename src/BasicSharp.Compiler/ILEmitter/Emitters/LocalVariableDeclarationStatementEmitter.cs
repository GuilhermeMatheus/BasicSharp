using BasicSharp.Compiler.Analyzer;
using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicSharp.Compiler.ILEmitter
{
    public class LocalVariableDeclarationStatementEmitter : ExpressionEmitter<LocalVariableDeclarationStatement>
    {
        public LocalVariableDeclarationStatementEmitter(CompilationBag compilationBag, ILocalIndexer localIndexer)
            : base(compilationBag, localIndexer) { }

        public override Tuple<Type, List<TacUnit>> GenerateWithType(LocalVariableDeclarationStatement node, string labelPrefix = "IL_", int index = 0)
        {
            throw new NotImplementedException();
        }
    }
}
