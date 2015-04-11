using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicSharp.Compiler.Analizer
{
    public class AnalysisResult
    {
        public SyntaxNode Node { get; internal set; }
        public string MessageResult { get; internal set; }
        public bool Recognized { get; internal set; }
    }
}