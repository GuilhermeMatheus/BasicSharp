using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler
{
    public class MethodStub
    {
        List<MethodStubParameter> parameters;

        public string Name { get; internal set; }
        public int Arity
        {
            get { return parameters.Count; }
        }
        public ReadOnlyCollection<MethodStubParameter> Parameters
        {
            get { return parameters.AsReadOnly(); }
        }

        public Assembly ExternalAssembly { get; private set; }

        public bool IsInternal { get; internal set; }
        public MethodDeclaration InternalDefinition { get; set; }

        public Type ReturnType { get; internal set; }

        public MethodStub(string name, bool isInternal, IEnumerable<MethodStubParameter> parameters)
        {
            this.Name = name;
            this.IsInternal = IsInternal;
            this.parameters = parameters == null ? new List<MethodStubParameter>() : parameters.ToList();
        }
        public static MethodStub FromMethodInfo(MethodInfo method)
        {
            var name = method.Name;
            var parameters = from item in method.GetParameters()
                             select new MethodStubParameter
                             {
                                 Name = item.Name,
                                 Type = item.ParameterType
                             };

            return new MethodStub(name, false, parameters)
            {
                ReturnType = method.ReturnType,
                ExternalAssembly = method.DeclaringType.Assembly
            };
        }
    }
}
