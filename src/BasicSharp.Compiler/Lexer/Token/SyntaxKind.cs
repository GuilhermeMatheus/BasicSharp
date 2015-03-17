using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.Lexer
{
    public enum SyntaxKind
    {
        None = 0,

        Identifier,

        OpenBracketToken,
        CloseBracketToken,
        OpenBraceToken,
        CloseBraceToken,
        OpenParenToken,
        CloseParenToken,
        DotToken,
        SemicolonToken,
        CommaToken,

        EqualsToken,
        EqualsEqualsToken,
        MinorToken,
        MinorEqualsToken,
        MajorToken,
        MajorEqualsToken,
        ModToken,

        PlusEqualsToken,
        PlusToken,
        MinusEqualsToken,
        MinusToken,
        AsteriskToken,
        AsteriskEqualsToken,
        SlashToken,
        SlashEqualsToken,

        ModuleKeyword,
        ImplementsDirectiveKeyword,

        NullKeyword,
        VoidKeyword,
        IntKeyword,
        DoubleKeyword,
        ByteKeyword,
        StringKeyword,
        CharKeyword,
        VarKeyword,

        EverybodyKeyword,
        MyKeyword,
        ReturnKeyword,

        IfKeyword,
        ElseKeyword,
        TrueKeyword,
        FalseKeyword,

        WhileKeyword,
        ForKeyword,
        BreakKeyword,

        StringLiteral,
        CharLiteral,
        IntegerLiteral,
        DoubleLiteral,
        ByteLiteral,

        EndOfLineTrivia,
        WhitespaceTrivia,
        TabTrivia,
        SingleLineCommentTrivia,
    }
}
