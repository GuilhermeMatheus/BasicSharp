using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.Analyzer
{
    public static class AnalysisResults
    {
        public static AnalysisResult CouldNotFoundType(ImplementsDirective node, string typeName)
        {
            var msg = string.Format("The type '{0}' could not be found (are you missing an assembly reference?)", typeName);
            return unrecognizedNode(node, msg);
        }

        public static AnalysisResult AmbiguousImplements(ImplementsDirective node, string fullClassName, List<Assembly> assemblies)
        {
            var msg = string.Format("The implements '{0}' is ambiguous between the following Assemblies: {2}", fullClassName, string.Join(" and ", assemblies.Select(x => x.FullName)));
            return unrecognizedNode(node, msg);
        }

        public static AnalysisResult InvalidType(ImplementsDirective node, string fullClassName)
        {
            var msg = string.Format("A implements directive can only be applied to static classes; the type '{0}' is not a static class", fullClassName);
            return unrecognizedNode(node, msg);
        }

        public static AnalysisResult AlreadyDefinedName(SyntaxNode node, string memberName)
        {
            var msg = string.Format("A member named '{0}' is already defined in this module", memberName);
            return unrecognizedNode(node, msg);
        }

        public static AnalysisResult EntryPointNotFound(ModuleDeclaration node)
        {
            var msg = string.Format("Module '{0}' does not contain a 'Main' method suitable for an entry point", node.Name.StringValue);
            return unrecognizedNode(node, msg);
        }

        public static AnalysisResult InvalidOperator(BinaryExpression node, Type lType, Type rType)
        {
            var msg = string.Format("Operator '{0}' cannot be applied to operands of type '{1}' and '{2}'", node.OperatorToken.StringValue, lType.Name, rType.Name);
            return unrecognizedNode(node, msg);
        }

        public static AnalysisResult InvalidAccessorName(SyntaxNode node, string name)
        {
            var msg = string.Format("The name '{0}' does not exist in the current context", name);
            return unrecognizedNode(node, msg);
        }

        public static AnalysisResult MemberIsDefinedMoreThanOnce(AccessorExpression node, string name)
        {
            var msg = string.Format("Member '{0}' is defined more than once", name);
            return unrecognizedNode(node, msg);
        }

        public static AnalysisResult OnlyLiteralsAllowed(Expression node)
        {
            var msg = string.Format("Only literals expressions are allowed in '{0}'", node.DisplayMember);
            return unrecognizedNode(node, msg);
        }

        public static AnalysisResult AmbiguousExternalMethod(MethodInvocationExpression node, List<Assembly> assemblies)
        {
            var msg = string.Format("The method '{0}' is declared in the following Assemblies: ", node.MethodName, string.Join(" and ", assemblies.Select(x => x.FullName)));
            return unrecognizedNode(node, msg);
        }

        public static AnalysisResult AmbiguousInternalMethod(MethodInvocationExpression node, List<MethodDeclaration> methods)
        {
            var msg = string.Format("The method '{0}' is declared in the following lines: ", node.MethodName, string.Join(" and ", methods.Select(x => x.Identifier.Line)));
            return unrecognizedNode(node, msg);
        }

        public static AnalysisResult InvalidConversion(SyntaxNode node, Type expectedType, Type found)
        {
            var msg = string.Format("Cannot implicitly convert type '{0}' to '{1}'", expectedType.Name, found.Name);
            return unrecognizedNode(node, msg);
        }

        static AnalysisResult unrecognizedNode(SyntaxNode node, string msg)
        {
            return new AnalysisResult
            {
                MessageResult = msg,
                Recognized = false,
                Node = node
            };
        }
    }
}