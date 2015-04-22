using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicSharp.Compiler.Parser.Extensions;
using System.Reflection;

namespace BasicSharp.Compiler.Analyzer
{
    public class CompilationBag
    {
        public static Type[] WellKnowTypes = {
                                                 typeof(int), typeof(string), typeof(double), typeof(char), typeof(bool), typeof(void),
                                                 typeof(int[]), typeof(string[]), typeof(double[]), typeof(char[]), typeof(bool[]) 
                                             };

        List<MethodStub> methodStubs = new List<MethodStub>();
        List<Type> allowedTypes = new List<Type>();
        List<Variable> fields = new List<Variable>();
        List<Assembly> assemblies = new List<Assembly>();
        List<Type> sessionTypes = new List<Type>();

        Dictionary<string, List<Assembly>> assemblyCache = new Dictionary<string, List<Assembly>>();

        public CompilationUnit CompilationUnit { get; private set; }
        public Project Project { get; private set; }
        public ReadOnlyCollection<Variable> Fields
        {
            get { return fields.AsReadOnly(); }
        }
        public ReadOnlyCollection<MethodStub> MethodStubs
        {
            get { return methodStubs.AsReadOnly(); }
        }
        public AnalyzerManager Analyzer { get; private set; }

        public CompilationBag(Project project, CompilationUnit compilationUnit)
        {
            this.Project = project;
            this.CompilationUnit = compilationUnit;
            this.Analyzer = new AnalyzerManager(this);

            initialize();
        }

        void initialize()
        {
            loadAssemblies();
            loadInternalMethodStubs();
            loadExternalMethodStubs();

            loadField();
        }

        //1. Verifica se a classe existe
        public bool ContainsClass(string fullClassName)
        {
            return this.assemblies.SelectMany(x => x.GetTypes())
                                  .Select(t => t.FullName)
                                  .Any(n => n.Equals(fullClassName));
        }
        //2. Verifica os assemblies que contém a classe
        public IEnumerable<Assembly> GetAssembliesForClass(string fullClassName)
        {
            if (assemblyCache.ContainsKey(fullClassName))
                return assemblyCache[fullClassName];

            var result = from ass in assemblies
                         let tuple = new { ass, types = ass.GetTypes() }
                         where tuple.types.Any(t => t.FullName.Equals(fullClassName))
                         select tuple.ass;

            assemblyCache.Add(fullClassName, result.ToList());

            return result;
        }
        //3. Verifica se a classe é Sealed e Abstract
        public bool IsValidClass(string fullClassName)
        {
            var src = GetAssembliesForClass(fullClassName);
            
            if (!src.Any())
                return false;

            if (src.Count() > 1)
                return false;

            var _class = from c in src.First().GetTypes()
                         where c.FullName.Equals(fullClassName) &&
                                                     c.IsSealed && 
                                                   c.IsAbstract
                             select c;

            allowedTypes.AddRange(_class);

            //Só poderá existir um tipo com estes atributos
            return _class.Any();
        }

        #region MethodStubs
        void loadAssemblies()
        {
            foreach (var item in Project.AssembliesAddress)
                assemblies.Add(Assembly.LoadFrom(item));
        }
        void loadInternalMethodStubs()
        {
            var internalMethods = from item in CompilationUnit.Module.Members
                                  where item is MethodDeclaration
                                  select item as MethodDeclaration;

            var internalMethodStubs = from item in internalMethods
                                      let name = item.Name
                                      let parameters = from p in item.ParameterList.Parameters
                                                       select new MethodStubParameter
                                                       {
                                                           Name = p.Identifier.StringValue,
                                                           Type = p.Type.GetCLRType()
                                                       }
                                      select new MethodStub(name, true, parameters)
                                      {
                                          IsInternal = true,
                                          InternalDefinition = item,
                                          ReturnType = item.ReturnType.GetCLRType()
                                      };

            this.methodStubs.AddRange(internalMethodStubs);
        }
        void loadExternalMethodStubs()
        {
            foreach (var ass in assemblies)
            {
                var staticClasses = from c in allowedTypes //ass.GetTypes()
                                    where c.IsSealed && c.IsAbstract
                                    select c;

                var validMethods = from m in staticClasses.SelectMany(c => c.GetMethods(BindingFlags.Public | BindingFlags.Static))
                                   where isValidMethod(m)
                                   select m;

                var externalMethodStubs = from m in validMethods
                                          select MethodStub.FromMethodInfo(m);

                this.methodStubs.AddRange(externalMethodStubs);
            }
        }
        bool isValidMethod(MethodInfo m)
        {
            if (!WellKnowTypes.Contains(m.ReturnType))
                return false;

            if (m.GetParameters()
                 .Select(x => x.ParameterType)
                 .Any(x => !WellKnowTypes.Contains(x))) return false;

            if (m.IsGenericMethod || m.IsGenericMethodDefinition)
                return false;

            return true;
        }
        #endregion

        public void AddSessionType(Type type)
        {
            this.sessionTypes.Add(type);
        }

        public List<MethodInfo> GetExternalMethodsFromSession(MethodInvocationExpression call, IEnumerable<Type> parameters)
        {
            var methods = from item in sessionTypes
                          let method = item.GetMethod(call.MethodName.StringValue, parameters.ToArray())
                          where method != null
                          select method;
            
            return methods.ToList();
        }

        public MethodDeclaration GetInternalMethodDefinition(MethodInvocationExpression call)
        {
            var methods = from item in MethodStubs
                          where item.IsInternal && item.Name.Equals(call.MethodName.StringValue)
                          select item;

            if (!methods.Any())
                return null;

            return methods.First().InternalDefinition;
        }

        public Variable GetField(string name)
        {
            return fields.Where(f => f.Name == name).FirstOrDefault();
        }

        void loadField()
        {
            var declarators = from item in CompilationUnit.Module.Members
                              where item is FieldDeclaration
                              let decl = (item as FieldDeclaration).Declaration
                              select new {
                                  type = decl.Type.GetCLRType(),
                                  vars = decl.Declarators
                              };

            var result = new List<Variable>();
            foreach (var item in declarators) {
                var type = item.type;
                foreach (var name in item.vars)
                    result.Add(new Variable
                    {
                        Name = name.Identifier.StringValue,
                        ClrType = type,
                        Definition = name
                    });
            }

            this.fields.AddRange(result);
        }
       
    }
}
