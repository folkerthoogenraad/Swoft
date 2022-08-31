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
        private DefaultLexer<Token> _lexer;

        public SwoftLexer()
        {
            _lexer = new DefaultLexer<Token>();

            // Error setup
            _lexer.SetErrorProcessor(data => new Token(TokenType.Unknown, data));

            // Whitespace etc
            _lexer.SequenceOf(char.IsWhiteSpace).Becomes(data => new Token(TokenType.Whitespace, data));

            // Keywords
            _lexer.Literal("function").Becomes(data => new Token(TokenType.KeywordFunction, data));
            _lexer.Literal("var").Becomes(data => new Token(TokenType.KeywordVar, data));
            _lexer.Literal("struct").Becomes(data => new Token(TokenType.KeywordStruct, data));
            _lexer.Literal("procedure").Becomes(data => new Token(TokenType.KeywordProcedure, data));
            _lexer.Literal("if").Becomes(data => new Token(TokenType.KeywordIf, data));
            _lexer.Literal("else").Becomes(data => new Token(TokenType.KeywordElse, data));
            _lexer.Literal("while").Becomes(data => new Token(TokenType.KeywordWhile, data));
            _lexer.Literal("when").Becomes(data => new Token(TokenType.KeywordWhen, data));
            _lexer.Literal("await").Becomes(data => new Token(TokenType.KeywordAwait, data));

            _lexer.Literal("public").Becomes(data => new Token(TokenType.ModifierPublic, data));
            _lexer.Literal("extern").Becomes(data => new Token(TokenType.ModifierExtern, data));

            // Identfiers
            _lexer.SequenceOf(char.IsLetterOrDigit)
                .StartsWith(char.IsLetter)
                .Becomes(data => new Token(TokenType.Identifier, data));

            // Operators, etc
            _lexer.Literal("(").Becomes(d => new Token(TokenType.BracketOpen, d));
            _lexer.Literal(")").Becomes(d => new Token(TokenType.BracketClose, d));
            _lexer.Literal("[").Becomes(d => new Token(TokenType.ArrayOpen, d));
            _lexer.Literal("]").Becomes(d => new Token(TokenType.ArrayClose, d));
            _lexer.Literal("{").Becomes(d => new Token(TokenType.CurlyOpen, d));
            _lexer.Literal("}").Becomes(d => new Token(TokenType.CurlyClose, d));

            _lexer.Literal("+").Becomes(d => new Token(TokenType.OperatorAdd, d));
            _lexer.Literal("-").Becomes(d => new Token(TokenType.OperatorSub, d));
            _lexer.Literal("*").Becomes(d => new Token(TokenType.OperatorMul, d));
            _lexer.Literal("/").Becomes(d => new Token(TokenType.OperatorDiv, d));
            _lexer.Literal("%").Becomes(d => new Token(TokenType.OperatorMod, d));
            _lexer.Literal("=").Becomes(d => new Token(TokenType.OperatorEq, d));
            _lexer.Literal("+=").Becomes(d => new Token(TokenType.OperatorEqAdd, d));
            _lexer.Literal("-=").Becomes(d => new Token(TokenType.OperatorEqSub, d));
            _lexer.Literal("*=").Becomes(d => new Token(TokenType.OperatorEqMul, d));
            _lexer.Literal("/=").Becomes(d => new Token(TokenType.OperatorEqDiv, d));
            _lexer.Literal("%=").Becomes(d => new Token(TokenType.OperatorEqMod, d));
            _lexer.Literal("==").Becomes(d => new Token(TokenType.OperatorIsEq, d));
            _lexer.Literal(">").Becomes(d => new Token(TokenType.OperatorIsGt, d));
            _lexer.Literal("<").Becomes(d => new Token(TokenType.OperatorIsLt, d));
            _lexer.Literal(">=").Becomes(d => new Token(TokenType.OperatorIsGtEq, d));
            _lexer.Literal("<=").Becomes(d => new Token(TokenType.OperatorIsLtEq, d));
            _lexer.Literal("||").Becomes(d => new Token(TokenType.OperatorOr, d));
            _lexer.Literal("&&").Becomes(d => new Token(TokenType.OperatorAnd, d));
            _lexer.Literal("!").Becomes(d => new Token(TokenType.OperatorNot, d));
            _lexer.Literal("&").Becomes(d => new Token(TokenType.OperatorBitwiseAnd, d));
            _lexer.Literal("|").Becomes(d => new Token(TokenType.OperatorBitwiseOr, d));
            _lexer.Literal("^").Becomes(d => new Token(TokenType.OperatorBitwiseNot, d));
            _lexer.Literal(">>").Becomes(d => new Token(TokenType.OperatorBitwiseShiftRight, d));
            _lexer.Literal("<<").Becomes(d => new Token(TokenType.OperatorBitwiseShiftLeft, d));
            _lexer.Literal("++").Becomes(d => new Token(TokenType.OperatorAddOne, d));
            _lexer.Literal("--").Becomes(d => new Token(TokenType.OperatorSubOne, d));

            _lexer.Literal("=>").Becomes(d => new Token(TokenType.Arrow, d));
            _lexer.Literal(":").Becomes(d => new Token(TokenType.Colon, d));
            _lexer.Literal(".").Becomes(d => new Token(TokenType.Lookup, d));
            _lexer.Literal(",").Becomes(d => new Token(TokenType.Seperator, d));
            _lexer.Literal(";").Becomes(d => new Token(TokenType.LineEnd, d));

            // Value literals
            _lexer.OpenClose(open: "\"", close: "\"", escape: "\\").Becomes(data => new Token(TokenType.String, data));
            _lexer.OpenClose(open: "'", close: "'", escape: "\\").Becomes(data => new Token(TokenType.String, data));
            _lexer.SequenceOf(char.IsDigit).Becomes(data => new Token(TokenType.Integer, data));
            _lexer.Lambda(TryLexFloat).Becomes(data => new Token(TokenType.Float, data));

            // Comments
            _lexer.OpenClose("//", "\n").Becomes(data => new Token(TokenType.Comment, data));
            _lexer.OpenClose("/*", "*/").Becomes(data => new Token(TokenType.Comment, data));
        }

        public LexerOutput<Token> Tokenize(string input)
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
