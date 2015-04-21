using BasicSharp.Compiler.Analyzer;
using BasicSharp.Compiler.Lexer;
using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.ILEmitter
{
    public class LiteralExpressionEmitter : ExpressionEmitter<LiteralExpression>
    {
        public LiteralExpressionEmitter(CompilationBag compilationBag, ILocalIndexer localIndexer)
            : base(compilationBag, localIndexer) { }
        
        public override Tuple<Type, List<TacUnit>> GenerateWithType(LiteralExpression node, string labelPrefix = "IL_", int index = 0)
        {
            Type typeResult;
            var result = new List<TacUnit>();

            var tac = new TacUnit
            {
                LabelPrefix = labelPrefix,
                LabelIndex = index
            };

            var kind = node.Value.Kind;
            switch (kind)
            {
                case SyntaxKind.NullKeyword:
                    typeResult = null;
                    tac.Op = OpCodes.Ldnull;
                    break;
                case SyntaxKind.TrueKeyword:
                case SyntaxKind.FalseKeyword:
                    typeResult = typeof(bool);
                    tac.Op = kind == SyntaxKind.TrueKeyword ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0;
                    break;
                case SyntaxKind.StringLiteral:
                    typeResult = typeof(string);
                    tac.Op = OpCodes.Ldstr;
                    tac.Value = node.Value.StringValue;
                    break;
                case SyntaxKind.CharLiteral:
                    typeResult = typeof(char);
                    tac.Op = OpCodes.Ldc_I4_S;
                    tac.Value = ((int)node.Value.CharValue).ToString();
                    break;
                case SyntaxKind.IntegerLiteral:
                    typeResult = typeof(int);
                    tac.Op = OpCodes.Ldc_I4;
                    tac.Value = node.Value.IntValue.ToString();
                    break;
                case SyntaxKind.DoubleLiteral:
                    typeResult = typeof(double);
                    tac.Op = OpCodes.Ldc_I4;
                    tac.Value = node.Value.DoubleValue.ToString();
                    break;
                case SyntaxKind.ByteLiteral:
                    typeResult = typeof(byte);
                    tac.Op = OpCodes.Ldc_I4_S;
                    tac.Value = ((int)node.Value.ByteValue).ToString();
                    break;
                default:
                    throw new ArgumentException("node.Value.Kind");
            }

            result.Add(tac);

            return new Tuple<Type, List<TacUnit>>(typeResult, result);
        }
    }
}
