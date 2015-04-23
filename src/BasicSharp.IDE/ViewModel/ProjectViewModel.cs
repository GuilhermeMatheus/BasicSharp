using BasicSharp.Compiler;
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
        public FileProject Project { get; set; }

        public ObservableCollection<string> AssembliesAddress
        {
            get
            {
                return Project.AssembliesAddress;
            }
            set
            {
                if (AssembliesAddress != value)
                {
                    Project.AssembliesAddress = value;
                    OnPropertyChanged();
                }
            }
        }
        public string SourceAddress
        {
            get
            {
                return Project.ProjectFileAddress;
            }
            set
            {
                Project.ProjectFileAddress = value;
            }
        }
        public string Name
        {
            get
            {
                return Project.Name;
            }
            set
            {
                Project.Name = value;
            }
        }

        public ProjectViewModel(FileProject project)
        {
            this.Project = project;
        }
    }
}
