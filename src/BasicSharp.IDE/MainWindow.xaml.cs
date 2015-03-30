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

namespace BasicSharp.IDE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            loadTextEditor();
        }

        void loadTextEditor()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var reader = assembly.GetManifestResourceStream("BasicSharp.IDE.Resources.Editor.HighlightingDefinitions.xshd");

            using (XmlReader r = XmlReader.Create(reader))
                textEditor.SyntaxHighlighting = HighlightingLoader.Load(r, HighlightingManager.Instance);
        }
    }
}
