using BasicSharp.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class FieldDeclaration : SyntaxNode
    {
        public TokenInfo Modifier { get; internal set; }

        public VariableDeclarator Declarator { get; internal set; }

        public TokenInfo SemiColon { get; internal set; }
    }
}
