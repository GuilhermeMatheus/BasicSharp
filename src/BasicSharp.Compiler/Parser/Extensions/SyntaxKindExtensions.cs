using BasicSharp.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.Parser.Extensions
{
    public static class SyntaxKindExtensions
    {
        public static bool IsModifier(this SyntaxKind syntaxKind)
        {
            return syntaxKind == SyntaxKind.EverybodyKeyword ||
                   syntaxKind == SyntaxKind.MyKeyword;
        }

        public static bool IsTypeNotContextual(this SyntaxKind syntaxKind)
        {
            switch (syntaxKind)
            {
                case SyntaxKind.VoidKeyword:
                case SyntaxKind.BoolKeyword:
                case SyntaxKind.IntKeyword:
                case SyntaxKind.DoubleKeyword:
                case SyntaxKind.ByteKeyword:
                case SyntaxKind.StringKeyword:
                case SyntaxKind.CharKeyword:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsTrivia(this SyntaxKind syntaxKind)
        {
            switch (syntaxKind)
            {
                case SyntaxKind.TabTrivia:
                case SyntaxKind.WhitespaceTrivia:
                case SyntaxKind.EndOfLineTrivia:
                case SyntaxKind.SingleLineCommentTrivia:
                    return true;
                default:
                    return false;
            }
        }
    }
}
