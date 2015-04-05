using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicSharp.Compiler.Parser.Extensions;
using BasicSharp.Compiler.Lexer;
using BasicSharp.Compiler.Parser;
using System.Collections.ObjectModel;
using System.Collections;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class ArgumentList : SyntaxNode
    {
        List<Argument> arguments = new List<Argument>();
        
        public TokenInfo OpenParenToken { get; set; }
        public ReadOnlyCollection<Argument> Arguments
        {
            get { return arguments.AsReadOnly(); }
        }
        public TokenInfo CloseParenToken { get; set; }

        public void AddArgument(Argument argument)
        {
            arguments.Add(argument);
        }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            yield return OpenParenToken;

            foreach (var arg in arguments)
                foreach (var item in arg.GetTokenEnumerable())
                    yield return item;

            yield return CloseParenToken;
        }

        public override IEnumerable GetChilds()
        {
            yield return OpenParenToken;

            foreach (var arg in arguments)
                yield return arg;

            yield return CloseParenToken;
        }
    }
}
