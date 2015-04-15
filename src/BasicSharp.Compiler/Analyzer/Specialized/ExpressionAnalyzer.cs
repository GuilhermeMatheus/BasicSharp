using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicSharp.Compiler.Lexer.Extensions;
using BasicSharp.Compiler.Parser.Extensions;
using BasicSharp.Compiler.Lexer;
using BasicSharp.Compiler.Analyzer.Extensions;

namespace BasicSharp.Compiler.Analyzer
{
    public class ExpressionAnalyzer : SpecializedAnalyzer<Expression>
    {
        Expression rootNode;

        public bool OnlyLiterals { get; private set; }
        public Type Type { get; private set; }
        
        public ExpressionAnalyzer(AnalyzerManager manager, bool onlyLiterals = false)
            : base(manager)
        {
            this.OnlyLiterals = onlyLiterals;
        }

        public override IEnumerable<AnalysisResult> GetAnalysis(Expression node)
        {
            rootNode = node;
            
            var analysisSession = new List<AnalysisResult>();

            this.Type = analyze(node, analysisSession);

            return analysisSession;
        }

        Type analyze(Expression node, List<AnalysisResult> analysisSession)
        {
            if (node is BinaryExpression)
                return analyzeBinaryExpression(node as BinaryExpression, analysisSession);

            else if (node is LiteralExpression)
                return (node as LiteralExpression).Value.Kind.GetLiteralCLRType();

            else if (node is AccessorExpression)
            {
                if (OnlyLiterals)
                    analysisSession.Add(AnalysisResults.OnlyLiteralsAllowed(rootNode));

                return analyzeAccessor(node as AccessorExpression, analysisSession);
            }
            else if (node is MethodInvocationExpression)
            {
                if (OnlyLiterals)
                    analysisSession.Add(AnalysisResults.OnlyLiteralsAllowed(rootNode));

                return analyzeMethodInvocation(node as MethodInvocationExpression, analysisSession);
            }

            node.ToString();

            throw null;
        }

        Type analyzeMethodInvocation(MethodInvocationExpression methodInvocationExpression, List<AnalysisResult> analysisSession)
        {
            var internalDeclarations = manager.CompilationBag.MethodStubs.Where(m => m.IsInternal).ToList();
            
            if (internalDeclarations.Count == 0)
            {
                var externalDeclarations = manager.CompilationBag.MethodStubs.Where(m => !m.IsInternal).ToList();
                
                if (externalDeclarations.Count == 0)
                {
                    analysisSession.Add(AnalysisResults.InvalidAccessorName(methodInvocationExpression, methodInvocationExpression.MethodName.StringValue));
                    return null;
                }
                else if (externalDeclarations.Count == 1)
                {
                    return externalDeclarations[0].ReturnType;
                }
                else
                {
                    analysisSession.Add(AnalysisResults.AmbiguousExternalMethod(methodInvocationExpression, externalDeclarations.Select(x => x.ExternalAssembly).ToList()));
                    return null;
                }
            }
            else if (internalDeclarations.Count == 1)
            {
                return internalDeclarations[0].ReturnType;
            }
            else
            {
                analysisSession.Add(AnalysisResults.AmbiguousInternalMethod(methodInvocationExpression, internalDeclarations.Select(x => x.InternalDefinition).ToList()));
                foreach (var item in internalDeclarations)
                    manager.CodeLens.AddCodeInfoTo(item.InternalDefinition, CodeInfo.Reference);

                return null;
            }


        }

        Type analyzeAccessor(AccessorExpression accessor, List<AnalysisResult> analysisSession)
        {
            var variables = manager.AskForBag(accessor).GetDefinitionFor(accessor);

            if (variables.Count == 0)
            {
                analysisSession.Add(AnalysisResults.InvalidAccessorName(accessor, accessor.Identifier.StringValue));
                return null;
            }
            else
            {
                if (variables.Count > 1)
                    analysisSession.Add(AnalysisResults.MemberIsDefinedMoreThanOnce(accessor, accessor.Identifier.StringValue));

                foreach (var item in variables)
                    manager.CodeLens.AddCodeInfoTo(item.Definition, CodeInfo.Reference);

                return variables[0].ClrType;
            }
        }

        Type analyzeBinaryExpression(BinaryExpression b, List<AnalysisResult> analysisSession)
        {
            var kind = b.OperatorToken.Kind;
            var isLogicalOperator = kind.IsLogicalOperator();

            var lType = analyze(b.LeftSide, analysisSession);
            var rType = analyze(b.RightSide, analysisSession);

            var result = TypeExtensions.GetSuitableType(lType, rType);

            if (result == null)
                analysisSession.Add(AnalysisResults.InvalidOperator(b, lType, rType));
            else if (result == typeof(string) || result == typeof(char))
            {
                if (!kind.IsIn(SyntaxKind.EqualsEqualsOperator, SyntaxKind.ExclamationEqualsToken))
                    analysisSession.Add(AnalysisResults.InvalidOperator(b, lType, rType));
            }

            return isLogicalOperator ? typeof(bool) : result;
        }

        
    }
}