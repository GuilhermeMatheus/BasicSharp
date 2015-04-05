using BasicSharp.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicSharp.Compiler.Parser.Extensions;
using System.Collections.ObjectModel;
using System.Collections;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class BlockStatement : Statement
    {
        List<Statement> statements = new List<Statement>();

        public TokenInfo OpenBraceToken { get; internal set; }
        public ReadOnlyCollection<Statement> Statements
        {
            get { return statements.AsReadOnly(); }
        }
        public TokenInfo CloseBraceToken { get; internal set; }

        public void AddStatement(Statement statement)
        {
            statements.Add(statement);
        }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            yield return OpenBraceToken;

            foreach (var node in statements)
                foreach (var item in node.GetTokenEnumerable())
                    yield return item;
            
            yield return CloseBraceToken;
        }

        public override IEnumerable GetChilds()
        {
            yield return OpenBraceToken;
            yield return CloseBraceToken;
        }
    }
}
