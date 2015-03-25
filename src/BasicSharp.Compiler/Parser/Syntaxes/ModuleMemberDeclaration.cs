using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public abstract class ModuleMemberDeclaration : SyntaxNode
    {
        public abstract bool IsPublic { get; }
    }
}
