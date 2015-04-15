using BasicSharp.IDE.Helpers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.IDE.ViewModel
{
    public class ProjectEditorViewModel : ViewModelBase
    {
        string selectedAssembly;
        public string SelectedAssembly
        {
            get { return selectedAssembly; }
            set
            {
                if (selectedAssembly != value)
                {
                    selectedAssembly = value;
                    OnPropertyChanged();
                }
            }
        }

        ProjectViewModel project;
        public ProjectViewModel Project
        {
            get { return project; }
            set
            {
                if (project != value)
                {
                    project = value;
                    OnPropertyChanged();
                }
            }
        }

        public Command AddReferenceCommand
        {
            get
            {

                return new Command {
                    ExecuteAction = _ =>
                    {
                        var opf = new OpenFileDialog
                            {
                                Filter = "Lybrary (.dll)|*.dll"
                            };

                        if (opf.ShowDialog() != true)
                            return;

                        Project.AssembliesAddress.Add(opf.FileName);
                        Project.OnPropertyChanged("");
                        OnPropertyChanged("");
                    }
                };
            }
        }
        public Command RemoveReferenceCommand
        {
            get
            {
                return new Command
                {
                    ExecuteAction = _ =>
                        {
                            Project.AssembliesAddress.Remove(this.SelectedAssembly);
                        }
                };
            }
        }
    }
}
