using BasicSharp.Compiler;
using BasicSharp.IDE.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace BasicSharp.IDE
{
    /// <summary>
    /// Interaction logic for ProjectDialog.xaml
    /// </summary>
    public partial class ProjectDialog : Window
    {
        ProjectEditorViewModel context;
        FileProject project;

        public ProjectDialog(FileProject project)
        {
            DataContextChanged += ProjectDialog_DataContextChanged;
            this.project = project;

            InitializeComponent();
        }

        #region Window Objects
        void ProjectDialog_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.context = e.NewValue as ProjectEditorViewModel;

            context.Project = context.Project ?? new ProjectViewModel(project ?? new FileProject());
        }
        #endregion

        public FileProject GetProject()
        {
            return context.Project.Project;
        }

        private void BtnSalvarClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
