using BasicSharp.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicSharp.Compiler.Parser.Extensions;

namespace BasicSharp.Compiler.Parser.Syntaxes
{
    public class MethodDeclaration : ModuleMemberDeclaration
    {
        public string Name
        {
            get { return Identifier.StringValue; }
        }
        public int Arity
        {
            get { return ParameterList.Parameters.Count; }
        }
        public override bool IsPublic
        {
            get { return Modifier.Kind == SyntaxKind.EverybodyKeyword; }
        }


        public TokenInfo Modifier { get; internal set; }
        public PredefinedType ReturnType { get; internal set; }
        public TokenInfo Identifier { get; internal set; }

        public ParameterList ParameterList { get; internal set; }

        public Block Block { get; internal set; }

        public override IEnumerable<TokenInfo> GetInternalTokens()
        {
            yield return Modifier;

            foreach (var item in ReturnType.GetTokenEnumerable())
                yield return item;

            yield return Identifier;

            foreach (var item in ParameterList.GetTokenEnumerable())
                yield return item;

            foreach (var item in Block.GetTokenEnumerable())
                yield return item;
        }
    }
}
