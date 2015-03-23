using BasicSharp.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class MethodDeclaration : SyntaxNode
    {
        public TokenInfo Modifier { get; internal set; }
        public TokenInfo ReturnType { get; internal set; }
        public TokenInfo Identifier { get; internal set; }

        public ParameterList ParameterList { get; internal set; }

        public Block Block { get; internal set; }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            yield return Modifier;
            yield return ReturnType;
            yield return Identifier;
            if (ParameterList != null)
                foreach (var item in ParameterList.GetInternalTokens())
                    yield return item;
        }
    }
}
