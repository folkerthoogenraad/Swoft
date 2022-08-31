

using System.Text;
using Swoft.Lex;

namespace Swoft.Parser
{
    public class UnexpectedTokenException : Exception
    {
        public Token? Current;
        public TokenType[] Expected;
        
        public UnexpectedTokenException(Token? current = null)
        {
            Current = current;
            Expected = new TokenType[0];
        }

        public UnexpectedTokenException(Token current, TokenType expected)
        {
            Current = current;
            Expected = new [] {expected};
        }
        
        public UnexpectedTokenException(TokenType expected)
        {
            Expected = new [] {expected};
        }

        public UnexpectedTokenException(Token current, params TokenType[] expected)
        {
            Current = current;
            Expected = expected;
        }


        public override string Message => GetMessage();

        private string GetMessage()
        {
            var builder = new StringBuilder();

            if(Current == null)
            {
                builder.Append("Unexpected token.");
            }
            else
            {
                builder.Append($"Unexpected token ({Current.ToString()}.");
            }

            if(Expected.Length == 0) return builder.ToString();

            builder.Append(" ");

            if(Expected.Length == 1)
            {
                builder.Append($"Expected {Expected[0]} instead.");
            }
            else
            {
                builder.Append("Expected on of ");

                builder.Append(string.Join(", ", Expected.Select(x => x.ToString())));

                builder.Append(" instead.");
            }
        }
    }
}