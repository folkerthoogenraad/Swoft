using Swoft.AST;
using Swoft.Lex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swoft.Parsers
{
    public class SwoftParser
    {
        private TokenStream _tokens;

        public SwoftParser(IEnumerable<Token> tokens)
            : this(new TokenStream(tokens.Where(token => !token.Type.IsMeta())))
        {
            
        }

        public SwoftParser(TokenStream stream)
        {
            _tokens = stream;
        }

        public FileSyntax ParseFile()
        {
            var statements = new List<StatementSyntax>();

            while (_tokens.HasCurrent())
            {
                statements.Add(ParseStatement());
            }

            return new FileSyntax(statements.ToArray());
        }

        public StatementSyntax ParseStatement()
        {
            var modifiers = ParseModifiers();

            var current = _tokens.Current();

            if (current.Type == TokenType.KeywordFunction)
            {
                return ParseFunction(modifiers);
            }

            if(modifiers != null)
            {
                throw new Exception("Illegal modifiers.");
            }
            
            if (current.Type == TokenType.KeywordVar)
            {
                return ParseVariable();
            }
            else if (current.Type == TokenType.CurlyOpen)
            {
                return ParseBlock();
            }
            else if (current.Type == TokenType.KeywordIf)
            {
                return ParseIfStatement();
            }
            else if (current.Type == TokenType.KeywordReturn)
            {
                return ParseReturnStatement();
            }
            else
            {
                return ParseExpressionStatement();
            }
        }

        public BlockStatementSyntax ParseBlock()
        {
            ExpectAndComsume(TokenType.CurlyOpen);

            var statements = new List<StatementSyntax>();

            while (!Is(TokenType.CurlyClose))
            {
                statements.Add(ParseStatement());
            }

            ExpectAndComsume(TokenType.CurlyClose);

            return new BlockStatementSyntax(statements.ToArray());
        }

        public IfStatementSyntax ParseIfStatement()
        {
            ExpectAndComsume(TokenType.KeywordIf);

            ExpressionSyntax condition = ParseExpression();
            StatementSyntax body = ParseStatement();
            List<ElseStatementSyntax> elseStatements = new List<ElseStatementSyntax>();

            while (Is(TokenType.KeywordElse))
            {
                var el = ParseElseStatement();

                elseStatements.Add(el);
            }

            return new IfStatementSyntax(condition, body, elseStatements.ToArray());
        }

        public ElseStatementSyntax ParseElseStatement()
        {
            ExpectAndComsume(TokenType.KeywordElse);

            ExpressionSyntax? condition = null;

            if (IsThenConsume(TokenType.KeywordIf))
            {
                condition = ParseExpression();
            }

            StatementSyntax body = ParseStatement();

            return new ElseStatementSyntax(condition, body);
        }

        public VariableStatementSyntax ParseVariable()
        {
            ExpectAndComsume(TokenType.KeywordVar);

            NameAndTypeSyntax nameAndType = ParseNameAndType();
            ExpressionSyntax? initialValue = null;

            if (IsThenConsume(TokenType.OperatorEq))
            {
                initialValue = ParseExpression();
            }

            ExpectAndComsume(TokenType.LineEnd);

            return new VariableStatementSyntax(nameAndType, initialValue);
        }

        public ListExpressionSyntax ParseListExpression()
        {
            var list = new List<ExpressionSyntax>();
            do
            {
                var exp = ParseExpression();

                list.Add(exp);
            } while (IsThenConsume(TokenType.Seperator));

            return new ListExpressionSyntax(list.ToArray());
        }

        public ExpressionSyntax ParseExpression(int precedence = 8)
        {
            if (precedence <= 0) throw new ApplicationException("Precedence cannot be below zero");

            // Calls and lookups
            if (precedence == 3)
            {
                var lhs = ParseExpression(precedence - 1);

                while(Is(TokenType.BracketOpen) || Is(TokenType.Lookup))
                {
                    if (IsThenConsume(TokenType.BracketOpen))
                    {
                        ListExpressionSyntax? arguments = null;
                        
                        if (!Is(TokenType.BracketClose))
                        {
                            arguments = ParseListExpression();
                        }

                        ExpectAndComsume(TokenType.BracketClose);

                        lhs = new CallExpression(lhs, arguments);
                    }
                    else if (IsThenConsume(TokenType.Lookup))
                    {
                        var id = ExpectAndComsume(TokenType.Identifier);

                        lhs = new LookupExpression(lhs, id.Data);
                    }
                }

                return lhs;
            }

            // Brackets
            if (precedence == 2)
            {
                if (IsThenConsume(TokenType.BracketOpen))
                {
                    var exp = ParseExpression();

                    ExpectAndComsume(TokenType.BracketClose);

                    return new BracketedExpressionSyntax(exp);
                }
            }

            // Literals and identifiers
            else if(precedence == 1)
            {
                // Array literals?
                if (IsThenConsume(TokenType.String, out var str))
                {
                    return new StringExpressionSyntax(str.Data);
                }
                else if (IsThenConsume(TokenType.Integer, out var integer))
                {
                    return new IntegerExpressionSyntax(int.Parse(integer.Data));
                }
                else if (IsThenConsume(TokenType.Identifier, out var id))
                {
                    return new IdentifierExpressionSyntax(id.Data);
                }
            }

            return ParseExpression(precedence - 1);
        }

        public ReturnStatementSyntax ParseReturnStatement()
        {
            ExpectAndComsume(TokenType.KeywordReturn);

            var expression = ParseExpression();

            ExpectAndComsume(TokenType.LineEnd);

            return new ReturnStatementSyntax(expression);
        }

        public ExpressionStatementSyntax ParseExpressionStatement()
        {
            var expression = ParseExpression();

            ExpectAndComsume(TokenType.LineEnd);

            return new ExpressionStatementSyntax(expression);
        }

        public FunctionStatementSyntax ParseFunction(ModifiersSyntax? modifiers)
        {
            ExpectAndComsume(TokenType.KeywordFunction);

            var identifier = ExpectAndComsume(TokenType.Identifier);

            string functionName = identifier.Data;
            List<NameAndTypeSyntax> parameters = new List<NameAndTypeSyntax>();
            TypeSyntax? returnType = null;
            StatementSyntax? body = null;

            ExpectAndComsume(TokenType.BracketOpen);

            while(!Is(TokenType.BracketClose))
            {
                var parameter = ParseNameAndType();

                parameters.Add(parameter);

                IsThenConsume(TokenType.Seperator);
            }

            ExpectAndComsume(TokenType.BracketClose);

            if (IsThenConsume(TokenType.Colon))
            {
                returnType = ParseTypeSyntax();
            }

            if (!IsThenConsume(TokenType.LineEnd))
            {
                body = ParseStatement();
            }

            return new FunctionStatementSyntax(functionName, parameters.ToArray(), body, returnType, modifiers);
        }

        public NameAndTypeSyntax ParseNameAndType()
        {
            var identifier = ExpectAndComsume(TokenType.Identifier);

            if (!IsThenConsume(TokenType.Colon))
            {
                return new NameAndTypeSyntax(identifier.Data, null);
            }

            var type = ParseTypeSyntax();

            return new NameAndTypeSyntax(identifier.Data, type);
        }

        public TypeSyntax ParseTypeSyntax()
        {
            var identifier = ExpectAndComsume(TokenType.Identifier);

            return new TypeSyntax(identifier.Data);
        }

        private ModifiersSyntax? ParseModifiers()
        {
            // No modifiers
            if(!Is(type => type.IsModifier()))
            {
                return null;
            }

            // TODO actually create a different modifier thing for each modifer so the syntax tree looks more correct :)
            bool isStatic = false;
            bool isExtern = false;

            while(Is(type => type.IsModifier()))
            {
                var token = GetAndConsume();

                switch (token.Type)
                {
                    case TokenType.ModifierExtern:
                        isExtern = true;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            return new ModifiersSyntax(isStatic, isExtern);
        }

        // ====================================================================== //
        // TODO this should be some kind of parseable interface or whatever
        // ====================================================================== //
        private void Consume()
        {
            _tokens.Next();
        }

        internal bool Is(TokenType type)
        {
            if (_tokens.Current().Type == type)
            {
                return true;
            }
            return false;
        }
        internal bool Is(Func<TokenType, bool> match)
        {
            if (match(_tokens.Current().Type))
            {
                return true;
            }
            return false;
        }

        internal bool IsThenConsume(TokenType type)
        {
            if (Is(type))
            {
                Consume();
                return true;
            }

            return false;
        }

        internal bool IsThenConsume(TokenType type, out Token value)
        {
            value = Get();

            if (Is(type))
            {
                Consume();
                return true;
            }

            return false;
        }

        internal Token Expect(TokenType type)
        {
            if (!Is(type))
            {
                UnexpectedToken(_tokens.Current(), type);
            }

            return _tokens.Current();
        }

        internal Token Get()
        {
            return _tokens.Current();
        }

        internal Token GetAndConsume()
        {
            var result = _tokens.Current();

            Consume();

            return result;
        }

        internal Token ExpectAndComsume(TokenType type)
        {
            var value = Expect(type);

            Consume();

            return value;
        }

        private void UnexpectedToken(Token current)
        {
            throw new UnexpectedTokenException(current);
        }

        private void UnexpectedToken(Token current, TokenType expected)
        {
            throw new UnexpectedTokenException(current, expected);
        }
    }
}
