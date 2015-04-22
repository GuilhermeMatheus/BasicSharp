using BasicSharp.Compiler;
using BasicSharp.Compiler.Analyzer;
using BasicSharp.Compiler.ILEmitter;
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

        private string msil;
        public string MSIL
        {
            get { return msil; }
            set
            {
                msil = value;
                OnPropertyChanged();
            }
        }

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

                updateAll();
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

        void updateAll()
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

            CompilationBag bag = null;
            var compilationUnit = parsedSyntax as CompilationUnit;
            
            try
            {
                bag = new CompilationBag(project, compilationUnit);
                var analyzer = bag.Analyzer;
                foreach (var item in analyzer.GetAnalysisForCompilationUnit())
                    Errors.Add(item.MessageResult);
            }
            catch { }

            if (Errors.Count == 0)
            {
                this.Status = Status.CompilationSuccess;
                if (bag != null)
                    this.MSIL = new CodeGenerator(bag, compilationUnit).Translate();
                else
                    this.MSIL = string.Empty;
            }
            else
            {
                this.Status = Status.CompilationException;
                this.MSIL = string.Empty;
            }
        }
    }
}