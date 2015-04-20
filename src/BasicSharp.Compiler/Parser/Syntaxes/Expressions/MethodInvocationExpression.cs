using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicSharp.Compiler.Parser.Extensions;
using BasicSharp.Compiler.Lexer;
using BasicSharp.Compiler.Parser;
using System.Collections;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class MethodInvocationExpression : Expression
    {
        public TokenInfo MethodName { get; internal set; }
        ArgumentList _arguments;
        public ArgumentList Arguments
        {
            get { return _arguments; }
            internal set
            {
                if (_arguments != value)
                {
                    _arguments = value;
                    Accept(value);
                }
            }
        }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            yield return MethodName;

            foreach (var item in Arguments.GetTokenEnumerable())
                yield return item;
        }

        public override IEnumerable GetChilds()
        {
            yield return MethodName;
            yield return Arguments;
        }
    }
}
