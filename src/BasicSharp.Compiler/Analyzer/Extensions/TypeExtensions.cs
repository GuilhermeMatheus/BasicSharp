using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.Analyzer.Extensions
{
    public static class TypeExtensions
    {
        public static Type GetSuitableType(Type l, Type r)
        {
            if (l == typeof(string))
                return r == typeof(string) ? l : null;

            if (l == typeof(char))
                return r == typeof(char) ? l : null;

            if (l == typeof(bool))
                return r == typeof(bool) ? l : null;

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
