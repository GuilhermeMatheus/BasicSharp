using System;
namespace BasicSharp.Compiler.ILEmitter
{
    public interface ILocalIndexer
    {
        int GetLocalIndex(string name);
    }
}
