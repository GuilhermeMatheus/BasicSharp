using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler
{
    public class MethodStubParameter
    {
        public string Name { get; internal set; }
        public Type Type { get; internal set; }
        public bool IsArray
        {
            get { return Type.IsArray; }
        }
    }
}
