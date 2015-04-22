using BasicSharp.Compiler.Lexer;
using BasicSharp.Compiler.Parser.Syntaxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicSharp.Compiler.Parser.Extensions;
using lxr = BasicSharp.Compiler.Lexer;
using System.Collections.ObjectModel;

namespace BasicSharp.Compiler.Parser
{
    public class Parser
    {
        readonly lxr.Lexer lexer;
        readonly IEnumerator<TokenInfo> enumerator;
        readonly Queue<TokenInfo> triviaBuffer;

        List<SyntacticException> _syntacticErrors;
        public ReadOnlyCollection<SyntacticException> SyntacticErrors
        {
            get { return _syntacticErrors.AsReadOnly(); }
        }

        public Parser(lxr.Lexer lexer)
            : this(lexer.GetTokens().GetEnumerator())
        {
            this.lexer = lexer;
        }

        public Parser(IEnumerator<TokenInfo> enumerator)
        {
            this.enumerator = enumerator;
            this.triviaBuffer = new Queue<TokenInfo>();
            this._syntacticErrors = new List<SyntacticException>();
        }

        public SyntaxNode GetSyntax()
        {
            enumerator.MoveNext();

            var result = getCompilationUnit();

            if (currentToken().Kind != SyntaxKind.None)
                handleError(SyntacticExceptions.NotExpectedToken(currentToken(), result));

            return result;
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
                handleError(SyntacticExceptions.ExpectedTokenNotFound(result, SyntaxKind.ModuleKeyword));

            result.Module = module;

            return result;
        }

        ImplementsDirective getImplementsDirective()
        {
            var implements = currentToken();

            if (implements.Kind != SyntaxKind.ImplementsDirectiveKeyword)
                return null;

            var result = new ImplementsDirective { ImplementsToken = implements };

            var expectingDot = true;
            while (moveNextToken().Kind == ((expectingDot = !expectingDot) ? SyntaxKind.DotToken : SyntaxKind.Identifier))
                result.AddFullClassNamePart(currentToken());

            if (currentToken().Kind != SyntaxKind.SemicolonToken)
                handleError(SyntacticExceptions.ExpectedTokenNotFound(result, SyntaxKind.SemicolonToken));

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
                handleError(SyntacticExceptions.ExpectedTokenNotFound(result, SyntaxKind.Identifier));

            dumpTrivia(result);
            result.Name = name;

            dumpTrivia(result);

            var openBrace = moveNextToken();
            if (openBrace.Kind != SyntaxKind.OpenBraceToken)
                handleError(SyntacticExceptions.ExpectedTokenNotFound(result, SyntaxKind.OpenBraceToken));

            result.OpenBraceToken = openBrace;
            moveNextToken();

            dumpTrivia(result);

            ModuleMemberDeclaration member;
            while ((member = getFieldOrMethodDeclaration()) != null)
            {
                result.AddMember(member);
                dumpTrivia(result);
            }

            var closeBrace = currentToken();
            if (closeBrace.Kind != SyntaxKind.CloseBraceToken)
                handleError(SyntacticExceptions.ExpectedTokenNotFound(result, SyntaxKind.CloseBraceToken));

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
                handleError(SyntacticExceptions.ExpectedTokenNotFound(currentToken().Line, currentToken().EndColumn, "predefinedType")); //Type esperado, etc etc

            var identifier = currentToken();
            if (identifier.Kind != SyntaxKind.Identifier)
                handleError(SyntacticExceptions.ExpectedTokenNotFound(currentToken().Line, currentToken().EndColumn, SyntaxKind.Identifier));

            moveNextToken();
            var parameterList = getParameterList();
            if (parameterList != null)
            {
                var block = getBlock();
                if (block == null)
                    handleError(SyntacticExceptions.ExpectedTokenNotFound(currentToken().Line, currentToken().EndColumn, SyntaxKind.OpenBraceToken));

                return new MethodDeclaration
                {
                    Modifier = modifier,
                    ReturnType = type,
                    Identifier = identifier,
                    ParameterList = parameterList,
                    Block = block
                };
            }
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
                    handleError(SyntacticExceptions.ExpectedTokenNotFound(result, SyntaxKind.CloseBraceToken));

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
                handleError(SyntacticExceptions.ExpectedTokenNotFound(result, SyntaxKind.CommaToken));

            variableDeclaration.AddTrivia(currentToken());
            moveNextToken();
            VariableAssignmentExpression varDecl;
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
                handleError(SyntacticExceptions.ExpectedTokenNotFound(result, SyntaxKind.SemicolonToken));

            result.SemicolonToken = currentToken();

            moveNextToken();
            return result;
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
                handleError(SyntacticExceptions.ExpectedTokenNotFound(result, SyntaxKind.CloseBraceToken));

            dumpTrivia(result);
            result.CloseParenToken = closeParen;

            moveNextToken();
            return result;
        }
        Parameter getParameter()
        {
            var type = getPredefinedType();
            if (type == null)
                return null;

            var identifier = currentToken();
            if (identifier.Kind != SyntaxKind.Identifier)
                handleError(SyntacticExceptions.ExpectedTokenNotFound(currentToken().Line, currentToken().EndColumn, SyntaxKind.Identifier));

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
                handleError(SyntacticExceptions.ExpectedTokenNotFound(result, SyntaxKind.CloseBraceToken));

            result.CloseBraceToken = closeBrace;

            return result;
        }

        Statement getStatement()
        {
            return getLocalVariableDeclarationStatement() ??
                   getMethodInvocationOrLocalVariableAssignmentStatement() ??
                   getReturnStatement() ??
                   getBreakStatement() ??
                   getIfStatement() ??
                   getForStatement() ??
                   (Statement)getWhileStatement();
        }

        Statement getMethodInvocationOrLocalVariableAssignmentStatement()
        {
            var identifier = currentToken();

            if (identifier.Kind != SyntaxKind.Identifier)
                return null;

            var curr = moveNextToken();
            if (curr.Kind == SyntaxKind.OpenParenToken)
            {
                moveNextToken();
                var methodInvocation = getMethodInvocationExpression(identifier, curr);

                var semicolon = currentToken();
                if (semicolon.Kind != SyntaxKind.SemicolonToken)
                    handleError(SyntacticExceptions.ExpectedTokenNotFound(currentToken().Line, currentToken().EndColumn, SyntaxKind.SemicolonToken));

                var methodResult = new MethodInvocationStatement
                {
                    MethodInvocation = methodInvocation,
                    SemicolonToken = curr
                };

                dumpTrivia(methodResult);
                moveNextToken();

                return methodResult;
            }

            var assignmentResult = getLocalVariableAssignmentStatementWith(identifier);

            dumpTrivia(assignmentResult);

            return assignmentResult;
        }
        LocalVariableAssignmentStatement getLocalVariableAssignmentStatementWith(TokenInfo identifier)
        {
            var declarator = getVariableDeclarator(identifier);
            if (declarator == null)
                return null;

            var result = new LocalVariableAssignmentStatement { Declarator = declarator };

            if (currentToken().Kind != SyntaxKind.SemicolonToken)
                handleError(SyntacticExceptions.ExpectedTokenNotFound(result, SyntaxKind.SemicolonToken));

            result.SemicolonToken = currentToken();
            dumpTrivia(result);

            moveNextToken();
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
                handleError(SyntacticExceptions.ExpectedTokenNotFound(result, SyntaxKind.SemicolonToken));

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
                handleError(SyntacticExceptions.ExpectedTokenNotFound(result, SyntaxKind.SemicolonToken));

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
                handleError(SyntacticExceptions.ExpectedTokenNotFound(result, SyntaxKind.OpenBraceToken));

            result.OpenParenToken = openParen;

            dumpTrivia(result);
            moveNextToken();
            result.Condition = getExpression();

            var closeParen = currentToken();
            if (closeParen.Kind != SyntaxKind.CloseParenToken)
                handleError(SyntacticExceptions.ExpectedTokenNotFound(result, SyntaxKind.CloseBraceToken));

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
                handleError(SyntacticExceptions.ExpectedTokenNotFound(result, SyntaxKind.OpenBraceToken));

            moveNextToken();

            result.OpenParenToken = openParen;
            dumpTrivia(result);

            result.Initializer = getExpression();
            var firstSemicolon = currentToken();
            if (firstSemicolon.Kind != SyntaxKind.SemicolonToken)
                handleError(SyntacticExceptions.ExpectedTokenNotFound(result, SyntaxKind.SemicolonToken));

            result.FirstSemicolonToken = firstSemicolon;
            dumpTrivia(result);

            moveNextToken();
            result.Condition = getExpression();
            var secondSemicolon = currentToken();
            if (secondSemicolon.Kind != SyntaxKind.SemicolonToken)
                handleError(SyntacticExceptions.ExpectedTokenNotFound(result, SyntaxKind.SemicolonToken));

            moveNextToken();
            result.SecondSemicolonToken = secondSemicolon;
            dumpTrivia(result);

            result.Incrementor = getExpression();

            var closeParen = currentToken();
            if (closeParen.Kind != SyntaxKind.CloseParenToken)
                handleError(SyntacticExceptions.ExpectedTokenNotFound(result, SyntaxKind.CloseBraceToken));

            moveNextToken();
            result.CloseParenToken = closeParen;
            dumpTrivia(result);

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
                handleError(SyntacticExceptions.ExpectedTokenNotFound(result, SyntaxKind.OpenParenToken));

            result.OpenParenToken = openParen;

            dumpTrivia(result);
            moveNextToken();
            result.Condition = getExpression();

            var closeParen = currentToken();
            if (closeParen.Kind != SyntaxKind.CloseParenToken)
                handleError(SyntacticExceptions.ExpectedTokenNotFound(result, SyntaxKind.CloseBraceToken));

            result.CloseParenToken = closeParen;

            dumpTrivia(result);
            moveNextToken();
            result.Block = getBlock();

            return result;
        }
        LocalVariableDeclarationStatement getLocalVariableDeclarationStatement()
        {
            var type = getPredefinedType();
            if (type == null)
                return null;

            var result = new LocalVariableDeclarationStatement();
            var identifier = currentToken();
            if (identifier.Kind != SyntaxKind.Identifier)
                handleError(SyntacticExceptions.ExpectedTokenNotFound(result, SyntaxKind.Identifier));

            moveNextToken();
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
                handleError(SyntacticExceptions.ExpectedTokenNotFound(result, SyntaxKind.CommaToken));

            variableDeclaration.AddTrivia(currentToken());
            moveNextToken();
            VariableAssignmentExpression varDecl;
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
                handleError(SyntacticExceptions.ExpectedTokenNotFound(result, SyntaxKind.SemicolonToken));

            result.SemicolonToken = currentToken();

            moveNextToken();
            return result;
        }
        #endregion

        #region Expressions
        Expression getExpression()
        {
            return getBinaryExpression();
        }

        Expression getBinaryExpression()
        {
            var arithmeticExpression = getArithmeticBinaryExpression();
            if (arithmeticExpression == null)
                return null;

            return getLogicalOrExpression(arithmeticExpression);
        }

        Expression getLogicalOrExpression(Expression left)
        {
            var op = currentToken();
            if (op.Kind != SyntaxKind.OrOperator)
                return getLogicalAndExpression(left);

            moveNextToken();

            return new LogicalOrExpression
            {
                LeftSide = left,
                OperatorToken = op,
                RightSide = getExpression()
            };
        }
        Expression getLogicalAndExpression(Expression left)
        {
            var op = currentToken();
            if (op.Kind != SyntaxKind.AndOperator)
                return getComparatorExpression(left);

            moveNextToken();

            return new LogicalAndExpression
            {
                LeftSide = left,
                OperatorToken = op,
                RightSide = getExpression()
            };
        }
        Expression getComparatorExpression(Expression left)
        {
            BinaryExpression result;
            var op = currentToken();

            switch (op.Kind)
            {
                case SyntaxKind.ExclamationEqualsToken:
                    result = new ExclamationEqualsExpression();
                    break;
                case SyntaxKind.EqualsEqualsOperator:
                    result = new EqualsEqualsExpression();
                    break;
                case SyntaxKind.MinorOperator:
                    result = new LogicalLessThanExpression();
                    break;
                case SyntaxKind.MinorEqualsOperator:
                    result = new LogicalLessOrEqualThanExpression();
                    break;
                case SyntaxKind.MajorOperator:
                    result = new LogicalGreaterThanExpression();
                    break;
                case SyntaxKind.MajorEqualsOperator:
                    result = new LogicalGreaterOrEqualThanExpression();
                    break;
                default:
                    return left;
            }

            moveNextToken();

            result.LeftSide = left;
            result.OperatorToken = op;
            result.RightSide = getExpression();

            return result;
        }

        Expression getArithmeticBinaryExpression()
        {
            Expression root = getAdditiveExpression(null), aux = null;

            if (root == null)
                return null;

            while (true)
            {
                if ((aux = getAdditiveExpression(root)) != root)
                {
                    root = aux;
                    continue;
                }
                else
                    break;
            }

            return root;
        }
        Expression getUnaryExpression()
        {
            return getLiteralExpression() ??
                   getMethodInvocationOrAccessorOrDeclaratorExpression() ??
                   getSignedUnaryExpression();
        }
        ParenthesedExpression getParenthesedExpression()
        {
            var openParen = currentToken();
            if (openParen.Kind != SyntaxKind.OpenParenToken)
                return null;

            moveNextToken();
            var innerExpression = getExpression();

            if (innerExpression == null)
                handleError(SyntacticExceptions.ExpectedTokenNotFound(currentToken().Line, currentToken().EndColumn, "innerExpression"));

            var closeParen = currentToken();
            if (closeParen.Kind != SyntaxKind.CloseParenToken)
                handleError(SyntacticExceptions.ExpectedTokenNotFound(currentToken().Line, currentToken().EndColumn, "closeParen"));

            moveNextToken();
            return new ParenthesedExpression
            {
                OpenParenToken = openParen,
                InnerExpression = innerExpression,
                CloseParenToken = closeParen
            };
        }
        Expression getMethodInvocationOrAccessorOrDeclaratorExpression()
        {
            var identifier = currentToken();
            if (identifier.Kind != SyntaxKind.Identifier)
                return null;

            var curr = moveNextToken();
            if (curr.Kind == SyntaxKind.OpenParenToken)
            {
                moveNextToken();
                return getMethodInvocationExpression(identifier, curr);
            }

            var result = new AccessorExpression { Identifier = identifier };
            dumpTrivia(result);

            if (curr.Kind == SyntaxKind.OpenBracketToken)
            {
                var bracketed = new BracketedArgument { OpenBracketToken = curr };
                moveNextToken();
                bracketed.ArgumentExpression = getExpression();

                if (currentToken().Kind != SyntaxKind.CloseBracketToken)
                    handleError(SyntacticExceptions.ExpectedTokenNotFound(result, SyntaxKind.CloseBracketToken));

                bracketed.CloseBracketToken = currentToken();
                moveNextToken();

                result.BracketedArgument = bracketed;
                dumpTrivia(bracketed);
            }
            else if (curr.Kind.IsAssignmentOperator())
            {
                moveNextToken();
                return new VariableAssignmentExpression { Identifier = identifier, Assignment = getAssignmentExpression(curr) };
            }

            return result;
        }
        VariableAssignmentExpression getVariableDeclarator()
        {
            var current = currentToken();

            if (current.Kind != SyntaxKind.Identifier)
                return null;

            moveNextToken();
            return getVariableDeclarator(current);
        }
        VariableAssignmentExpression getVariableDeclarator(TokenInfo identifier)
        {
            var current = currentToken();

            if (current.Kind.IsIn(SyntaxKind.CommaToken, SyntaxKind.SemicolonToken))
                return new VariableAssignmentExpression { Identifier = identifier };

            if (current.Kind.IsAssignmentOperator())
            {
                moveNextToken();
                return new VariableAssignmentExpression { Identifier = identifier, Assignment = getAssignmentExpression(current) };
            }
            handleError(SyntacticExceptions.ExpectedTokenNotFound(currentToken().Line, currentToken().EndColumn, SyntaxKind.SemicolonToken));
            return null;
        }
        AssignmentExpression getAssignmentExpression(TokenInfo assignmentOperatorInfo)
        {
            return new AssignmentExpression
            {
                OperatorToken = assignmentOperatorInfo,
                Expression = getExpression()
            };
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
                    handleError(SyntacticExceptions.ExpectedTokenNotFound(currentToken().Line, currentToken().EndColumn, SyntaxKind.OpenParenToken));

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
                else
                {
                    if (currentToken().Kind != SyntaxKind.CloseParenToken)
                    {
                        handleError(SyntacticExceptions.ExpectedTokenNotFound(SyntaxKind.CloseParenToken, arguments));
                        break;
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

            if (op.Kind == SyntaxKind.PlusToken)
                return new AddExpression
                {
                    LeftSide = leftSide,
                    OperatorToken = op,
                    RightSide = rightSide
                };
            else
                return new SubtractExpression
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

            if (op.Kind == SyntaxKind.AsteriskToken)
                return new MultiplyExpression
                {
                    LeftSide = leftSide,
                    OperatorToken = op,
                    RightSide = rightSide
                };
            else
                return new DivideExpression
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

            return new ModExpression
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
                return new UnaryExpression { SignalToken = signal, Expression = getLiteralExpression() ?? getMethodInvocationOrAccessorOrDeclaratorExpression() ?? getParenthesedExpression() as Expression };
            }

            return getLiteralExpression() ?? getMethodInvocationOrAccessorOrDeclaratorExpression() ?? getParenthesedExpression() as Expression;
        }
        LiteralExpression getLiteralExpression()
        {
            var value = currentToken();

            if (!value.Kind.IsLiteral())
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
                if (node != null)
                    node.AddTrivia(triviaBuffer.Dequeue());
                else
                    triviaBuffer.Dequeue();
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
        void handleError(SyntacticException error = null)
        {
            _syntacticErrors.Add(error);
        }
    }
}