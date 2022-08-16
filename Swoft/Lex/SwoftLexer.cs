using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyLex;

namespace Swoft.Lex
{
    public class SwoftLexer
    {
        private DefaultLexer<SwoftToken> _lexer;

        public SwoftLexer()
        {
            _lexer = new DefaultLexer<SwoftToken>();

            // Error setup
            _lexer.SetErrorProcessor(data => new SwoftToken(SwoftTokenType.Unknown, data));

            // Whitespace etc
            _lexer.SequenceOf(char.IsWhiteSpace).Becomes(data => new SwoftToken(SwoftTokenType.Whitespace, data));

            // Keywords
            _lexer.Literal("function").Becomes(data => new SwoftToken(SwoftTokenType.Keyword, data));
            _lexer.Literal("class").Becomes(data => new SwoftToken(SwoftTokenType.Keyword, data));
            _lexer.Literal("struct").Becomes(data => new SwoftToken(SwoftTokenType.Keyword, data));

            // Identfiers
            _lexer.SequenceOf(char.IsLetterOrDigit)
                .StartsWith(char.IsLetter)
                .Becomes(data => new SwoftToken(SwoftTokenType.Identifier, data));

            // Operators, etc
            _lexer.Literal("(").Becomes(d => new SwoftToken(SwoftTokenType.BracketOpen, d));
            _lexer.Literal(")").Becomes(d => new SwoftToken(SwoftTokenType.BracketClose, d));
            _lexer.Literal("[").Becomes(d => new SwoftToken(SwoftTokenType.ArrayOpen, d));
            _lexer.Literal("]").Becomes(d => new SwoftToken(SwoftTokenType.ArrayClose, d));
            _lexer.Literal("{").Becomes(d => new SwoftToken(SwoftTokenType.CurlyOpen, d));
            _lexer.Literal("}").Becomes(d => new SwoftToken(SwoftTokenType.CurlyClose, d));

            _lexer.Literal("+").Becomes(d => new SwoftToken(SwoftTokenType.BinaryOperator, d));
            _lexer.Literal("-").Becomes(d => new SwoftToken(SwoftTokenType.BinaryOperator, d));
            _lexer.Literal("*").Becomes(d => new SwoftToken(SwoftTokenType.BinaryOperator, d));
            _lexer.Literal("/").Becomes(d => new SwoftToken(SwoftTokenType.BinaryOperator, d));
            _lexer.Literal("%").Becomes(d => new SwoftToken(SwoftTokenType.BinaryOperator, d));

            _lexer.Literal("=").Becomes(d => new SwoftToken(SwoftTokenType.BinaryOperator, d));
            _lexer.Literal("+=").Becomes(d => new SwoftToken(SwoftTokenType.BinaryOperator, d));
            _lexer.Literal("-=").Becomes(d => new SwoftToken(SwoftTokenType.BinaryOperator, d));
            _lexer.Literal("*=").Becomes(d => new SwoftToken(SwoftTokenType.BinaryOperator, d));
            _lexer.Literal("/=").Becomes(d => new SwoftToken(SwoftTokenType.BinaryOperator, d));
            _lexer.Literal("%=").Becomes(d => new SwoftToken(SwoftTokenType.BinaryOperator, d));

            _lexer.Literal("==").Becomes(d => new SwoftToken(SwoftTokenType.BinaryOperator, d));
            _lexer.Literal(">").Becomes(d => new SwoftToken(SwoftTokenType.BinaryOperator, d));
            _lexer.Literal("<").Becomes(d => new SwoftToken(SwoftTokenType.BinaryOperator, d));
            _lexer.Literal(">=").Becomes(d => new SwoftToken(SwoftTokenType.BinaryOperator, d));
            _lexer.Literal("<=").Becomes(d => new SwoftToken(SwoftTokenType.BinaryOperator, d));

            _lexer.Literal("||").Becomes(d => new SwoftToken(SwoftTokenType.BinaryOperator, d));
            _lexer.Literal("&&").Becomes(d => new SwoftToken(SwoftTokenType.BinaryOperator, d));
            _lexer.Literal("!").Becomes(d => new SwoftToken(SwoftTokenType.BinaryOperator, d));

            _lexer.Literal("&").Becomes(d => new SwoftToken(SwoftTokenType.BinaryOperator, d));
            _lexer.Literal("|").Becomes(d => new SwoftToken(SwoftTokenType.BinaryOperator, d));
            _lexer.Literal("^").Becomes(d => new SwoftToken(SwoftTokenType.BinaryOperator, d));
            _lexer.Literal(">>").Becomes(d => new SwoftToken(SwoftTokenType.BinaryOperator, d));
            _lexer.Literal("<<").Becomes(d => new SwoftToken(SwoftTokenType.BinaryOperator, d));

            _lexer.Literal("++").Becomes(d => new SwoftToken(SwoftTokenType.UnaryOperator, d));
            _lexer.Literal("--").Becomes(d => new SwoftToken(SwoftTokenType.UnaryOperator, d));

            _lexer.Literal("=>").Becomes(d => new SwoftToken(SwoftTokenType.Arrow, d));
            _lexer.Literal(":").Becomes(d => new SwoftToken(SwoftTokenType.Colon, d));
            _lexer.Literal(".").Becomes(d => new SwoftToken(SwoftTokenType.Lookup, d));
            _lexer.Literal(",").Becomes(d => new SwoftToken(SwoftTokenType.Seperator, d));
            _lexer.Literal(";").Becomes(d => new SwoftToken(SwoftTokenType.LineEnd, d));

            // Value literals
            _lexer.OpenClose(open: "\"", close: "\"", escape: "\\").Becomes(data => new SwoftToken(SwoftTokenType.String, data));
            _lexer.OpenClose(open: "'", close: "'", escape: "\\").Becomes(data => new SwoftToken(SwoftTokenType.String, data));
            _lexer.SequenceOf(char.IsDigit).Becomes(data => new SwoftToken(SwoftTokenType.Integer, data));
            _lexer.Lambda(TryLexFloat).Becomes(data => new SwoftToken(SwoftTokenType.Float, data));

            // Comments
            _lexer.OpenClose("//", "\n").Becomes(data => new SwoftToken(SwoftTokenType.Comment, data));
            _lexer.OpenClose("/*", "*/").Becomes(data => new SwoftToken(SwoftTokenType.Comment, data));
        }

        public LexerOutput<SwoftToken> Tokenize(string input)
        {
            return _lexer.Tokenize(input);
        }

        private string? TryLexFloat(IStringSpanIterator stream)
        {
            if (!char.IsDigit(stream.Current()))
            {
                return null;
            }

            StringBuilder builder = new StringBuilder();

            builder.Append(stream.Current());

            while (stream.Next() && char.IsDigit(stream.Current()))
            {
                builder.Append(stream.Current());
            }

            if (!stream.HasCurrent() || stream.Current() != '.')
            {
                return builder.ToString();
            }

            Debug.Assert(stream.Current() == '.');
            builder.Append('.');

            while (stream.Next() && char.IsDigit(stream.Current()))
            {
                builder.Append(stream.Current());
            }

            return builder.ToString();
        }
    }
}
