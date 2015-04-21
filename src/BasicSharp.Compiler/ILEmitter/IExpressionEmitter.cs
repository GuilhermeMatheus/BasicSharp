using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
namespace BasicSharp.Compiler.ILEmitter
{
    public interface ITacEmitter
    {
        Tuple<Type, List<TacUnit>> GenerateTypeTac(SyntaxNode node, string labelPrefix = "IL_", int index = 0);
    }
}
