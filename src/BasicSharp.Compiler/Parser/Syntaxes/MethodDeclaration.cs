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
        PredefinedType _returnType;
        public PredefinedType ReturnType
        {
            get { return _returnType; }
            internal set
            {
                if (_returnType != value)
                {
                    _returnType = value;
                    Accept(value);
                }
            }
        }
        public TokenInfo Identifier { get; internal set; }

        ParameterList _parameterList;
        public ParameterList ParameterList
        {
            get { return _parameterList; }
            internal set
            {
                if (_parameterList != value)
                {
                    _parameterList = value;
                    Accept(value);
                }
            }
        }

        BlockStatement _block;
        public BlockStatement Block
        {
            get { return _block; }
            internal set
            {
                if (_block != value)
                {
                    _block = value;
                    Accept(value);
                }
            }
        }

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

        public override System.Collections.IEnumerable GetChilds()
        {
            yield return Modifier;
            yield return ReturnType;
            yield return Identifier;
            yield return ParameterList;
            yield return Block;
        }
    }
}
