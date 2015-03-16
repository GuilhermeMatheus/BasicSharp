using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicSharp.Utils;
using System.Collections.ObjectModel;
using BasicSharp.Compiler.Lexer.Extensions;

namespace BasicSharp.Compiler.Lexer
{
    public class Lexer
    {
        readonly SlidingText text;

        List<SyntacticException> _syntacticErrors;
        public ReadOnlyCollection<SyntacticException> SyntacticErrors
        {
            get { return _syntacticErrors.AsReadOnly(); }
        }

        public Lexer(Stream sourceStream) : this(new SlidingText(sourceStream)) { }

        public Lexer(SlidingText text)
        {
            this.text = text;
            this._syntacticErrors = new List<SyntacticException>();
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
                    case '_':
                        yield return scanIdentifierOrKeyword();
                        continue;
                    case '/':
                    case '\t':
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
                            Begin = text.Offset,
                            End = text.Offset + 1,
                            StringValue = text.Peek().ToString(),
                            Kind = getSyntaxKind(text.Next())
                        };
                        continue;
                    case SlidingText.INVALID_CHAR:
                        yield break;
                    default:
                        handleSyntacticError(SyntacticExceptions.SymbolNotExpected(text, ch.ToString(), null, new string[] { }), null);
                        continue;
                }
            }
        }

        #region Scan methods
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
            var ret = new TokenInfo { Begin = text.Offset, Kind = SyntaxKind.None };
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
            if (!text.Peek().IsCharacter())
                return null;

            var ret = new TokenInfo { Begin = text.Offset };
            var stringValue = text.Next().ToString();

            char c; 
            while ((c = text.Peek()).IsCharacter() || c.IsDigit())
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
                case "return":
                    return SyntaxKind.ReturnKeyword;
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
            if ((ret = scanTabTrivia()) != null)
                return ret;
            if ((ret = scanWhitespaceTrivia()) != null)
                return ret;
            if ((ret = scanEndOfLineTrivia()) != null)
                return ret;

            return null;
        }
        TokenInfo scanTabTrivia()
        {
            var ret = new TokenInfo { Begin = text.Offset, Kind = SyntaxKind.TabTrivia };
            var stringValue = string.Empty;

            while (text.Peek() == '\t')
                stringValue += text.Next();

            if (string.IsNullOrEmpty(stringValue))
                return null;

            ret.StringValue = stringValue;
            ret.End = ret.Begin + stringValue.Length;

            return ret;
        }
        TokenInfo scanWhitespaceTrivia()
        {
            var ret = new TokenInfo { Begin = text.Offset, Kind = SyntaxKind.WhitespaceTrivia };
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
            var ret = new TokenInfo { Begin = text.Offset, Kind = SyntaxKind.EndOfLineTrivia };
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
            if (!text.Peek().Equals('/'))
                return null;

            var ret = new TokenInfo { Begin = text.Offset, Kind = SyntaxKind.SingleLineCommentTrivia };
            var stringValue = "//";

            if (text.AdvanceIfMatches("//"))
                while (!text.Peek().IsLineBreak())
                    stringValue += text.Next();
            else
                handleSyntacticError(SyntacticExceptions.SymbolNotExpected(text, text.Peek(1).ToString(), ret, "/"), ret);

            ret.StringValue = stringValue;
            ret.End = ret.Begin + stringValue.Length;

            return ret;
        }

        TokenInfo scanStringLiteral()
        {
            var ret = new TokenInfo { Begin = text.Offset, Kind = SyntaxKind.StringLiteral };
            var stringValue = "\"";

            if (text.AdvanceIfMatches("\""))
            {
                char c;
                while ((c = text.Peek()) != '"')
                {
                    if (c.IsLineBreak())
                    {
                        handleSyntacticError(SyntacticExceptions.SymbolNotExpected(text, "line ending", ret, "\""), ret);
                        break;
                    }
                    stringValue += text.Next();
                }
                stringValue += text.Next();
            }
            else
                return null;

            ret.StringValue = stringValue;
            ret.End = ret.Begin + stringValue.Length;

            return ret;
        }
        TokenInfo scanCharLiteral()
        {
            var ret = new TokenInfo { Begin = text.Offset, Kind = SyntaxKind.CharLiteral };
            var stringValue = "'";

            if (text.AdvanceIfMatches("'"))
            {
                char c;
                while ((c = text.Peek()) != '\'')
                {
                    if (c.IsLineBreak())
                    {
                        handleSyntacticError(SyntacticExceptions.SymbolNotExpected(text, "line ending", ret, "'"), ret);
                        break;
                    }
                    stringValue += text.Next();
                }
                stringValue += text.Next();
            }
            else
                return null;

            if (stringValue.Length < 3)
                handleSyntacticError(SyntacticExceptions.EmptyCharLiteral(text, ret), ret);
            if (stringValue.Length > 3)
                handleSyntacticError(SyntacticExceptions.TooManyCharactersInCharLiteral(text, ret), ret);

            ret.StringValue = stringValue;
            ret.End = ret.Begin + stringValue.Length;
            if (!ret.IsMalformedToken)
                ret.CharValue = stringValue[1];

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

            TokenInfo doublePlace;

            if (text.AdvanceIfMatches('.'))
            {
                doublePlace = scanIntegerLiteral();
                if (doublePlace == null)
                {
                    handleSyntacticError(SyntacticExceptions.SymbolNotExpected(text, text.Peek().ToString(), integerToken, SyntaxKind.IntegerLiteral), integerToken);
                    return integerToken;
                }
            }
            else
            {
                return integerToken;
            }

            return doublePlace.MakeDoublePlacePartOf(integerToken);
        }
        TokenInfo scanByteLiteral()
        {
            var ret = new TokenInfo { Begin = text.Offset, Kind = SyntaxKind.ByteLiteral };

            if (!text.AdvanceIfMatches("0b"))
                return null;

            var stringValue = "0b";
            for (int i = 0; i < 8; i++)
            {
                if (text.Peek().IsBinary())
                    stringValue += text.Next();
                else
                {
                    handleSyntacticError(SyntacticExceptions.SymbolNotExpected(text, text.Peek().ToString(), ret, "0", "1"), ret);
                    break;
                }
            }

            ret.End = ret.Begin + stringValue.Length;
            ret.StringValue = stringValue;
            
            if (!ret.IsMalformedToken)
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
            var ret = new TokenInfo { Begin = text.Offset, Kind = SyntaxKind.IntegerLiteral };
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
        #endregion

        void handleSyntacticError(SyntacticException error, TokenInfo currentToken)
        {
            if (currentToken != null)
                currentToken.IsMalformedToken = true;
            
            _syntacticErrors.Add(error);
            
            text.Next();
        }
    }
}
