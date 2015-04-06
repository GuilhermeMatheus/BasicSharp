using BasicSharp.Compiler.Lexer;
using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
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
        
        private SourceViewModel currentSource;
        public SourceViewModel CurrentSource
        {
            get { return currentSource; }
            set
            {
                if (currentSource == value)
                    return; currentSource = value;
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
            this.CurrentSource = new SourceViewModel { FileName = "Module1.bs" };
        }

    }
}
