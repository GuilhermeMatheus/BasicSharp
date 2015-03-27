using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicSharp.Compiler.Parser.Extensions;
using BasicSharp.Compiler.Lexer;
using BasicSharp.Compiler.Parser;
using System.Collections.ObjectModel;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class ArgumentList : SyntaxNode
    {
        List<Argument> arguments = new List<Argument>();
        
        public TokenInfo OpenParen { get; set; }
        public ReadOnlyCollection<Argument> Arguments
        {
            get { return arguments.AsReadOnly(); }
        }
        public TokenInfo CloseParen { get; set; }

        public void AddArgument(Argument argument)
        {
            arguments.Add(argument);
        }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            yield return OpenParen;

            foreach (var arg in arguments)
                foreach (var item in arg.GetTokenEnumerable())
                    yield return item;

            yield return CloseParen;
        }
    }
}
