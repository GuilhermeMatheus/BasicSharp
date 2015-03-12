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
            this(new SlidingText(sourceStream))
        {

        }

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
                    case '/':
                        yield return scanComment();
                        break;
                    case '"':
                        yield return scanStringLiteral();
                        break;
                    case '\'':
                        yield return scanCharLiteral();
                        break;
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
                        break;
                    case SlidingText.INVALID_CHAR:
                    default:
                        yield break;
                }
            }
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
            byte result  = 0;

            for (int i = 2; i < 10; i++)
                if (text[i] == '1')
                    result += (byte)(1 << (9 - i));

            return result;
        }
        TokenInfo scanIntegerLiteral()
        {
            var ret = new TokenInfo { Begin = text.Position, Kind = SyntaxKind.IntegerLiteral };
            var stringValue = string.Empty;

            while(Char.IsDigit(text.Peek()))
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
