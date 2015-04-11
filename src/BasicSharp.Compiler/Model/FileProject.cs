using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler
{
    public class FileProject : Project
    {
        public string SourceFileAddress { get; set; }

        public override Stream GetSourceStream()
        {
            if (SourceFileAddress == null)
                return null;

            return new FileStream(SourceFileAddress, FileMode.Open);
        }
    }
}