using System;

namespace BasicSharp.Compiler.ILEmitter
{
    public class LocalInfo
    {
        public int Index { get; internal set; }
        public Variable Variable { get; internal set; }
    }

    public interface ILocalIndexer
    {
        LocalInfo GetLocalInfo(string name);
    }
}
