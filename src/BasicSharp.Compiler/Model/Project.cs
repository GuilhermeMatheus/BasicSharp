using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler
{
    public abstract class Project
    {
        public string Name { get; set; }
        public List<string> AssembliesAddress { get; set; }
        
        public abstract Stream GetSourceStream();
    }
}
