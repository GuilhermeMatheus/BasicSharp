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

            if ((root = getCompilationUnit()) != null)
                return root;

            if ((root = getFieldOrMethodDeclaration()) != null)
                return root;

            if ((root = getExpression()) != null)
                return root;

            throw new NotImplementedException();
        }

        #region CompilerUnit
        CompilationUnit getCompilationUnit()
        {
            var hasImplementsDirective = false;
            var result = new CompilationUnit();

            ImplementsDirective currImplDir;
            while ((currImplDir = getImplementsDirective()) != null)
            {
                hasImplementsDirective = true;
                result.AddImplementsDirective(currImplDir);
                dumpTriviaBufferInto(result);
            }

            if (!hasImplementsDirective)
                return null;

            var module = getModuleDeclaration();
            dumpTriviaBufferInto(result);

            if (module == null)
                throw null;

            result.Module = module;

            return result;
        }

        ImplementsDirective getImplementsDirective()
        {
            var implements = currentToken();

            if (implements.Kind != SyntaxKind.ImplementsDirectiveKeyword)
                return null;

            var result = new ImplementsDirective { ImplementsToken = implements };

            while (consumeCurrentTokenAndGetNext().Kind.IsIn(SyntaxKind.Identifier, SyntaxKind.DotToken))
                result.AddFullClassNamePart(currentToken());

            if (currentToken().Kind != SyntaxKind.SemicolonToken)
                throw null;

            result.Semicolon = currentToken();
            consumeCurrentTokenAndGetNext();
            dumpTriviaBufferInto(result);

            return result;
        }
        ModuleDeclaration getModuleDeclaration()
        {
            var module = currentToken();
            if (module.Kind != SyntaxKind.ModuleKeyword)
                return null;

            var result = new ModuleDeclaration { ModuleToken = module };

            var name = consumeCurrentTokenAndGetNext();
            if (name.Kind != SyntaxKind.Identifier)
                throw null;

            dumpTriviaBufferInto(result);
            result.Name = name;

            dumpTriviaBufferInto(result);

            var openBrace = consumeCurrentTokenAndGetNext();
            if (openBrace.Kind != SyntaxKind.OpenBraceToken)
                throw null;

            result.OpenBrace = openBrace;
            consumeCurrentTokenAndGetNext();

            dumpTriviaBufferInto(result);

            ModuleMemberDeclaration member;
            while ((member = getFieldOrMethodDeclaration()) != null)
            {
                result.AddMember(member);
                dumpTriviaBufferInto(result);
            }

            var closeBrace = consumeCurrentTokenAndGetNext();
            if (closeBrace.Kind != SyntaxKind.CloseBraceToken)
                throw null;

            result.CloseBrace = closeBrace;
            consumeCurrentTokenAndGetNext();

            return result;
        }

        #endregion

        #region FieldOrMethodDeclaration
        ModuleMemberDeclaration getFieldOrMethodDeclaration()
        {
            var modifier = currentToken();
            if (!modifier.Kind.IsModifier())
                return null;

            consumeCurrentTokenAndGetNext();
            var type = getPredefinedType();
            if (type == null)
                throw null; //Type esperado, etc etc

            var identifier = currentToken();
            if (identifier.Kind != SyntaxKind.Identifier)
                throw null; //Type esperado, etc etc

            consumeCurrentTokenAndGetNext();
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
        PredefinedType getPredefinedType()
        {
            var type = currentToken();

            if (!type.Kind.IsTypeNotContextual())
                return null;

            var result = new PredefinedType { TypeToken = type };

            var openBracket = consumeCurrentTokenAndGetNext();
            if (openBracket.Kind == SyntaxKind.OpenBracketToken)
            {
                consumeCurrentTokenAndGetNext();

                var expression = getExpression();
                var closeBracket = currentToken();
                if (closeBracket.Kind != SyntaxKind.CloseBracketToken)
                    throw null;

                result.ArraySpecifier = new ArrayRankSpecifier
                {
                    OpenBracket = openBracket,
                    Value = expression,
                    CloseBracket = closeBracket
                };

                consumeCurrentTokenAndGetNext();
            }

            return result;
        }

        FieldDeclaration getFieldDeclaration(TokenInfo modifier, PredefinedType type, TokenInfo identifier)
        {
            var result = new FieldDeclaration { Modifier = modifier };

            var variableDeclaration = VariableDeclaration.WithDeclarator(getVariableDeclarator<ConstantAssignmentExpression>(identifier));
            variableDeclaration.Type = type;
            result.Declaration = variableDeclaration;

            var current = currentToken();
            if (current.Kind == SyntaxKind.SemicolonToken)
            {
                dumpTriviaBufferInto(variableDeclaration);
                consumeCurrentTokenAndGetNext();
                result.Semicolon = current;
                return result;
            }

            dumpTriviaBufferInto(variableDeclaration);
            if (current.Kind != SyntaxKind.CommaToken)
                throw null;

            variableDeclaration.AddTrivia(currentToken());
            consumeCurrentTokenAndGetNext();
            VariableDeclarator<ConstantAssignmentExpression> varDecl;
            while ((varDecl = getVariableDeclarator<ConstantAssignmentExpression>()) != null)
            {
                dumpTriviaBufferInto(variableDeclaration);
                variableDeclaration.AddDeclarator(varDecl);

                if (currentToken().Kind == SyntaxKind.CommaToken)
                {
                    variableDeclaration.AddTrivia(currentToken());
                    consumeCurrentTokenAndGetNext();
                }
            }

            if (currentToken().Kind != SyntaxKind.SemicolonToken)
                throw null;

            result.Semicolon = currentToken();

            consumeCurrentTokenAndGetNext();
            return result;
        }
        VariableDeclarator<T> getVariableDeclarator<T>()
            where T : AssignmentExpression
        {
            var current = currentToken();

            if (current.Kind != SyntaxKind.Identifier)
                return null;

            consumeCurrentTokenAndGetNext();
            return getVariableDeclarator<T>(current);
        }

        VariableDeclarator<T> getVariableDeclarator<T>(TokenInfo identifier)
            where T : AssignmentExpression
        {
            var current = currentToken();

            if (current.Kind.IsIn(SyntaxKind.CommaToken, SyntaxKind.SemicolonToken))
                return new VariableDeclarator<T> { Identifier = identifier };

            if (current.Kind == SyntaxKind.EqualsToken)
                return new VariableDeclarator<T> { Identifier = identifier, Assignment = getAssignmentExpression<T>(current) };

            throw null;
        }
        T getAssignmentExpression<T>(TokenInfo assignmentOperatorInfo)
            where T : AssignmentExpression
        {
            if (typeof(T) == typeof(ConstantAssignmentExpression))
                throw null;

            if (typeof(T) == typeof(AssignmentExpression))
                throw null;

            throw null;
        }

        ParameterList getParameterList()
        {
            var openParen = currentToken();
            if (openParen.Kind != SyntaxKind.OpenParenToken)
                return null;

            var result = new ParameterList { OpenParenToken = openParen };
            consumeCurrentTokenAndGetNext();

            Parameter currParam;
            while ((currParam = getParameter()) != null)
            {
                dumpTriviaBufferInto(result);
                result.AddParameter(currParam);

                var comma = currentToken();
                if (comma.Kind == SyntaxKind.CommaToken)
                {
                    dumpTriviaBufferInto(result);
                    result.AddTrivia(comma);
                    consumeCurrentTokenAndGetNext();
                }
            }

            var closeParen = currentToken();
            if (closeParen.Kind != SyntaxKind.CloseParenToken)
                throw null;

            dumpTriviaBufferInto(result);
            result.CloseParenToken = closeParen;

            consumeCurrentTokenAndGetNext();
            return result;
        }
        Parameter getParameter()
        {
            var type = currentToken();
            if (!type.Kind.IsTypeNotContextual())
                return null;

            var identifier = consumeCurrentTokenAndGetNext();
            if (identifier.Kind != SyntaxKind.Identifier)
                throw null;

            consumeCurrentTokenAndGetNext();
            var result = new Parameter { Type = type, Identifier = identifier };
            dumpTriviaBufferInto(result);

            return result;
        }
        //MethodDeclaration ::=  Modifier? Method_Type Identifier Parameters_List Block
        MethodDeclaration getMethod()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Block members
        Block getBlock()
        {
            var openBrace = currentToken();
            if (openBrace.Kind != SyntaxKind.OpenBraceToken)
                return null;

            TokenInfo closeBrace;
            while ((closeBrace = consumeCurrentTokenAndGetNext()).Kind != SyntaxKind.CloseBraceToken) { }

            consumeCurrentTokenAndGetNext();
            return new Block { OpenBrace = openBrace, CloseBrace = closeBrace };
        }

        LocalVariableDeclaration getLocalVariableDeclaration()
        {
            var type = getPredefinedType();
            if (type == null)
                return null;
            return null;


        }
        #endregion

        #region Expressions
        Expression getExpression()
        {
            Expression root;

            if ((root = getArithmeticBinaryExpression()) != null)
                return root;

            return new ConstantAssignmentExpression();
        }

        Expression getArithmeticBinaryExpression()
        {
            Expression root = getUnaryExpression();

            if (root == null)
                return null;

            Expression aux = null;
            while(true)
            {
                if ((aux = getAdditiveExpression(root)) != root)
                    root = aux;
                else if ((aux = getMultiplicativeExpression(root)) != root)
                    root = aux;
                else if ((aux = getModExpression(root)) != root)
                    root = aux;
                else
                    break;
            }

            return root;
        }

        Expression getUnaryExpression()
        {
            return getLiteralExpression() ?? getInvocationOrAccessorExpression() ?? getSignedUnaryExpression();
        }

        ParenthesedExpression getParenthesedExpression()
        {
            var openParen = currentToken();
            if (openParen.Kind != SyntaxKind.OpenParenToken)
                return null;

            consumeCurrentTokenAndGetNext();
            var innerExpression = getExpression();

            if (innerExpression == null)
                throw null;

            var closeParen = currentToken();
            if (closeParen.Kind != SyntaxKind.CloseParenToken)
                throw null;

            consumeCurrentTokenAndGetNext();
            return new ParenthesedExpression
            {
                OpenParenToken = openParen,
                InnerExpression = innerExpression,
                CloseParenToken = closeParen
            };
        }

        Expression getInvocationOrAccessorExpression()
        {
            var identifier = currentToken();
            if (identifier.Kind != SyntaxKind.Identifier)
                return null;

            var openParen = consumeCurrentTokenAndGetNext();
            if (openParen.Kind != SyntaxKind.OpenParenToken)
                return new AccessorExpression { Identifier = identifier };

            consumeCurrentTokenAndGetNext();

            var result = new InvocationExpression { MethodName = identifier };
            dumpTriviaBufferInto(result);

            var arguments = new ArgumentList { OpenParen = openParen };
            result.Arguments = arguments;

            TokenInfo c;
            while ((c = currentToken()).Kind != SyntaxKind.CloseParenToken)
            {
                dumpTriviaBufferInto(arguments);

                var expr = getExpression();
                if (expr != null)
                {
                    arguments.AddArgument(new Argument { Expression = expr });
                    if (currentToken().Kind == SyntaxKind.CommaToken)
                    {
                        arguments.AddTrivia(currentToken());
                        consumeCurrentTokenAndGetNext();
                    }
                }
            }

            arguments.CloseParen = currentToken();
            dumpTriviaBufferInto(arguments);

            consumeCurrentTokenAndGetNext();

            return result;
        }

        Expression getAdditiveExpression(Expression leftSide)
        {
            leftSide = leftSide ?? getUnaryExpression();

            dumpTriviaBufferInto(leftSide);

            var op = currentToken();

            if (!op.Kind.IsIn(SyntaxKind.PlusToken, SyntaxKind.MinusToken))
                return leftSide;

            consumeCurrentTokenAndGetNext();

            var rightSide = getModExpression(null);

            return new BinaryExpression
            {
                LeftSide = leftSide,
                OperatorToken = op,
                RightSide = rightSide
            };
        }

        Expression getMultiplicativeExpression(Expression leftSide)
        {
            leftSide = leftSide ?? getAdditiveExpression(null);

            dumpTriviaBufferInto(leftSide);

            var op = currentToken();

            if (!op.Kind.IsIn(SyntaxKind.AsteriskToken, SyntaxKind.SlashToken))
                return leftSide;

            consumeCurrentTokenAndGetNext();

            var rightSide = getModExpression(null);

            return new BinaryExpression
            {
                LeftSide = leftSide,
                OperatorToken = op,
                RightSide = rightSide
            };
        }

        Expression getModExpression(Expression leftSide)
        {
            leftSide = leftSide ?? getMultiplicativeExpression(null);

            dumpTriviaBufferInto(leftSide);

            var op = currentToken();

            if (!op.Kind.IsIn(SyntaxKind.ModToken))
                return leftSide;

            consumeCurrentTokenAndGetNext();

            var rightSide = getUnaryExpression();

            return new BinaryExpression
            {
                LeftSide = leftSide,
                OperatorToken = op,
                RightSide = rightSide
            };
        }

        Expression getSignedUnaryExpression()
        {
            var signal = currentToken();
            
            if (signal.Kind.IsIn(SyntaxKind.MinusToken, SyntaxKind.PlusToken))
            {
                consumeCurrentTokenAndGetNext();
                return new UnaryExpression { SignalToken = signal, Expression = getExpression() };
            }

            return getLiteralExpression() ?? getInvocationOrAccessorExpression() ?? getParenthesedExpression() as Expression;
        }

        LiteralExpression getLiteralExpression()
        {
            var value = currentToken();

            if (!value.Kind.IsLiteral())
                return null;

            consumeCurrentTokenAndGetNext();

            var result = new LiteralExpression { Value = value };
            dumpTriviaBufferInto(result);
            
            return result;
        }
        #endregion

        void dumpTriviaBufferInto(SyntaxNode node)
        {
            while (triviaBuffer.Count > 0)
                node.AddTrivia(triviaBuffer.Dequeue());
        }
        TokenInfo currentToken()
        {
            if (enumerator.Current.Kind.IsTrivia())
                return consumeCurrentTokenAndGetNext();

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