using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swoft.Compiler
{
#if false
    public class Parser
    {
        private Iterator<Token> _tokens;

        private List<string> _messages;

        // TODO use some kind of messages struct where there is also a message type.
        public IEnumerable<string> Messages => _messages;

        public Parser(IEnumerable<Token> tokens)
        {
            // Ignore unkown, comments and whitespaces
            _tokens = new Iterator<Token>(tokens
                .Where(token => token.Type != Token.TokenType.Unknown
                    && token.Type != Token.TokenType.Comment
                    && token.Type != Token.TokenType.WhiteSpace)
                );

            _messages = new List<string>();
        }

        public IEnumerable<SmolExpression> Parse()
        {
            while (_tokens.HasCurrent && _tokens.Current().Type != Token.TokenType.EndOfFile)
            {
                yield return ParseExpression();
            }
        }

        public IEnumerable<SmolExpression> ParseBracketed()
        {
            while (_tokens.HasCurrent && _tokens.Current().Type != Token.TokenType.EndOfFile && _tokens.Current().Type != Token.TokenType.BracketClose)
            {
                yield return ParseExpression();
            }
        }

        public IEnumerable<SmolExpression> ParseLambda()
        {
            while (_tokens.HasCurrent 
                && _tokens.Current().Type != Token.TokenType.EndOfFile 
                && _tokens.Current().Type != Token.TokenType.CurlyClose)
            {
                yield return ParseExpression();
            }
        }

        public SmolValue ConvertToLambda(IEnumerable<SmolExpression> expressions)
        {
            return new SmolLambda(new UnscopedBlockExpression(expressions.ToArray()));
        }

        public SmolExpression ParseExpression()
        {
            var current = _tokens.Current();

            if (current.Type == Token.TokenType.Identifier)
            {
                var param = _tokens.Next();

                var command = new FunctionCallSmolExpression(current.Data);

                while (param != null && param.Type == Token.TokenType.Param)
                {
                    var name = param.Data.Substring(1);

                    _tokens.Next();

                    var value = ParseExpression();
                    command.Parameters.Add(name, value);

                    param = _tokens.Current();
                }

                return command;
            }
            else if (current.Type == Token.TokenType.Number)
            {
                _tokens.Next();
                return new ConstSmolExpression(new SmolNumber(double.Parse(current.Data, CultureInfo.InvariantCulture)));
            }
            else if (current.Type == Token.TokenType.String)
            {
                _tokens.Next();

                var data = current.Data;
                var actual = Language.Unescape(data.Substring(1, data.Length - 2));

                return new ConstSmolExpression(new SmolString(actual));
            }
            else if (current.Type == Token.TokenType.Boolean)
            {
                _tokens.Next();
                return new ConstSmolExpression(new SmolBoolean(bool.Parse(current.Data)));
            }
            else if (current.Type == Token.TokenType.Null)
            {
                _tokens.Next();
                return new ConstSmolExpression(new SmolNull());
            }
            else if (current.Type == Token.TokenType.BracketOpen)
            {
                // Consume (
                _tokens.Next();

                var block = ParseBracketed().ToArray();

                current = _tokens.Current();

                if (current == null || current.Type != Token.TokenType.BracketClose)
                {
                    _messages.Add($"Unexpected token {current.Type} ({current.Data}). Expected {Token.TokenType.BracketClose} instead.");
                }

                // Consume )
                _tokens.Next();

                return new BlockSmolExpression(block);
            }
            else if (current.Type == Token.TokenType.CurlyOpen)
            {
                // Consume {
                _tokens.Next();

                var block = ParseLambda().ToArray();

                current = _tokens.Current();

                if (current == null || current.Type != Token.TokenType.CurlyClose)
                {
                    _messages.Add($"Unexpected token {current}. Expected {Token.TokenType.CurlyClose} instead.");
                }

                // Consume }
                _tokens.Next();

                var value = ConvertToLambda(block);

                return new ConstSmolExpression(value);
            }
            else if (current.Type == Token.TokenType.ArrayOpen)
            {
                // Consume [
                current = _tokens.Next();

                var expressions = new List<SmolExpression>();
                
                while(current.Type != Token.TokenType.ArrayClose && current.Type != Token.TokenType.EndOfFile)
                {
                    var expression = ParseExpression();

                    expressions.Add(expression);

                    current = _tokens.Current();
                }

                if (current.Type != Token.TokenType.ArrayClose)
                {
                    _messages.Add($"Unexpected token {current.Type} ({current.Data}). Expected {Token.TokenType.BracketClose} instead.");
                }

                // Consume ]
                _tokens.Next();

                return new ArraySmolExpression(expressions.ToArray());
            }
            else if (current.Type == Token.TokenType.Lookup)
            {
                var data = new LookupSmolExpression(current.Data.Substring(1));

                current = _tokens.Next();

                if (current.Type == Token.TokenType.Question)
                {
                    _tokens.Next();
                    return new ExistsSmolExpression(data);
                }

                return data;
            }
            else if (current.Type == Token.TokenType.LineEnd)
            {
                _tokens.Next();

                return new DumpStackExpression();
            }
            else if (current.Type == Token.TokenType.Variable)
            {
                var name = current.Data.Substring(1);
                var data = new VariableSmolExpression(name);

                current = _tokens.Next();

                if (current.Type == Token.TokenType.Question)
                {
                    _tokens.Next();
                    return new ExistsSmolExpression(data);
                }

                return data;
            }
            else if (current.Type == Token.TokenType.Store)
            {
                var variable = _tokens.Next();

                if(variable.Type != Token.TokenType.Variable)
                {
                    _messages.Add($"Unexpected token {current.Type} ({current.Data}). Expected {Token.TokenType.Variable} instead.");

                    return null;
                }

                var lookupChain = new List<string>();

                lookupChain.Add(variable.Data.Substring(1));

                var token = _tokens.Next();

                while(_tokens.HasCurrent && token.Type == Token.TokenType.Lookup)
                {
                    lookupChain.Add(token.Data.Substring(1));
                    token = _tokens.Next();
                }

                return new StoreSmolExpression(lookupChain.ToArray());
            }
            else if(current.Type == Token.TokenType.At)
            {
                _tokens.Next();
                return new ParentStackExpression();
            }
            else
            {
                _messages.Add($"Unexpected token {current.Type} ({current.Data}).");

                _tokens.Next();

                return ParseExpression();
            }
        }


        public static SmolExpression[] Parse(Token[] tokens)
        {
            Parser parser = new Parser(tokens);
            var parsed = parser.Parse().ToArray();

            if (parser.Messages.Count() > 0)
            {
                throw new SmolParseException(parser.Messages.ToArray());
            }

            return parsed;
        }
    }
#endif
}
