using BasicSharp.Compiler.Analyzer;
using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicSharp.Compiler.ILEmitter
{
    public class BlockEmitter : TacEmitter<BlockStatement>
    {
        public BlockEmitter(CompilationBag compilationBag)
            : base(compilationBag) { }



        public override List<TacUnit> Generate(BlockStatement node, string labelPrefix = "IL_", int index = 0)
        {
            throw null;   
        }
    }

    public class ExpressionEmitter : TacEmitter<Expression>
    {
        public ExpressionEmitter(CompilationBag compilationBag)
            : base(compilationBag) { }

        public override List<TacUnit> Generate(Expression node, string labelPrefix = "IL_", int index = 0)
        {
            var result = new List<TacUnit>();


            return result;
        }
    }

}
