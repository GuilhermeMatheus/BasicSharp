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

namespace BasicSharp.IDE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.DataContextChanged += MainWindow_DataContextChanged;
            this.Loaded += MainWindow_Loaded;
            
            InitializeComponent();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            loadTextEditor();
        }

        void MainWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var context = e.NewValue as IdeViewModel;
            if (context == null)
                return;

            context.SelectedItemChanged += context_SelectedItemChanged;
        }

        void context_SelectedItemChanged(object sender, SelectedItemChanged e)
        {
            if (textEditor.IsFocused)
                return;

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

        void loadTextEditor()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var reader = assembly.GetManifestResourceStream("BasicSharp.IDE.Resources.Editor.HighlightingDefinitions.xshd");

            using (XmlReader r = XmlReader.Create(reader))
                textEditor.SyntaxHighlighting = HighlightingLoader.Load(r, HighlightingManager.Instance);
        }
    }
}
