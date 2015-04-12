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
            var msg = "The implements is ambiguous between the following Assemblies: " + string.Join(" and ", assemblies.Select(x => x.FullName));
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
            var msg = string.Format("Module '{0}' does not contain a 'Main' method suitable for an entry point", node.Name);
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
