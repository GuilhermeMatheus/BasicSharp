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
        public string ProjectFileAddress { get; set; }

        public override Stream GetSourceStream()
        {
            if (ProjectFileAddress == null)
                return null;
            
            var sourcePath = System.IO.Path.GetDirectoryName(ProjectFileAddress) + "\\Source.bs";
            return new FileStream(sourcePath , FileMode.Open);
        }
    }
}