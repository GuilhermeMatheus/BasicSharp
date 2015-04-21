using BasicSharp.Compiler.Analyzer;
using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.ILEmitter
{
    public abstract class ExpressionEmitter<T> : TacEmitter<T>, ITacEmitter
        where T : SyntaxNode
    {
        public ExpressionEmitter(CompilationBag compilationBag, ILocalIndexer localIndexer)
            : base(compilationBag, localIndexer) { }

        public Tuple<Type, List<TacUnit>> GenerateTypeTac(SyntaxNode node, string labelPrefix = "IL_", int index = 0)
        {
            return GenerateWithType(node as T, labelPrefix, index);
        }
        public abstract Tuple<Type, List<TacUnit>> GenerateWithType(T node, string labelPrefix = "IL_", int index = 0);

        public override List<TacUnit> Generate(T node, string labelPrefix = "IL_", int index = 0)
        {
            return GenerateWithType(node, labelPrefix, index).Item2;
        }
    }
}
