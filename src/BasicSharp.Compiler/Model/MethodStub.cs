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
        public int Arity { get; internal set; }
        public ReadOnlyCollection<MethodStubParameter> Parameters
        {
            get { return parameters.AsReadOnly(); }
        }

        public MethodStub(string name, IEnumerable<MethodStubParameter> parameters)
        {
            this.Name = name;
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

            return new MethodStub(name, parameters);
        }

    }
}
