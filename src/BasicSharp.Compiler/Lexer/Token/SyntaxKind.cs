using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.Lexer
{
    public enum SyntaxKind
    {
        Namespace,
        Identifier,

        OpenBraceToken,
        CloseBraceToken,
        OpenBracketToken,
        CloseBracketToken,
        DotToken,
        CommaToken,
        MinusToken,
        PlusToken,
        EqualsToken,

        ModuleKeyword,
        IntKeyword,
        ByteKeyword,
        StringKeyword,
        EverybodyKeyword,
        MyKeyword,
        IfKeyword,
        ElseKeyword,
        TrueKeyword,
        FalseKeyword,
        ForKeyword,

        StringLiteral,
        CharLiteral,
        IntegerLiteral,
        DoubleLiteral,
        ByteLiteral,

        WhitespaceTrivia,
        SingleLineCommentTrivia,
    }
}
