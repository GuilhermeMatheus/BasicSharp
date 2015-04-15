using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.Analyzer.Extensions
{
    public static class TypeExtensions
    {
        static readonly Type[] NoHierarchyTypes = { typeof(string), typeof(char), typeof(bool) };

        public static Type GetSuitableType(Type l, Type r)
        {
            if (NoHierarchyTypes.Contains(l) || NoHierarchyTypes.Contains(r))
                return (l == r) ? l : null;
            
            return GetMaxNumericAncestor(l, r);
        }

        public static Type GetMaxNumericAncestor(Type l, Type r)
        {
            if (l == typeof(double) || r == typeof(double))
                return typeof(double);

            if (l == typeof(int) || r == typeof(int))
                return typeof(int);

            if (l == typeof(byte) || r == typeof(byte))
                return typeof(byte);

            return null;
        }

    }
}
