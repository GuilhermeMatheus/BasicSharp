using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler
{
    public class Variable
    {
        public Type ClrType { get; set; }
        public string Name { get; set; }
        public int LocalInitIndex { get; set; }
        public SyntaxNode Definition { get; set; }
    }
}
