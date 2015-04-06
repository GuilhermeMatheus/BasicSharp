using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public interface ISyntaxTreeNode
    {
        IEnumerable Childs { get; }
        string DisplayMember { get; }
    }
}
