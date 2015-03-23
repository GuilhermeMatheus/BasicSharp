using BasicSharp.Compiler.Lexer;
using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicSharp.Compiler.Parser.Extensions;
using lxr = BasicSharp.Compiler.Lexer;

namespace BasicSharp.Compiler.Parser
{
    public class Parser
    {
        readonly lxr.Lexer lexer;
        readonly IEnumerator<TokenInfo> enumerator;
        readonly Queue<TokenInfo> triviaBuffer;

        public Parser(lxr.Lexer lexer)
        {
            this.lexer = lexer;
            this.enumerator = lexer.GetTokens().GetEnumerator();
            this.triviaBuffer = new Queue<TokenInfo>();
        }

        public SyntaxNode GetSyntax()
        {
            enumerator.MoveNext();
            SyntaxNode root;

            if ((root = getFieldOrMethodDeclaration()) != null)
                return root;

            throw new NotImplementedException();
        }

        #region Syntax Reduces
        SyntaxNode getFieldOrMethodDeclaration()
        {
            var modifier = currentToken();
            if (!modifier.Kind.IsModifier())
                return null;

            var type = consumeCurrentTokenAndGetNext();
            if (!type.Kind.IsNotContextualType())
                throw null; //Type esperado, etc etc

            var identifier = consumeCurrentTokenAndGetNext();
            if (identifier.Kind != SyntaxKind.Identifier)
                throw null; //Type esperado, etc etc

            var parameterList = getParameterList();
            if (parameterList != null)
                return new MethodDeclaration
                {
                    Modifier = modifier,
                    ReturnType = type,
                    Identifier = identifier,
                    ParameterList = parameterList,
                    Block = getBlock()
                };

            return getFieldDeclaration(modifier, type, identifier);
        }

        private FieldDeclaration getFieldDeclaration(TokenInfo modifier, TokenInfo type, TokenInfo identifier)
        {
            var result = new FieldDeclaration { Modifier = modifier };

            var variableDeclaration = VariableDeclaration.WithDeclarator(getVariableDeclarator(identifier));
            variableDeclaration.Type = type;

            var current = currentToken();
            if (current.Kind == SyntaxKind.SemicolonToken) {
                result.SemiColon = consumeCurrentTokenAndGetNext();
                return result;
            }

            dumpTriviaBufferInto(variableDeclaration);
            if (current.Kind != SyntaxKind.CommaToken)
                throw null;

            VariableDeclarator varDecl;
            while ((varDecl = getVariableDeclarator()) != null) {
                dumpTriviaBufferInto(variableDeclaration);
                variableDeclaration.AddDeclarator(varDecl);
            }

            current = consumeCurrentTokenAndGetNext();
            if (current.Kind != SyntaxKind.SemicolonToken)
                throw null;

            result.SemiColon = current;

            return result;
        }
        VariableDeclaration getVariableDeclaration()
        {
            throw new NotImplementedException();
        }
        VariableDeclarator getVariableDeclarator()
        {
            var current = currentToken();
            if (current.Kind == SyntaxKind.Identifier)
                return getVariableDeclarator(current);

            return null;
        }
        VariableDeclarator getVariableDeclarator(TokenInfo identifier)
        {
            var current = currentToken();

            if (current.Kind == SyntaxKind.CommaToken)
                return new VariableDeclarator { Identifier = identifier };

            if (current.Kind == SyntaxKind.EqualsToken)
                return new VariableDeclarator { Identifier = identifier, Assignment = getAssignmentExpression() };

            throw null;
        }
        AssignmentExpression getAssignmentExpression()
        {
            throw new NotImplementedException();
        }

        //MethodDeclaration ::=  Modifier? Method_Type Identifier Parameters_List Block
        MethodDeclaration getMethod()
        {
            throw new NotImplementedException();
        }
        Block getBlock()
        {
            throw new NotImplementedException();
        }
        ParameterList getParameterList()
        {
            throw new NotImplementedException();
        }
        Parameter getParameter()
        {
            throw new NotImplementedException();
        }
        #endregion

        void dumpTriviaBufferInto(SyntaxNode node)
        {
            while(triviaBuffer.Count > 0)
                node.AddTrivia(triviaBuffer.Dequeue());
        }
        TokenInfo currentToken()
        {
            return enumerator.Current;
        }
        TokenInfo consumeCurrentTokenAndGetNext()
        {
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Kind.IsTrivia())
                    triviaBuffer.Enqueue(enumerator.Current);
                else
                    break;
            }

            return enumerator.Current;
        }

    }
}