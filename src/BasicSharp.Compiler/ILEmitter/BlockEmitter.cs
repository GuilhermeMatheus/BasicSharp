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
        public BlockEmitter(CompilationBag compilationBag, ILocalIndexer localIndexer)
            : base(compilationBag, localIndexer) { }



        public override List<TacUnit> Generate(BlockStatement node, string labelPrefix = "IL_", int index = 0)
        {
            throw null;   
        }
    }
}
