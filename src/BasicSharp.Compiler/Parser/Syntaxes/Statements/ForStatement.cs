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

        Expression _initializer;
        public Expression Initializer
        {
            get { return _initializer; }
            internal set
            {
                if (_initializer != value)
                {
                    _initializer = value;
                    Accept(value);
                }
            }
        }
        public TokenInfo FirstSemicolonToken { get; internal set; }

        Expression _condition;
        public Expression Condition
        {
            get { return _condition; }
            internal set
            {
                if (_condition != value)
                {
                    _condition = value;
                    Accept(value);
                }
            }
        }
        public TokenInfo SecondSemicolonToken { get; internal set; }

        Expression _incrementor;
        public Expression Incrementor
        {
            get { return _incrementor; }
            internal set
            {
                if (_incrementor != value)
                {
                    _incrementor = value;
                    Accept(value);
                }
            }
        }
        public TokenInfo CloseParenToken { get; internal set; }

        BlockStatement _block;
        public BlockStatement Block
        {
            get { return _block; }
            internal set
            {
                if (_block != value)
                {
                    _block = value;
                    Accept(value);
                }
            }
        }

        public override IEnumerable GetChilds()
        {
            yield return ForToken;
            yield return OpenParenToken;
            yield return Initializer;
            yield return FirstSemicolonToken;
            yield return Condition;
            yield return SecondSemicolonToken;
            yield return Incrementor;
            yield return CloseParenToken;
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

            yield return CloseParenToken;

            foreach (var item in Block.GetTokenEnumerable())
                yield return item;
        }
    }
}
