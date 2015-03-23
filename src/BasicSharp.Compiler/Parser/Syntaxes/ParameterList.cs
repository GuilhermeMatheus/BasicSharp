using BasicSharp.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class ParameterList : SyntaxNode
    {
        List<Parameter> parameters = new List<Parameter>();
        List<TokenInfo> commas = new List<TokenInfo>();

        public TokenInfo OpenParenToken { get; private set; }

        public ReadOnlyCollection<Parameter> Parameters
        {
            get { return new ReadOnlyCollection<Parameter>(parameters); }
        }

        public ReadOnlyCollection<TokenInfo> Commas
        {
            get { return new ReadOnlyCollection<TokenInfo>(commas); }
        }

        public TokenInfo CloseParenToken { get; private set; }

        public void AddParameter(Parameter parameter)
        {
            this.parameters.Add(parameter);
        }

        public void AddComma(TokenInfo commaToken)
        {
            commas.Add(commaToken);
        }
    }
}
