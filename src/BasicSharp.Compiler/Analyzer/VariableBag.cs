﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.Analyzer
{
    public class VariableBag
    {
        readonly List<Variable> knowVariables = new List<Variable>();
        ReadOnlyCollection<Variable> KnowVariables
        {
            get { return knowVariables.AsReadOnly(); }
        }

        public void AddKnowVariable(Variable var)
        {
            this.knowVariables.Add(var);
        }

        public static VariableBag operator +(VariableBag l, VariableBag r)
        {
            if (l == null)
                return r;

            if (r == null)
                return l;

            var result = new VariableBag();
            
            foreach (var item in l.KnowVariables.Concat(r.KnowVariables))
                result.AddKnowVariable(item);

            return result;
        }
    }
}
