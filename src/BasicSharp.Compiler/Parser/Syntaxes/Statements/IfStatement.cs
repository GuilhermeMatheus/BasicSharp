using BasicSharp.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using BasicSharp.Compiler.Parser.Extensions;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class IfStatement : Statement
    {
        public TokenInfo IfToken { get; internal set; }
        public TokenInfo OpenParenToken { get; internal set; }
        public Expression Condition { get; internal set; }
        public TokenInfo CloseParenToken { get; internal set; }
        public BlockStatement Then { get; internal set; }

        public TokenInfo ElseToken { get; internal set; }
        public BlockStatement Else { get; internal set; }

        public override IEnumerable GetChilds()
        {
            yield return IfToken;
            yield return OpenParenToken;
            yield return CloseParenToken;
            yield return Condition;
            yield return Then;
            yield return ElseToken;
            yield return Else;
        }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            yield return IfToken;
            yield return OpenParenToken;
            
            foreach (var item in Condition.GetTokenEnumerable())
                yield return item;

            yield return CloseParenToken;
            
            foreach (var item in Then.GetTokenEnumerable())
                yield return item;
            
            yield return ElseToken;

            foreach (var item in Else.GetTokenEnumerable())
                yield return item;
        }
    }
}