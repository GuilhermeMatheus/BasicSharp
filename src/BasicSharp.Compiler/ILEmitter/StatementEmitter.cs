using BasicSharp.Compiler.Analyzer;
using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicSharp.Compiler.ILEmitter
{
    public class StatementEmitter : TacEmitter<Statement>
    {
        public StatementEmitter(CompilationBag compilationBag, ILocalIndexer localIndexer)
            : base(compilationBag, localIndexer) { }

        IfEmitter _ifEmitter;
        Emitter<IfStatement> ifEmitter
        {
            get { return _ifEmitter ?? (_ifEmitter = new IfEmitter(compilationBag, localIndexer)); }
        }
        
        WhileEmitter _whileEmitter;
        Emitter<WhileStatement> whileEmitter
        {
            get { return _whileEmitter ?? (_whileEmitter = new WhileEmitter(compilationBag, localIndexer)); }
        }

        ForEmitter _forEmitter;
        Emitter<ForStatement> forEmitter
        {
            get { return _forEmitter ?? (_forEmitter = new ForEmitter(compilationBag, localIndexer)); }
        }

        public override void BuildString(StringBuilder builder, Statement node)
        {
            if (node is IfStatement)
                ifEmitter.BuildString(builder, node as IfStatement);
            else if (node is WhileStatement)
                whileEmitter.BuildString(builder, node as WhileStatement);
            else if (node is ForStatement)
                forEmitter.BuildString(builder, node as ForStatement);

            throw new NotImplementedException();
        }

        public override List<TacUnit> Generate(Statement node, string labelPrefix = "IL_", int index = 0)
        {
            throw new NotImplementedException();
        }
    }
}
