using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.Lexer
{
    [DebuggerDisplay("Kind = {Kind},\nStringValue = {StringValue}")]
    public class TokenInfo
    {
        public SyntaxKind Kind { get; set; }
        public int Begin { get; set; }
        public int End { get; set; }
        public bool IsMalformedToken { get; set; }

        public string StringValue { get; set; }
        public char CharValue { get; set; }
        public int IntValue { get; set; }
        public bool BooleanValue { get; set; }
        public byte ByteValue { get; set; }
        public double DoubleValue { get; set; }

        #region Object Overrides
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            return GetHashCode() == obj.GetHashCode();
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                hash = hash * 23 + IsMalformedToken.GetHashCode();
                hash = hash * 23 + Kind.GetHashCode();
                hash = hash * 23 + Begin.GetHashCode();
                hash = hash * 23 + End.GetHashCode();
                hash = hash * 23 + IntValue.GetHashCode();
                hash = hash * 23 + BooleanValue.GetHashCode();
                hash = hash * 23 + ByteValue.GetHashCode();
                hash = hash * 23 + DoubleValue.GetHashCode();

                if (StringValue != null)
                    hash = hash * 23 + StringValue.GetHashCode();
                
                return hash;
            }
        }
        #endregion

    }
}
