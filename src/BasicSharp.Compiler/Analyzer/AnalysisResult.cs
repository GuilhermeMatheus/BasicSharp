using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BasicSharp.Compiler.Analyzer
{
    [DebuggerDisplay("{MessageResult}")]
    public class AnalysisResult
    {
        public SyntaxNode Node { get; internal set; }
        public string MessageResult { get; internal set; }
        public bool Recognized { get; internal set; }

        public static List<AnalysisResult> EmptyList = new List<AnalysisResult>();
        public static AnalysisResult Empty = new AnalysisResult();
    }
}