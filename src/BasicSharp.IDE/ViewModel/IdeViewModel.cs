using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.IDE.ViewModel
{
    public class IdeViewModel : ViewModelBase
    {
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

        public IdeViewModel()
        {
            this.CurrentSource = new SourceViewModel { FileName = "Module1.bs" };
        }

    }
}
