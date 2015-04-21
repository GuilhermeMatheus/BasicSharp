using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.ILEmitter.Extensions
{
    public static class TypeExtensions
    {
        public static string GetMsilTypeName(this Type type)
        {
            if (type == typeof(int))
                return "int32";
            if (type == typeof(int[]))
                return "int32[]";
            
            if (type == typeof(string))
                return "string";
            if (type == typeof(string[]))
                return "string[]";
            
            if (type == typeof(char))
                return "char";
            if (type == typeof(char[]))
                return "char[]";
            
            if (type == typeof(byte))
                return "uint8";
            if (type == typeof(byte[]))
                return "uint8[]";

            if (type == typeof(double))
                return "float64";
            if (type == typeof(double[]))
                return "float64[]";

            if (type == typeof(bool)) 
                return "bool";
            if (type == typeof(bool[]))
                return "bool[]";

            if (type == typeof(void))
                return "void";

            throw new ArgumentException("type");
        }

        public static OpCode GetOpCodeToPushValue(this Type type)
        {
            if (type.IsArray)
                return OpCodes.Newarr;

            if (type == typeof(int))
                return OpCodes.Ldc_I4;
            if (type == typeof(string))
                return OpCodes.Ldstr;
            if (type == typeof(char))
                return OpCodes.Ldc_I4_S;
            if (type == typeof(byte))
                return OpCodes.Ldc_I4_S;
            if (type == typeof(double))
                return OpCodes.Ldc_R8;
            if (type == typeof(bool))
                return OpCodes.Ldc_I4;

            throw new ArgumentException("type");
        }
    }
}
