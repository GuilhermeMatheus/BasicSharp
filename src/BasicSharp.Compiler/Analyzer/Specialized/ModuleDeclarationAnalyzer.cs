using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicSharp.Compiler.Analyzer
{
    public class ModuleDeclarationAnalyzer : SpecializedAnalyzer<ModuleDeclaration>
    {
        public ModuleDeclarationAnalyzer(AnalyzerManager manager)
            : base(manager) { }

        public override IEnumerable<AnalysisResult> GetAnalysis(ModuleDeclaration node)
        {
            bool containsMainMethod = false;
            var membersByName = new Dictionary<string, List<SyntaxNode>>();

            var auxListForFields = new List<FieldDeclaration>();
            var auxListMethods = new List<MethodDeclaration>();
            
            foreach (var item in node.Members)
            {
                if (item is MethodDeclaration)
                {
                    var method = item as MethodDeclaration;
                    auxListMethods.Add(method);

                    containsMainMethod = containsMainMethod || (method.Name.Equals("Main"));

                    if (membersByName.ContainsKey(method.Name))
                        membersByName[method.Name].Add(method);
                    else
                        membersByName.Add(method.Name, new List<SyntaxNode> { method });
                }
                else
                {
                    var fieldsDecl = item as FieldDeclaration;
                    auxListForFields.Add(fieldsDecl);

                    foreach (var _field in fieldsDecl.Declaration.Declarators)
                    {
                        var name = _field.Identifier.StringValue;
                        if (membersByName.ContainsKey(name))
                            membersByName[name].Add(_field);
                        else
                            membersByName.Add(name, new List<SyntaxNode> { _field });
                    }
                }
            }

            foreach (var item in membersByName.Where(x => x.Value.Count > 1))
                foreach (var ambiguous in item.Value.Skip(1))
                    yield return AnalysisResults.AlreadyDefinedName(ambiguous, item.Key);

            if (!containsMainMethod)
                yield return AnalysisResults.EntryPointNotFound(node);

            foreach (var item in AnalysisFor(auxListForFields))
                yield return item;

            foreach (var item in AnalysisFor(auxListForFields))
                yield return item;
        }
    }
}
