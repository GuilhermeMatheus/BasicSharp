using BasicSharp.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicSharp.Compiler.Parser.Extensions;
using System.Collections;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class ForStatement : Statement
    {
        public TokenInfo ForToken { get; internal set; }
        public TokenInfo OpenParenToken { get; internal set; }
        
        public Expression Initializer { get; internal set; }
        public TokenInfo FirstSemicolonToken { get; internal set; }
        
        public Expression Condition { get; internal set; }
        public TokenInfo SecondSemicolonToken { get; internal set; }
        
        public Expression Incrementor { get; internal set; }

        public BlockStatement Block { get; internal set; }

        public override IEnumerable GetChilds()
        {
            yield return ForToken;
            yield return OpenParenToken;
            yield return Initializer;
            yield return FirstSemicolonToken;
            yield return Condition;
            yield return SecondSemicolonToken;
            yield return Incrementor;
            yield return Block;
        }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            yield return ForToken;
            yield return OpenParenToken;
            
            foreach (var item in Initializer.GetTokenEnumerable())
                yield return item;
            
            yield return FirstSemicolonToken;
            
            foreach (var item in Condition.GetTokenEnumerable())
                yield return item;
            
            yield return SecondSemicolonToken;
            
            foreach (var item in Incrementor.GetTokenEnumerable())
                yield return item;
            
            foreach (var item in Block.GetTokenEnumerable())
                yield return item;
        }
    }
}
