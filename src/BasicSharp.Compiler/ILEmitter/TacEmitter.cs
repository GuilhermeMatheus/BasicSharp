using BasicSharp.Compiler.Analyzer;
using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.ILEmitter
{
    public abstract class TacEmitter<T> : Emitter<T>
        where T : SyntaxNode
    {
        protected ILocalIndexer localIndexer;

        public TacEmitter(CompilationBag compilationBag, ILocalIndexer localIndexer)
            : base(compilationBag)
        {
            this.localIndexer = localIndexer;
        }

        public abstract List<TacUnit> Generate(T node, string labelPrefix = "IL_", int index = 0);

        public override void BuildString(StringBuilder builder, T node)
        {
            foreach (var item in Generate(node))
                builder.AppendLine(item.ToString());
        }

        protected string GetLabel(string prefix, int index)
        {
            return prefix + index.ToString().PadLeft(4, '0');
        }
    }
}
