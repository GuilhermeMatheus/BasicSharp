using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicSharp.Compiler.Parser.Extensions;

namespace BasicSharp.Compiler.Analyzer
{
    public sealed class AnalyzerManager
    {
        public CodeLens CodeLens { get; private set; }
        public CompilationBag CompilationBag { get; private set; }

        readonly Dictionary<SyntaxNode, VariableBag> bagFromNode = new Dictionary<SyntaxNode, VariableBag>();
        readonly List<AnalysisResult> analysisResults = new List<AnalysisResult>();
        
        public ReadOnlyCollection<AnalysisResult> AnalysisResults
        {
            get { return analysisResults.AsReadOnly(); }
        }

        public AnalyzerManager(Project project, CompilationUnit compilationUnit)
        {
            this.CompilationBag = new CompilationBag(project, compilationUnit);
            analyzeCompilationUnit();
        }

        void analyzeCompilationUnit()
        {
            var analysis = AnalyzerFactory.GetAnalyzerFor<CompilationUnit>(this).GetAnalysis(CompilationBag.CompilationUnit);
            addAnalysis(analysis);
        }

        public VariableBag AskForBag(SyntaxNode child)
        {
            var p = child.Parent;
            if (p == null)
                return null;

            if (bagFromNode.ContainsKey(child.Parent))
                return bagFromNode[p] + AskForBag(p.Parent);
            else
                return createBagFor(p) + AskForBag(p.Parent);
        }

        VariableBag createBagFor(SyntaxNode node)
        {
            var result = new VariableBag();

            IEnumerable<Statement> statements = null;

            if (node is MethodDeclaration)
            {
                var method = node as MethodDeclaration;
                foreach (var item in method.ParameterList.Parameters)
                {
                    var type = item.Type.GetCLRType();
                    var name = item.Identifier.StringValue;

                    result.AddKnowVariable(new Variable
                    {
                        ClrType = type,
                        Name = name
                    });
                }
                statements = method.Block.Statements;
            }
            else if (node is ForStatement) //Analizar o bloco inicializador do ForStatement
                statements = (node as ForStatement).Block.Statements;
            else if (node is IfStatement)
                throw new NotImplementedException();
            else if (node is WhileStatement)
                statements = (node as WhileStatement).Block.Statements;
            else if (node is BlockStatement)
                statements = (node as BlockStatement).Statements;

            var varDecls = from item in statements
                           where item.GetType() == typeof(LocalVariableDeclarationStatement)
                           select item as LocalVariableDeclarationStatement;

            var variables = (from item in varDecls select createVariableFrom(item));
            foreach (var item in variables.SelectMany(x => x))
                result.AddKnowVariable(item);

            bagFromNode.Add(node, result);

            return result;
        }
        IEnumerable<Variable> createVariableFrom(LocalVariableDeclarationStatement declaration)
        {
            var type = declaration.Declaration.Type.GetCLRType();

            foreach (var item in declaration.Declaration.Declarators)
                yield return new Variable
                {
                    ClrType = type,
                    Name = item.Identifier.StringValue
                };
        }

        bool addAnalysis(IEnumerable<AnalysisResult> analysis)
        {
            var src = analysis.Where(a => a != AnalysisResult.Empty);

            if (!src.Any())
                return false;

            analysisResults.AddRange(src);
            return true;
        }
    }
}