using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler
{
    public abstract class Project
    {
        public string Name { get; set; }
        public ObservableCollection<string> AssembliesAddress { get; set; }
        
        public abstract Stream GetSourceStream();

        public Project()
        {
            this.AssembliesAddress = new ObservableCollection<string>();
        }
    }
}
