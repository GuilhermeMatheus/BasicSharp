using BasicSharp.Compiler;
using BasicSharp.Compiler.Lexer;
using BasicSharp.Compiler.Parser.Syntaxes;
using BasicSharp.IDE.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.IDE.ViewModel
{
    public class SelectedItemChanged : EventArgs
    {
        public object Item { get; set; }
    }

    public class IdeViewModel : ViewModelBase
    {
        public event EventHandler<SelectedItemChanged> SelectedItemChanged = (s, e) => { };

        FileProject project;
        public FileProject Project
        {
            get
            {
                return project;
            }
            set
            {
                if (project != value)
                {
                    project = value;
                    OnPropertyChanged();
                }
            }
        }

        private SourceViewModel currentSource;
        public SourceViewModel CurrentSource
        {
            get { return currentSource; }
            set
            {
                if (currentSource == value)
                    return; 
                
                currentSource = value;
                OnPropertyChanged();
            }
        }

        private object selectedItem;
        public object SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                OnPropertyChanged();

                if (value is TokenInfo || value is SyntaxNode)
                    SelectedItemChanged(this, new SelectedItemChanged { Item = value });

            }
        }

        public IdeViewModel()
        {
            //this.CurrentSource = new SourceViewModel { FileName = "Module1.bs" };
        }


        #region Comandos
        public Command NovoProjetoCommand { get { return null; } }
        public Command AbrirProjetoCommand { get { return null; } }
        #endregion
    }
}
