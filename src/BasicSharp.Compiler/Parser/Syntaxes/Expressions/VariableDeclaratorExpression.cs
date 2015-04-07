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
        public AssignmentExpression Assignment { get; set; }

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
