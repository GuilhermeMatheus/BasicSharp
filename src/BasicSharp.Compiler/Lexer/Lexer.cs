using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicSharp.Utils;

namespace BasicSharp.Compiler.Lexer
{
    public class Lexer
    {
        SlidingText text;

        public Lexer(Stream sourceStream) :
            this(new SlidingText(sourceStream)) { }

        public Lexer(SlidingText text)
        {
            this.text = text;
        }

        public IEnumerable<TokenInfo> GetTokens()
        {
            while (true)
            {
                var ch = text.Peek();

                switch (ch)
                {
                    case '"':
                        yield return scanStringLiteral();
                        continue;
                    case '\'':
                        yield return scanCharLiteral();
                        continue;
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        yield return scanNumericLiteral();
                        continue;
                    case 'a':
                    case 'b':
                    case 'c':
                    case 'd':
                    case 'e':
                    case 'f':
                    case 'g':
                    case 'h':
                    case 'i':
                    case 'j':
                    case 'k':
                    case 'l':
                    case 'm':
                    case 'n':
                    case 'o':
                    case 'p':
                    case 'q':
                    case 'r':
                    case 's':
                    case 't':
                    case 'u':
                    case 'v':
                    case 'w':
                    case 'x':
                    case 'y':
                    case 'z':
                    case 'A':
                    case 'B':
                    case 'C':
                    case 'D':
                    case 'E':
                    case 'F':
                    case 'G':
                    case 'H':
                    case 'I':
                    case 'J':
                    case 'K':
                    case 'L':
                    case 'M':
                    case 'N':
                    case 'O':
                    case 'P':
                    case 'Q':
                    case 'R':
                    case 'S':
                    case 'T':
                    case 'U':
                    case 'V':
                    case 'W':
                    case 'X':
                    case 'Y':
                    case 'Z':
                        yield return scanIdentifierOrKeyword();
                        continue;
                    case '/':
                    case '\n':
                    case '\r':
                    case ' ':
                        yield return scanTrivia();
                        continue;
                    case '=':
                    case '<':
                    case '>':
                    case '%':
                    case '+':
                    case '-':
                    case '*':
                    case '\\':
                        yield return scanAssignmentOrRelationalOperator();
                        continue;
                    case '[':
                    case ']':
                    case '(':
                    case ')':
                    case '{':
                    case '}':
                    case ',':
                    case '.':
                    case ';':
                        yield return new TokenInfo
                        {
                            Begin = text.Position,
                            End = text.Position + 1,
                            StringValue = text.Peek().ToString(),
                            Kind = getSyntaxKind(text.Next())
                        };
                        continue;
                    case SlidingText.INVALID_CHAR:
                    default:
                        yield break;
                }
            }
        }

        SyntaxKind getSyntaxKind(char c)
        {
            switch (c)
            {
                case '[':
                    return SyntaxKind.OpenBracketToken;
                case ']':
                    return SyntaxKind.CloseBracketToken;
                case '(':
                    return SyntaxKind.OpenParenToken;
                case ')':
                    return SyntaxKind.CloseParenToken;
                case '{':
                    return SyntaxKind.OpenBraceToken;
                case '}':
                    return SyntaxKind.CloseBraceToken;
                case ',':
                    return SyntaxKind.CommaToken;
                case '.':
                    return SyntaxKind.DotToken;
                case ';':
                    return SyntaxKind.SemicolonToken;
                default:
                    return SyntaxKind.None;
            }

        }

        TokenInfo scanAssignmentOrRelationalOperator()
        {
            var ret = new TokenInfo { Begin = text.Position, Kind = SyntaxKind.None };
            var stringValue = string.Empty;

            if (text.AdvanceIfMatches(stringValue = "=="))
                ret.Kind = SyntaxKind.EqualsEqualsToken;
            else if (text.AdvanceIfMatches(stringValue = "="))
                ret.Kind = SyntaxKind.EqualsToken;
            else if (text.AdvanceIfMatches(stringValue = "<="))
                ret.Kind = SyntaxKind.MinorEqualsToken;
            else if (text.AdvanceIfMatches(stringValue = "<"))
                ret.Kind = SyntaxKind.MinorToken;
            else if (text.AdvanceIfMatches(stringValue = ">="))
                ret.Kind = SyntaxKind.MajorEqualsToken;
            else if (text.AdvanceIfMatches(stringValue = ">"))
                ret.Kind = SyntaxKind.MajorToken;
            else if (text.AdvanceIfMatches(stringValue = "%"))
                ret.Kind = SyntaxKind.ModToken;
            else if (text.AdvanceIfMatches(stringValue = "+="))
                ret.Kind = SyntaxKind.PlusEqualsToken;
            else if (text.AdvanceIfMatches(stringValue = "+"))
                ret.Kind = SyntaxKind.PlusToken;
            else if (text.AdvanceIfMatches(stringValue = "-="))
                ret.Kind = SyntaxKind.MinusEqualsToken;
            else if (text.AdvanceIfMatches(stringValue = "-"))
                ret.Kind = SyntaxKind.MinusToken;
            else if (text.AdvanceIfMatches(stringValue = "*="))
                ret.Kind = SyntaxKind.AsteriskEqualsToken;
            else if (text.AdvanceIfMatches(stringValue = "*"))
                ret.Kind = SyntaxKind.AsteriskToken;
            else if (text.AdvanceIfMatches(stringValue = "\\="))
                ret.Kind = SyntaxKind.SlashEqualsToken;
            else if (text.AdvanceIfMatches(stringValue = "\\"))
                ret.Kind = SyntaxKind.SlashToken;

            if (ret.Kind == SyntaxKind.None)
                return null;

            ret.StringValue = stringValue;
            ret.End = ret.Begin + stringValue.Length;

            return ret;
        }

        TokenInfo scanIdentifierOrKeyword()
        {
            var ret = new TokenInfo { Begin = text.Position };
            var stringValue = string.Empty;

            while (text.Peek().IsCharacter())
                stringValue += text.Next();

            if (string.IsNullOrEmpty(stringValue))
                return null;

            var kind = getKeywordKind(stringValue);
            if (kind == SyntaxKind.None)
                kind = SyntaxKind.Identifier;

            ret.StringValue = stringValue;
            ret.Kind = kind;
            ret.End = ret.Begin + stringValue.Length;

            return ret;
        }
        SyntaxKind getKeywordKind(string text)
        {
            switch (text)
            {
                case "for":
                    return SyntaxKind.ForKeyword;
                case "while":
                    return SyntaxKind.WhileKeyword;
                case "my":
                    return SyntaxKind.MyKeyword;
                case "everybody":
                    return SyntaxKind.EverybodyKeyword;
                case "if":
                    return SyntaxKind.IfKeyword;
                case "else":
                    return SyntaxKind.ElseKeyword;
                case "module":
                    return SyntaxKind.ModuleKeyword;
                case "void":
                    return SyntaxKind.VoidKeyword;
                case "int":
                    return SyntaxKind.IntKeyword;
                case "double":
                    return SyntaxKind.DoubleKeyword;
                case "string":
                    return SyntaxKind.StringKeyword;
                case "char":
                    return SyntaxKind.CharKeyword;
                case "byte":
                    return SyntaxKind.ByteKeyword;
                case "var":
                    return SyntaxKind.VarKeyword;
                case "implements":
                    return SyntaxKind.ImplementsDirectiveKeyword;
                default:
                    break;
            }
            return SyntaxKind.None;
        }


        TokenInfo scanTrivia()
        {
            TokenInfo ret;

            if ((ret = scanComment()) != null)
                return ret;
            if ((ret = scanWhitespaceTrivia()) != null)
                return ret;
            if ((ret = scanEndOfLineTrivia()) != null)
                return ret;

            return null;
        }
        TokenInfo scanWhitespaceTrivia()
        {
            var ret = new TokenInfo { Begin = text.Position, Kind = SyntaxKind.WhitespaceTrivia };
            var stringValue = string.Empty;

            while (text.Peek() == ' ')
                stringValue += text.Next();

            if (string.IsNullOrEmpty(stringValue))
                return null;

            ret.StringValue = stringValue;
            ret.End = ret.Begin + stringValue.Length;

            return ret;
        }
        TokenInfo scanEndOfLineTrivia()
        {
            var ret = new TokenInfo { Begin = text.Position, Kind = SyntaxKind.EndOfLineTrivia };
            var stringValue = string.Empty;
            char c;

            while ((c = text.Peek()) == '\n' || c == '\r')
                stringValue += text.Next();

            if (string.IsNullOrEmpty(stringValue))
                return null;

            ret.StringValue = stringValue;
            ret.End = ret.Begin + stringValue.Length;

            return ret;
        }

        TokenInfo scanComment()
        {
            var ret = new TokenInfo { Begin = text.Position, Kind = SyntaxKind.SingleLineCommentTrivia };
            var stringValue = "//";

            if (!text.Peek().Equals('/'))
                return null;

            if (text.AdvanceIfMatches("//"))
                while (!text.Peek().IsLineBreak())
                    stringValue += text.Next();
            else
                throw SyntacticException.SymbolNotExpected(text, text.Peek(1).ToString(), SyntaxKind.SingleLineCommentTrivia, "/");

            ret.StringValue = stringValue;
            ret.End = ret.Begin + stringValue.Length;

            return ret;
        }

        TokenInfo scanStringLiteral()
        {
            var ret = new TokenInfo { Begin = text.Position, Kind = SyntaxKind.StringLiteral };
            var stringValue = "\"";

            if (text.AdvanceIfMatches("\""))
            {
                char c;
                while ((c = text.Peek()) != '"')
                {
                    if (c.IsLineBreak())
                        throw SyntacticException.SymbolNotExpected(text, "line ending", SyntaxKind.StringLiteral, "\"");

                    stringValue += text.Next();
                }
                stringValue += c;
            }
            else
                return null;

            ret.StringValue = stringValue;
            ret.End = ret.Begin + stringValue.Length;

            return ret;
        }

        TokenInfo scanCharLiteral()
        {
            var ret = new TokenInfo { Begin = text.Position, Kind = SyntaxKind.CharLiteral };
            var stringValue = string.Empty;

            if (!text.Peek().Equals('\''))
                return null;

            stringValue += text.Next();
            if (text.Peek().Equals('\''))
                throw SyntacticException.SymbolNotExpected(text, text.Peek().ToString(), SyntaxKind.CharLiteral, "an character after ' token.");

            stringValue += text.Next();
            if (!text.Peek().Equals('\''))
                throw SyntacticException.SymbolNotExpected(text, text.Peek().ToString(), SyntaxKind.CharLiteral, "\"'\"");

            stringValue += text.Next();

            ret.StringValue = stringValue;
            ret.CharValue = stringValue[1];
            ret.End = ret.Begin + stringValue.Length;

            return ret;
        }

        TokenInfo scanNumericLiteral()
        {
            var tryByteLiteral = scanByteLiteral();
            if (tryByteLiteral != null)
                return tryByteLiteral;

            var integerToken = scanIntegerLiteral();
            if (integerToken == null)
                return null;

            TokenInfo doublePlace = null;

            if (text.AdvanceIfMatches('.'))
            {
                doublePlace = scanIntegerLiteral();
                if (doublePlace == null)
                    throw SyntacticException.SymbolNotExpected(text, text.Next().ToString(), SyntaxKind.DoubleLiteral, SyntaxKind.IntegerLiteral);
            }
            else
            {
                return integerToken;
            }

            return doublePlace.MakeDoublePlacePartOf(integerToken);
        }
        TokenInfo scanByteLiteral()
        {
            var ret = new TokenInfo { Begin = text.Position, Kind = SyntaxKind.ByteLiteral };

            if (!text.AdvanceIfMatches("0b"))
                return null;

            var stringValue = "0b";
            for (int i = 0; i < 8; i++)
            {
                if (text.Peek().IsBinary())
                    stringValue += text.Next();
                else
                    throw SyntacticException.SymbolNotExpected(text, text.Next().ToString(), SyntaxKind.ByteLiteral, "0", "1");
            }

            ret.End = ret.Begin + stringValue.Length;
            ret.StringValue = stringValue;
            ret.ByteValue = getByteValue(stringValue);

            return ret;
        }
        byte getByteValue(string text)
        {
            byte result = 0;

            for (int i = 2; i < 10; i++)
                if (text[i] == '1')
                    result += (byte)(1 << (9 - i));

            return result;
        }
        TokenInfo scanIntegerLiteral()
        {
            var ret = new TokenInfo { Begin = text.Position, Kind = SyntaxKind.IntegerLiteral };
            var stringValue = string.Empty;

            while (Char.IsDigit(text.Peek()))
                stringValue += text.Next();

            if (String.IsNullOrEmpty(stringValue))
                return null;

            ret.StringValue = stringValue;
            ret.IntValue = int.Parse(stringValue);
            ret.End = ret.Begin + stringValue.Length;

            return ret;
        }
    }
}
