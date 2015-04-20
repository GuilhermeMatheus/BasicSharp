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
        public string PreferredFirstLabel { get; private set; }

        public TacEmitter(CompilationBag compilationBag)
            : base(compilationBag) { }

        public void SetPreferredFirstLabel(string label)
        {
            this.PreferredFirstLabel = label;
        }
        
        public abstract List<TacUnit> Generate(T node, string labelPrefix = "IL_", int index = 0);

        public override void BuildString(StringBuilder builder, T node)
        {
            foreach (var item in Generate(node))
                builder.AppendLine(item.ToString());
        }
    }
}
