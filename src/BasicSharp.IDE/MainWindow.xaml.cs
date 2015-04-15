using BasicSharp.Compiler.Lexer;
using BasicSharp.Compiler.Parser.Syntaxes;
using BasicSharp.IDE.ViewModel;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using BasicSharp.Compiler.Parser.Extensions;
using System.Xml.Serialization;
using System.IO;
using BasicSharp.Compiler;
using Microsoft.Win32;

namespace BasicSharp.IDE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IdeViewModel context;

        public MainWindow()
        {
            this.DataContextChanged += MainWindow_DataContextChanged;
            this.Loaded += MainWindow_Loaded;
            
            InitializeComponent();
            treeView.SelectedItemChanged += treeView_SelectedItemChanged;
        }

        void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            blockItemChanged = FocusManager.GetFocusedElement(this).GetType() != typeof(TreeViewItem);
        }
        bool blockItemChanged = false;

        #region Window Objects
        void loadTextEditor()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var reader = assembly.GetManifestResourceStream("BasicSharp.IDE.Resources.Editor.HighlightingDefinitions.xshd");

            using (XmlReader r = XmlReader.Create(reader))
                textEditor.SyntaxHighlighting = HighlightingLoader.Load(r, HighlightingManager.Instance);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            loadTextEditor();
        }
        void MainWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            context = e.NewValue as IdeViewModel;
            if (context == null)
                return;

            context.SelectedItemChanged += context_SelectedItemChanged;
        }

        void context_SelectedItemChanged(object sender, SelectedItemChanged e)
        {
            if (blockItemChanged)
            {
                blockItemChanged = false;
                return;
            }

            var syntaxNode = e.Item as SyntaxNode;
            if (syntaxNode != null)
            {
                var col = syntaxNode.Tokens.Where(x => !x.Kind.IsTrivia());
                var selectionStart = col.First().Begin;
                var length = col.Last().End - selectionStart;
                textEditor.Select(selectionStart, length);
                return;
            }

            var token = e.Item as TokenInfo;
            if (token != null)
            {
                var selectionStart = token.Begin;
                var length = token.End - selectionStart;
                textEditor.Select(selectionStart, length);
                return;
            }
        }
        #endregion

        private void BtnCriarProjeto(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog { Filter = "Project (.xml)|*.xml" };
            
            if (sfd.ShowDialog() != true)
                return;
            
            var project = new FileProject { ProjectFileAddress = sfd.FileName };
            var dialog = new ProjectDialog(project);

            dialog.ShowDialog();

            this.context.Project = project;
            this.context.CurrentSource = new SourceViewModel();
            this.context.CurrentSource.SetSourceAndProject("", project);
        }

        private void BtnAbrirProjeto(object sender, RoutedEventArgs e)
        {
            var opf = new OpenFileDialog { Filter = "Project (.xml)|*.xml" };

            if (opf.ShowDialog() != true)
                return;

            var project = loadProject(opf.FileName);
            this.context.Project = project;
            this.context.CurrentSource = loadSource(project);
        }

        private void BtnSalvarProjeto(object sender, RoutedEventArgs e)
        {
            saveProject(context.Project);
        }

        private void BtnPropriedadesProjeto(object sender, RoutedEventArgs e)
        {
            var dialog = new ProjectDialog(context.Project);

            dialog.ShowDialog();
        }

        FileProject loadProject(string XMLString)
        {
            var oXmlSerializer = new XmlSerializer(typeof(FileProject));
            var project = oXmlSerializer.Deserialize(new StreamReader(XMLString)) as FileProject;
            
            return project;
        }
        SourceViewModel loadSource(FileProject project)
        {
            var sourcePath = System.IO.Path.GetDirectoryName(project.ProjectFileAddress) + "\\Source.bs";
            var source = File.ReadAllText(sourcePath);
            var result = new SourceViewModel();
            
            result.SetSourceAndProject(source, project);

            return result;
        }
        void saveProject(FileProject project)
        {
            var xmlDoc = new XmlDocument();
            var xmlSerializer = new XmlSerializer(typeof(FileProject));

            using (var xmlStream = new FileStream(project.ProjectFileAddress, FileMode.Create, FileAccess.Write))
                xmlSerializer.Serialize(xmlStream, project);
            
            var sourcePath = System.IO.Path.GetDirectoryName(project.ProjectFileAddress) + "\\Source.bs";
            File.WriteAllText(sourcePath, context.CurrentSource.Source);
        }

    }
}