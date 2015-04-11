using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.IDE.ViewModel
{
    public class ProjectViewModel : ViewModelBase 
    {
        public ObservableCollection<string> AssembliesAddress { get; set; }
        public string SourceAddress { get; set; }
        public string Name { get; set; }
    }
}
