using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicSharp.Compiler.Analyzer
{
    public class CodeLens
    {
        Dictionary<SyntaxNode, List<CodeInfo>> syntaxesInfo = new Dictionary<SyntaxNode, List<CodeInfo>>();

        public void AddCodeInfoTo(SyntaxNode syntax, CodeInfo info)
        {
            if (syntaxesInfo.ContainsKey(syntax))
                syntaxesInfo[syntax].Add(info);
            else
                syntaxesInfo.Add(syntax, new List<CodeInfo> { info });
        }

        public IReadOnlyCollection<CodeInfo> GetCodeInfoFrom(SyntaxNode syntax)
        {
            if (!syntaxesInfo.ContainsKey(syntax))
                return null;

            return syntaxesInfo[syntax].AsReadOnly();
        }
    }
}
