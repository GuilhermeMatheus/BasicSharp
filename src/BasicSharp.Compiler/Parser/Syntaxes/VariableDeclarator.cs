using BasicSharp.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class VariableDeclarator : SyntaxNode
    {
        public TokenInfo Identifier { get; set; }
        public AssignmentExpression Assignment { get; set; }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            yield return Identifier;
            if (Assignment != null)
                foreach (var item in Assignment.Tokens)
                    yield return item;
        }
    }
}
