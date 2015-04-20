using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.ILEmitter
{
    public class TacUnit
    {
        public string LabelPrefix { get; internal set; }
        public int LabelIndex { get; internal set; }
        public OpCode Op { get; internal set; }
        public string Value { get; internal set; }

        public override string ToString()
        {
            return string.Format("{0,10}{1}: {2,-10} {3}", LabelPrefix, LabelIndex.ToString().PadLeft(4, '0'), Op.Name, Value);
        }
    }
}
