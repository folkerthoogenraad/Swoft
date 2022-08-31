using Swoft.AST;
using Swoft.Lex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swoft.Parser
{
    public class SwoftParser
    {
        private TokenStream _tokens;

        public SwoftParser(IEnumerable<Token> tokens)
            : this(new TokenStream(tokens))
        {
            
        }

        public SwoftParser(TokenStream stream)
        {
            _tokens = stream;
        }

        public Statement[] ParseStatements()
        {
            var statements = new List<Statement>();

            while (_tokens.HasCurrent())
            {
                statements.Add(ParseStatement());
            }

            return statements.ToArray();
        }

        public Statement ParseStatement()
        {
            var current = _tokens.Current();

            // Ignore modifiers for now.
            var modifiers = ParseModifiers();

            if(current.Type == TokenType.KeywordFunction)
            {
                return ParseFunction();
            }

            UnexpectedToken(current);
        }

        public FunctionStatement ParseFunction()
        {
            if(_tokens.Current().Type != TokenType.KeywordFunction)
            {
                UnexpectedToken(_tokens.Current(), TokenType.KeywordFunction);
            }

            _tokens.Next();


        }

        public int ParseModifiers()
        {
            // Ignore all modifiers currently :)
            while(_tokens.HasCurrent() && _tokens.Current().Type.IsModifier())
            {
                _tokens.Next();
            }

            return 0;
        }

        public void UnexpectedToken(Token current)
        {
            throw new UnexpectedTokenException(current);
        }

        public void UnexpectedToken(Token current, TokenType expected)
        {
            throw new UnexpectedTokenException(current, expected);
        }
    }
}
