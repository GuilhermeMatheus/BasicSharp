using BasicSharp.Compiler.Analyzer;
using BasicSharp.Compiler.Lexer;
using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using BasicSharp.Compiler.ILEmitter.Extensions;

namespace BasicSharp.Compiler.ILEmitter
{
    public class VariableAssignmentExpressionEmitter : ExpressionEmitter<VariableAssignmentExpression>
    {
        public VariableAssignmentExpressionEmitter(CompilationBag compilationBag, ILocalIndexer localIndexer)
            : base(compilationBag, localIndexer) { }

        public override Tuple<Type, List<TacUnit>> GenerateWithType(VariableAssignmentExpression node, string labelPrefix = "IL_", int index = 0)
        {
            var assign = node.Assignment;
            var targetName = node.Identifier.StringValue;

            Type typeResult;
            var result = new List<TacUnit>();

            var expr = TacEmitterFactory.GenerateWithNode(assign.Expression, compilationBag, localIndexer, labelPrefix, index);
            typeResult = expr.Item1;
            result.AddRange(expr.Item2);

            var label = result.GetNextLabel();

            OpCode opAssignment;
            String valueAssignment;
            LocalInfo localTarget = localIndexer.GetLocalInfo(targetName);
            Variable moduleTarget = null;

            if (localTarget == null)
            {
                moduleTarget = compilationBag.GetField(targetName);
                opAssignment = OpCodes.Stsfld;
                var memberName = compilationBag.CompilationUnit.Module.Name.StringValue;
                valueAssignment = string.Format("{0} {1}::{2}", typeResult.GetMsilTypeName(), memberName, targetName);
            }
            else
            {
                if (localTarget.IsArgument)
                {
                    opAssignment = OpCodes.Starg;
                    valueAssignment = targetName;
                }
                else 
                {
                    opAssignment = OpCodes.Stloc;
                    valueAssignment = localTarget.Index.ToString();
                }
            }

            var operatorActions = getTacForAssignmentOperator(assign.OperatorToken.Kind, label.Item1, label.Item2, localTarget, valueAssignment);
            result.AddRange(operatorActions);

            label = result.GetNextLabel();

            result.Add(new TacUnit
            {
                LabelPrefix = label.Item1,
                LabelIndex = label.Item2,
                Op = opAssignment,
                Value = valueAssignment
            });

            return new Tuple<Type, List<TacUnit>>(typeResult, result);
        }

        List<TacUnit> getTacForAssignmentOperator(SyntaxKind syntaxKind, string labelPrefix, int index, LocalInfo local, string fieldAccess)
        {
            SyntaxKind[] compositeTypes = { 
                                              SyntaxKind.PlusEqualsToken,
                                              SyntaxKind.MinusEqualsToken,
                                              SyntaxKind.AsteriskEqualsToken,
                                              SyntaxKind.SlashEqualsToken
                                          };

            var result = new List<TacUnit>();

            if (!compositeTypes.Contains(syntaxKind))
                return result;

            var ldTac = new TacUnit
            {
                LabelPrefix = labelPrefix,
                LabelIndex = index++
            };

            if (local == null)
            {
                ldTac.Op = OpCodes.Ldsfld;
                ldTac.Value = fieldAccess;
            }
            else
            {
                ldTac.Op = OpCodes.Ldloc;
                ldTac.Value = local.Index.ToString();
            }

            result.Add(ldTac);

            OpCode op;

            switch (syntaxKind)
            {
                case SyntaxKind.PlusEqualsToken:
                    op = OpCodes.Add;
                    break;
                case SyntaxKind.MinusEqualsToken:
                    op = OpCodes.Sub;
                    break;
                case SyntaxKind.AsteriskEqualsToken:
                    op = OpCodes.Mul;
                    break;
                case SyntaxKind.SlashEqualsToken:
                    op = OpCodes.Div;
                    break;
                default:
                    throw new NotImplementedException();
            }

            result.Add(new TacUnit
            {
                LabelPrefix = labelPrefix,
                LabelIndex = index++,
                Op = op
            });

            return result;
        }
    }
}
