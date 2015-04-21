using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
namespace BasicSharp.Compiler.ILEmitter
{
    public interface IExpressionEmitter
    {
        Tuple<Type, List<TacUnit>> GenerateWithType(Expression node, string labelPrefix = "IL_", int index = 0);
    }
}
