using BasicSharp.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicSharp.Compiler.Parser.Extensions;
using System.Collections;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class VariableAssignmentExpression : Expression
    {
        public TokenInfo Identifier { get; set; }
        AssignmentExpression _assignment;
        public AssignmentExpression Assignment
        {
            get { return _assignment; }
            internal set
            {
                if (_assignment != value)
                {
                    _assignment = value;
                    Accept(value);
                }
            }
        }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            yield return Identifier;
            foreach (var item in Assignment.GetTokenEnumerable())
                yield return item;
        }

        public override IEnumerable GetChilds()
        {
            yield return Identifier;
            yield return Assignment;
        }
    }
}
