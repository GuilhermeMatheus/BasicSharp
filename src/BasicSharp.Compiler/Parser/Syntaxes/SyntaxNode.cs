using BasicSharp.Compiler.Lexer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    [DebuggerDisplay("{AsString()}")]
    public abstract class SyntaxNode : ISyntaxTreeNode
    {
        List<TokenInfo> trivias = new List<TokenInfo>();

        public SyntaxNode Parent { get; protected set; }

        public ReadOnlyCollection<TokenInfo> Trivias
        {
            get { return new ReadOnlyCollection<TokenInfo>(trivias); }
        }

        public ReadOnlyCollection<TokenInfo> Tokens
        {
            get
            {
                var source = from item in Trivias.Concat(GetInternalTokens())
                             where item != null
                             orderby item.Begin
                             select item;

                return source.ToList().AsReadOnly();
            }
        }

        public void AddTrivia(TokenInfo trivia)
        {
            trivias.Add(trivia);
        }

        public abstract IEnumerable GetChilds();
        public abstract IEnumerable<TokenInfo> GetInternalTokens();

        public string AsString()
        {
            var result = string.Empty;

            foreach (var item in Tokens)
                result += item.StringValue;

            return result;
        }

        public IEnumerable Childs
        {
            get
            {
                return GetChilds();
            }
        }

        public string DisplayMember
        {
            get { return GetType().Name; }
        }

        protected void Accept(SyntaxNode node)
        {
            node.Parent = this;
        }
    }
}
