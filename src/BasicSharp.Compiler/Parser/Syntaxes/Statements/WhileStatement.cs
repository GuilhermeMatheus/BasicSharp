using BasicSharp.Compiler.Lexer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicSharp.Compiler.Parser.Extensions;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class WhileStatement : Statement
    {
        public TokenInfo WhileToken { get; internal set; }
        public TokenInfo OpenParenToken { get; internal set; }
        public TokenInfo CloseParenToken { get; internal set; }

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
            yield return WhileToken;
            yield return OpenParenToken;
            yield return Condition;
            yield return CloseParenToken;
            yield return Block;
        }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            yield return WhileToken;
            yield return OpenParenToken;
            
            foreach (var item in Condition.GetTokenEnumerable())
                yield return item;
            
            yield return CloseParenToken;

            foreach (var item in Block.GetTokenEnumerable())
                yield return item;
        }
    }
}
