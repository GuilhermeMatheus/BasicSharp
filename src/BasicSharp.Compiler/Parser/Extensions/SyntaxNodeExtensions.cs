using BasicSharp.Compiler.Lexer;
using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.Parser.Extensions
{
    public static class SyntaxNodeExtensions
    {
        public static IEnumerable<TokenInfo> GetTokenEnumerable(this SyntaxNode syntax)
        {
            if (syntax == null)
                return Enumerable.Empty<TokenInfo>();

            return syntax.Tokens;
        }

    }
}
