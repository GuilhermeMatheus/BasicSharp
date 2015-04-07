using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class LogicalOrExpression : BinaryExpression { }
    public class LogicalAndExpression : BinaryExpression { }
    
    public class LogicalGreaterThanExpression : BinaryExpression { }
    public class LogicalGreaterOrEqualThanExpression : BinaryExpression { }
    
    public class LogicalLessThanExpression : BinaryExpression { }
    public class LogicalLessOrEqualThanExpression : BinaryExpression { }

    public class EqualsEqualsExpression : BinaryExpression { }
}
