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
        public string FileAddress { get; set; }

        private string fileName;
        public string FileName
        {
            get { return fileName; }
            set
            {
                if (fileName == value)
                    return; fileName = value;
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

                updateTokenAndSyntax();
            }
        }

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
        }

        void updateTokenAndSyntax()
        {
            var lexer = LexerFactory.FromString(Source);
            var tokens = lexer.GetTokens().ToList();
            var parser = new Parser(tokens.GetEnumerator());

            Tokens = new ObservableCollection<TokenInfo>(tokens);

            try
            {
                Syntax = new List<SyntaxNode> { parser.GetSyntax() };
                this.Status = Status.CompilationSuccess;
            }
            catch 
            {
                this.Status = Status.CompilationException;
            }
        }
    }
}