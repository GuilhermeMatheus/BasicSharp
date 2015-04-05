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
            : this(lexer.GetTokens().GetEnumerator())
        {
            this.lexer = lexer;
        }

        public Parser(IEnumerator<TokenInfo> enumerator)
        {
            this.enumerator = enumerator;
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
                dumpTrivia(result);
            }

            if (!hasImplementsDirective)
                return null;

            var module = getModuleDeclaration();
            dumpTrivia(result);

            if (module == null)
                handleError();

            result.Module = module;

            return result;
        }

        ImplementsDirective getImplementsDirective()
        {
            var implements = currentToken();

            if (implements.Kind != SyntaxKind.ImplementsDirectiveKeyword)
                return null;

            var result = new ImplementsDirective { ImplementsToken = implements };

            while (moveNextToken().Kind.IsIn(SyntaxKind.Identifier, SyntaxKind.DotToken))
                result.AddFullClassNamePart(currentToken());

            if (currentToken().Kind != SyntaxKind.SemicolonToken)
                handleError();

            result.SemicolonToken = currentToken();
            moveNextToken();
            dumpTrivia(result);

            return result;
        }
        ModuleDeclaration getModuleDeclaration()
        {
            var module = currentToken();
            if (module.Kind != SyntaxKind.ModuleKeyword)
                return null;

            var result = new ModuleDeclaration { ModuleToken = module };

            var name = moveNextToken();
            if (name.Kind != SyntaxKind.Identifier)
                handleError();

            dumpTrivia(result);
            result.Name = name;

            dumpTrivia(result);

            var openBrace = moveNextToken();
            if (openBrace.Kind != SyntaxKind.OpenBraceToken)
                handleError();

            result.OpenBraceToken = openBrace;
            moveNextToken();

            dumpTrivia(result);

            ModuleMemberDeclaration member;
            while ((member = getFieldOrMethodDeclaration()) != null)
            {
                result.AddMember(member);
                dumpTrivia(result);
            }

            var closeBrace = moveNextToken();
            if (closeBrace.Kind != SyntaxKind.CloseBraceToken)
                handleError();

            result.CloseBraceToken = closeBrace;
            moveNextToken();

            return result;
        }

        #endregion

        #region FieldOrMethodDeclaration
        ModuleMemberDeclaration getFieldOrMethodDeclaration()
        {
            var modifier = currentToken();
            if (!modifier.Kind.IsModifier())
                return null;

            moveNextToken();
            var type = getPredefinedType();
            if (type == null)
                handleError(); //Type esperado, etc etc

            var identifier = currentToken();
            if (identifier.Kind != SyntaxKind.Identifier)
                handleError(); //Type esperado, etc etc

            moveNextToken();
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

            var openBracket = moveNextToken();
            if (openBracket.Kind == SyntaxKind.OpenBracketToken)
            {
                moveNextToken();

                var expression = getExpression();
                var closeBracket = currentToken();
                if (closeBracket.Kind != SyntaxKind.CloseBracketToken)
                    handleError();

                result.ArraySpecifier = new ArrayRankSpecifier
                {
                    OpenBracketToken = openBracket,
                    Value = expression,
                    CloseBracketToken = closeBracket
                };

                moveNextToken();
            }

            return result;
        }

        FieldDeclaration getFieldDeclaration(TokenInfo modifier, PredefinedType type, TokenInfo identifier)
        {
            var result = new FieldDeclaration { Modifier = modifier };

            var variableDeclaration = VariableDeclaration.WithDeclarator(getVariableDeclarator(identifier));
            variableDeclaration.Type = type;
            result.Declaration = variableDeclaration;

            var current = currentToken();
            if (current.Kind == SyntaxKind.SemicolonToken)
            {
                dumpTrivia(variableDeclaration);
                moveNextToken();
                result.SemicolonToken = current;
                return result;
            }

            dumpTrivia(variableDeclaration);
            if (current.Kind != SyntaxKind.CommaToken)
                handleError();

            variableDeclaration.AddTrivia(currentToken());
            moveNextToken();
            VariableDeclarator varDecl;
            while ((varDecl = getVariableDeclarator()) != null)
            {
                dumpTrivia(variableDeclaration);
                variableDeclaration.AddDeclarator(varDecl);

                if (currentToken().Kind == SyntaxKind.CommaToken)
                {
                    variableDeclaration.AddTrivia(currentToken());
                    moveNextToken();
                }
            }

            if (currentToken().Kind != SyntaxKind.SemicolonToken)
                handleError();

            result.SemicolonToken = currentToken();

            moveNextToken();
            return result;
        }
        VariableDeclarator getVariableDeclarator()
        {
            var current = currentToken();

            if (current.Kind != SyntaxKind.Identifier)
                return null;

            moveNextToken();
            return getVariableDeclarator(current);
        }

        VariableDeclarator getVariableDeclarator(TokenInfo identifier)
        {
            var current = currentToken();

            if (current.Kind.IsIn(SyntaxKind.CommaToken, SyntaxKind.SemicolonToken))
                return new VariableDeclarator { Identifier = identifier };

            if (current.Kind == SyntaxKind.EqualsToken)
                return new VariableDeclarator { Identifier = identifier, Assignment = getAssignmentExpression(current) };

            handleError();
            return null;
        }
        AssignmentExpression getAssignmentExpression(TokenInfo assignmentOperatorInfo)
        {
            handleError();
            return null;
        }

        ParameterList getParameterList()
        {
            var openParen = currentToken();
            if (openParen.Kind != SyntaxKind.OpenParenToken)
                return null;

            var result = new ParameterList { OpenParenToken = openParen };
            moveNextToken();

            Parameter currParam;
            while ((currParam = getParameter()) != null)
            {
                dumpTrivia(result);
                result.AddParameter(currParam);

                var comma = currentToken();
                if (comma.Kind == SyntaxKind.CommaToken)
                {
                    dumpTrivia(result);
                    result.AddTrivia(comma);
                    moveNextToken();
                }
            }

            var closeParen = currentToken();
            if (closeParen.Kind != SyntaxKind.CloseParenToken)
                handleError();

            dumpTrivia(result);
            result.CloseParenToken = closeParen;

            moveNextToken();
            return result;
        }
        Parameter getParameter()
        {
            var type = currentToken();
            if (!type.Kind.IsTypeNotContextual())
                return null;

            var identifier = moveNextToken();
            if (identifier.Kind != SyntaxKind.Identifier)
                handleError();

            moveNextToken();
            var result = new Parameter { Type = type, Identifier = identifier };
            dumpTrivia(result);

            return result;
        }
        
        MethodDeclaration getMethod()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Block members
        BlockStatement getBlock()
        {
            var openBrace = currentToken();
            if (openBrace.Kind != SyntaxKind.OpenBraceToken)
                return null;

            var result = new BlockStatement { OpenBraceToken = openBrace };
            moveNextToken();
            dumpTrivia(result);

            Statement stmt;
            while ((stmt = getStatement()) != null)
                result.AddStatement(stmt);

            TokenInfo closeBrace = currentToken();
            dumpTrivia(result);

            moveNextToken();

            if (closeBrace.Kind != SyntaxKind.CloseBraceToken)
                handleError();

            result.CloseBraceToken = closeBrace;

            return result;
        }

        Statement getStatement()
        {
            return (Statement) getMethodInvocationStatement() ?? 
                   (Statement) getReturnStatement() ??
                   (Statement) getBreakStatement() ??
                   (Statement) getIfStatement() ??
                   (Statement) getForStatement() ??
                   (Statement) getWhileStatement();
        }

        MethodInvocationStatement getMethodInvocationStatement()
        {
            var methodInvocation = getMethodInvocationExpression();
            if (methodInvocation == null)
                return null;

            var semicolon = currentToken();
            if (semicolon.Kind != SyntaxKind.SemicolonToken)
                handleError();

            moveNextToken();
            var result = new MethodInvocationStatement
            {
                MethodInvocation = methodInvocation,
                SemicolonToken = semicolon
            };

            dumpTrivia(result);

            return result;
        }

        ReturnStatement getReturnStatement()
        {
            var returnToken = currentToken();
            if (returnToken.Kind != SyntaxKind.ReturnKeyword)
                return null;

            var result = new ReturnStatement { ReturnToken = returnToken };
            
            moveNextToken();
            dumpTrivia(result);

            result.Expression = getExpression();

            var semicolon = currentToken();
            if (semicolon.Kind != SyntaxKind.SemicolonToken)
                handleError();

            moveNextToken();
            result.SemicolonToken = semicolon;
            dumpTrivia(result);
            return result;
        }

        BreakStatement getBreakStatement()
        {
            var breakToken = currentToken();
            if (breakToken.Kind != SyntaxKind.BreakKeyword)
                return null;

            var result = new BreakStatement { BreakToken = breakToken };
            
            dumpTrivia(result);

            var semicolon = moveNextToken();
            if (semicolon.Kind != SyntaxKind.SemicolonToken)
                handleError();

            moveNextToken();
            dumpTrivia(result);

            result.SemicolonToken = semicolon;

            return result;

        }

        IfStatement getIfStatement()
        {
            var ifToken = currentToken();
            if (ifToken.Kind != SyntaxKind.IfKeyword)
                return null;

            var result = new IfStatement { IfToken = ifToken };

            var openParen = moveNextToken();
            if (openParen.Kind != SyntaxKind.OpenParenToken)
                handleError();
            
            result.OpenParenToken = openParen;

            dumpTrivia(result);
            moveNextToken();
            result.Condition = getExpression();

            var closeParen = currentToken();
            if (closeParen.Kind != SyntaxKind.CloseParenToken)
                handleError();

            result.CloseParenToken = closeParen;

            dumpTrivia(result);
            moveNextToken();
            result.Then = getBlock();

            var elseToken = currentToken();
            if (elseToken.Kind == SyntaxKind.ElseKeyword)
            {
                result.ElseToken = elseToken;

                dumpTrivia(result);
                
                moveNextToken();

                result.Else = getBlock();
            }

            return result;
        }

        ForStatement getForStatement()
        {
            var forToken = currentToken();
            if (forToken.Kind != SyntaxKind.ForKeyword)
                return null;

            var result = new ForStatement { ForToken = forToken };
            dumpTrivia(result);

            var openParen = moveNextToken();
            if (openParen.Kind != SyntaxKind.OpenParenToken)
                handleError();

            moveNextToken();

            result.OpenParenToken = openParen;
            dumpTrivia(result);

            result.Initializer = getExpression();
            var firstSemicolon = currentToken();
            if (firstSemicolon.Kind != SyntaxKind.SemicolonToken)
                handleError();

            result.FirstSemicolonToken = firstSemicolon;
            dumpTrivia(result);

            moveNextToken();
            result.Condition = getExpression();
            var secondSemicolon = currentToken();
            if (secondSemicolon.Kind != SyntaxKind.SemicolonToken)
                handleError();

            moveNextToken();
            result.SecondSemicolonToken = secondSemicolon;
            dumpTrivia(result);

            result.Incrementor = getExpression();

            result.Block = getBlock();

            return result;
        }

        WhileStatement getWhileStatement()
        {
            var whileToken = currentToken();
            if (whileToken.Kind != SyntaxKind.WhileKeyword)
                return null;

            var result = new WhileStatement { WhileToken = whileToken };

            var openParen = moveNextToken();
            if (openParen.Kind != SyntaxKind.OpenParenToken)
                handleError();

            result.OpenParenToken = openParen;

            dumpTrivia(result);
            moveNextToken();
            result.Condition = getExpression();

            var closeParen = currentToken();
            if (closeParen.Kind != SyntaxKind.CloseParenToken)
                handleError();

            result.CloseParenToken = closeParen;

            dumpTrivia(result);
            moveNextToken();
            result.Block = getBlock();

            return result;
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

            //if((root = getBooleanExpression()) != null)
            //    return root;

            if ((root = getArithmeticBinaryExpression()) != null)
                return root;

            return null;
        }

        //UNDONE: Precedence is not respected
        Expression getArithmeticBinaryExpression()
        {
            Expression root = getAdditiveExpression(null), aux = null;

            if (root == null)
                return null;

            while(true)
            {
                if ((aux = getAdditiveExpression(root)) != root) {
                    root = aux;
                    continue;
                }
                else
                    break;
            }

            return root;
        }

        Expression getBooleanExpression()
        {
            var result = getArithmeticBinaryExpression() ?? getBooleanLiteralOrAccessor();
            if (result == null)
                return null;

            TokenInfo c;
            while ((c = currentToken()).Kind != SyntaxKind.SemicolonToken && c.Kind != SyntaxKind.None)
            {
                if (c.Kind.IsLogicalOperator())
                {
                    moveNextToken();
                    var rightSide = getArithmeticBinaryExpression() ?? getBooleanLiteralOrAccessor();

                    if (rightSide == null)
                        handleError();

                    result = new BinaryExpression
                    {
                        LeftSide = result,
                        OperatorToken = c,
                        RightSide = rightSide
                    };
                }
            }

            return result;
        }
        Expression getBooleanLiteralOrAccessor()
        {
            Expression result;

            var tknCurrent = currentToken();
            if (tknCurrent.Kind.IsIn(SyntaxKind.FalseKeyword, SyntaxKind.TrueKeyword))
            {
                result = new LiteralExpression { Value = tknCurrent };
                moveNextToken();
            }
            else
                result = getMethodInvocationOrAccessorExpression();

            return result;
        }

        Expression getUnaryExpression()
        {
            return getNumericLiteralExpression() ?? getMethodInvocationOrAccessorExpression() ?? getSignedUnaryExpression();
        }

        ParenthesedExpression getParenthesedExpression()
        {
            var openParen = currentToken();
            if (openParen.Kind != SyntaxKind.OpenParenToken)
                return null;

            moveNextToken();
            var innerExpression = getExpression();

            if (innerExpression == null)
                handleError();

            var closeParen = currentToken();
            if (closeParen.Kind != SyntaxKind.CloseParenToken)
                handleError();

            moveNextToken();
            return new ParenthesedExpression
            {
                OpenParenToken = openParen,
                InnerExpression = innerExpression,
                CloseParenToken = closeParen
            };
        }

        Expression getMethodInvocationOrAccessorExpression()
        {
            var identifier = currentToken();
            if (identifier.Kind != SyntaxKind.Identifier)
                return null;

            var openParen = moveNextToken();
            if (openParen.Kind != SyntaxKind.OpenParenToken)
            {
                var result = new AccessorExpression { Identifier = identifier };
                dumpTrivia(result);
                return result;
            }

            moveNextToken();
            return getMethodInvocationExpression(identifier, openParen);
        }

        MethodInvocationExpression getMethodInvocationExpression(TokenInfo identifier = null, TokenInfo openParen = null)
        {
            if (identifier == null && openParen == null)
            {
                identifier = currentToken();
                if (identifier.Kind != SyntaxKind.Identifier)
                    return null;

                openParen = moveNextToken();
                if (openParen.Kind != SyntaxKind.OpenParenToken)
                    handleError();

                moveNextToken();
            }

            var result = new MethodInvocationExpression { MethodName = identifier };
            dumpTrivia(result);

            var arguments = new ArgumentList { OpenParenToken = openParen };
            result.Arguments = arguments;

            TokenInfo c;
            while ((c = currentToken()).Kind != SyntaxKind.CloseParenToken)
            {
                dumpTrivia(arguments);

                var expr = getExpression();
                if (expr != null)
                {
                    arguments.AddArgument(new Argument { Expression = expr });
                    if (currentToken().Kind == SyntaxKind.CommaToken)
                    {
                        arguments.AddTrivia(currentToken());
                        moveNextToken();
                    }
                }
            }

            arguments.CloseParenToken = currentToken();
            dumpTrivia(arguments);

            moveNextToken();

            return result;
        }
        
        Expression getAdditiveExpression(Expression leftSide)
        {
            leftSide = leftSide ?? getMultiplicativeExpression(null);

            if (leftSide == null)
                return null;

            dumpTrivia(leftSide);
            var op = currentToken();

            if (!op.Kind.IsIn(SyntaxKind.PlusToken, SyntaxKind.MinusToken))
                return getMultiplicativeExpression(leftSide);

            moveNextToken();

            var rightSide = getMultiplicativeExpression(null);

            return new BinaryExpression
            {
                LeftSide = leftSide,
                OperatorToken = op,
                RightSide = rightSide
            };
        }

        Expression getMultiplicativeExpression(Expression leftSide)
        {
            leftSide = leftSide ?? getModExpression(null);

            if (leftSide == null)
                return null;

            dumpTrivia(leftSide);

            var op = currentToken();

            if (!op.Kind.IsIn(SyntaxKind.AsteriskToken, SyntaxKind.SlashToken))
                return getModExpression(leftSide);

            moveNextToken();

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
            leftSide = leftSide ?? getUnaryExpression();
            
            if (leftSide == null)
                return null;
            
            dumpTrivia(leftSide);

            var op = currentToken();

            if (!op.Kind.IsIn(SyntaxKind.ModOperator))
                return leftSide;

            moveNextToken();

            var rightSide = getModExpression(null);

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
                moveNextToken();
                return new UnaryExpression { SignalToken = signal, Expression = getNumericLiteralExpression() ?? getMethodInvocationOrAccessorExpression() ?? getParenthesedExpression() as Expression };
            }

            return getNumericLiteralExpression() ?? getMethodInvocationOrAccessorExpression() ?? getParenthesedExpression() as Expression;
        }

        LiteralExpression getNumericLiteralExpression()
        {
            var value = currentToken();

            if (!value.Kind.IsNumericLiteral())
                return null;

            moveNextToken();

            var result = new LiteralExpression { Value = value };
            dumpTrivia(result);
            
            return result;
        }
        #endregion

        void dumpTrivia(SyntaxNode node)
        {
            while (triviaBuffer.Count > 0)
                node.AddTrivia(triviaBuffer.Dequeue());
        }
        TokenInfo currentToken()
        {
            if (enumerator.Current == null)
                return new TokenInfo { Kind = SyntaxKind.None };

            if (enumerator.Current.Kind.IsTrivia())
                return moveNextToken();

            return enumerator.Current;
        }
        TokenInfo moveNextToken()
        {
            var hasNext = true;
            while (hasNext = enumerator.MoveNext())
            {
                if (enumerator.Current.Kind.IsTrivia())
                    triviaBuffer.Enqueue(enumerator.Current);
                else
                    break;
            }

            if (!hasNext)
                return new TokenInfo { Kind = SyntaxKind.None };

            return enumerator.Current;
        }

        //TODO: Implements Error Handler
        void handleError()
        {
            throw new Exception();
        }
    }
}