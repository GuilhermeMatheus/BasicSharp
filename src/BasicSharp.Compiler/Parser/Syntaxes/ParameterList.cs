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

        public TokenInfo OpenParenToken { get; internal set; }

        public ReadOnlyCollection<Parameter> Parameters
        {
            get { return new ReadOnlyCollection<Parameter>(parameters); }
        }

        public TokenInfo CloseParenToken { get; internal set; }

        public void AddParameter(Parameter parameter)
        {
            this.parameters.Add(parameter);
        }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            yield return OpenParenToken;

            foreach (var param in parameters)
                foreach (var item in param.GetInternalTokens())
                    yield return item;

            yield return CloseParenToken;
        }
    }
}
