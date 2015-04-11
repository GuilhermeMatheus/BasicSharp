using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicSharp.Compiler.Parser.Extensions;
using System.Reflection;

namespace BasicSharp.Compiler.Analizer
{
    public class CompilationBag
    {
        public static Type[] WellKnowTypes = {
                                                 typeof(int), typeof(string), typeof(double), typeof(char), typeof(bool), typeof(void),
                                                 typeof(int[]), typeof(string[]), typeof(double[]), typeof(char[]), typeof(bool[]) 
                                             };

        List<MethodStub> methodStubs = new List<MethodStub>();
        List<Variable> fields = new List<Variable>();

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

        public CompilationBag(Project project, CompilationUnit compilationUnit)
        {
            this.Project = project;
            this.CompilationUnit = compilationUnit;

            initialize();
        }

        void initialize()
        {
            loadInternalMethodStubs();
            loadExternalMethodStubs();

            loadField();
        }

        #region MethodStubs
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
                                      select new MethodStub(name, parameters);

            this.methodStubs.AddRange(internalMethodStubs);
        }
        void loadExternalMethodStubs()
        {
            foreach (var item in Project.AssembliesAddress)
            {
                var ass = Assembly.LoadFrom(item);

                var staticClasses = from c in ass.GetTypes()
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

        void loadField()
        {
            var declarators = from item in CompilationUnit.Module.Members
                              where item is FieldDeclaration
                              let decl = (item as FieldDeclaration).Declaration
                              select new {
                                  type = decl.Type.GetCLRType(),
                                  vars = from declarator in decl.Declarators
                                         select declarator.Identifier.StringValue
                              };

            var result = new List<Variable>();
            foreach (var item in declarators) {
                var type = item.type;
                foreach (var name in item.vars)
                    result.Add(new Variable { Name = name, ClrType = type });
            }

            this.fields.AddRange(result);
        }
    }
}
