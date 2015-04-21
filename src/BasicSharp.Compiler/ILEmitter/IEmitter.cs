using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Text;

namespace BasicSharp.Compiler.ILEmitter
{
    interface IEmitter<T>
     where T : SyntaxNode
    {
        void BuildString(StringBuilder builder, T node);
    }
}
