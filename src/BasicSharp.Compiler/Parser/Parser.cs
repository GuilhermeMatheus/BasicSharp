using BasicSharp.Compiler.Lexer;
using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lxr = BasicSharp.Compiler.Lexer;

namespace BasicSharp.Compiler.Parser
{
    public class Parser
    {
        readonly lxr.Lexer lexer;
        
        public Parser(lxr.Lexer lexer)
        {
            this.lexer = lexer;
        }

        public IEnumerable<Syntax> GetSyntaxes()
        {
            throw new NotImplementedException();

            foreach (var item in lexer.GetTokens())
            {
                switch (item.Kind)
                {
                    default:
                        break;
                }
            }
        }
    }
}
