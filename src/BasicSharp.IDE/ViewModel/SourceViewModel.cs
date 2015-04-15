using BasicSharp.Compiler;
using BasicSharp.Compiler.Analyzer;
using BasicSharp.Compiler.Lexer;
using BasicSharp.Compiler.Parser;
using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.IDE.ViewModel
{
    public enum Status
    {
        CompilationException,
        CompilationSuccess
    }

    public class SourceViewModel : ViewModelBase
    {
        FileProject project = null;

        private string source;
        public string Source
        {
            get { return source; }
            set
            {
                if (source == value)
                    return;

                source = value;
                OnPropertyChanged();

                updateTokenAndSyntax();
            }
        }

        public ObservableCollection<string> Errors { get; set; }

        private ObservableCollection<TokenInfo> tokens;
        public ObservableCollection<TokenInfo> Tokens
        {
            get { return tokens; }
            set
            {
                if (tokens == value) return; tokens = value;
                OnPropertyChanged();
            }
        }

        private List<SyntaxNode> syntax;
        public List<SyntaxNode> Syntax
        {
            get { return syntax; }
            set
            {
                if (syntax == value) 
                    return;
                
                syntax = value;
                OnPropertyChanged();
            }
        }

        private Status status;
        public Status Status
        {
            get { return status; }
            set
            {
                status = value;
                OnPropertyChanged();
            }
        }

        public SourceViewModel()
        {
            this.Tokens = new ObservableCollection<TokenInfo>();
            this.Errors = new ObservableCollection<string>();
        }

        public void SetSourceAndProject(string source, FileProject project)
        {
            this.Source = source;
            this.project = project;
        }

        void updateTokenAndSyntax()
        {
            Errors.Clear();

            var lexer = LexerFactory.FromString(Source);
            var tokens = lexer.GetTokens().ToList();
            var parser = new Parser(tokens.GetEnumerator());

            Tokens = new ObservableCollection<TokenInfo>(tokens);

            var parsedSyntax = parser.GetSyntax();
            Syntax = new List<SyntaxNode> { parsedSyntax };

            foreach (var item in lexer.LexicalErrors)
                Errors.Add(item.Message);
            
            foreach (var item in parser.SyntacticErrors)
                Errors.Add(item.Message);

            if (parsedSyntax as CompilationUnit == null)
                return;

            try
            {
                var analyzer = new AnalyzerManager(project, parsedSyntax as CompilationUnit);
                foreach (var item in analyzer.GetAnalysisForCompilationUnit())
                    Errors.Add(item.MessageResult);
            }
            catch { }

            this.Status = Errors.Count == 0 ? Status.CompilationSuccess : Status.CompilationException;
        }
    }
}