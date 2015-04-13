using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicSharp.Utils;

namespace BasicSharp.Compiler
{
    public class MemoryProject : Project
    {
        public string Source { get; set; }

        public override Stream GetSourceStream()
        {
            return Source.GetMemoryStream();
        }
    }
}
