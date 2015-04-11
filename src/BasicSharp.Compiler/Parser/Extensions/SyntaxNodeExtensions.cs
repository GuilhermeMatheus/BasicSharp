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

        public static Type GetCLRType(this PredefinedType type)
        {
            var isArray = type.ArraySpecifier != null;
            
            switch (type.TypeToken.Kind)
            {
                case SyntaxKind.VoidKeyword:
                    return typeof(void);
                case SyntaxKind.IntKeyword:
                    return isArray ? typeof(int[]) : typeof(int);
                case SyntaxKind.ByteKeyword:
                    return isArray ? typeof(byte[]) : typeof(byte);
                case SyntaxKind.BoolKeyword:
                    return isArray ? typeof(bool[]) : typeof(bool);
                case SyntaxKind.DoubleKeyword:
                    return isArray ? typeof(double[]) : typeof(double);
                case SyntaxKind.StringKeyword:
                    return isArray ? typeof(string[]) : typeof(string);
                case SyntaxKind.CharKeyword:
                    return isArray ? typeof(char[]) : typeof(char);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
