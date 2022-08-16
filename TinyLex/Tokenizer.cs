using System.Text;

namespace TinyLex
{
    public interface ITokenizer<TToken> 
        where TToken: class
    {
        public int Precedence { get; }
        public TToken? Tokenize(IStringSpanIterator stream);
    }

    public static class ITokenizerExtensions
    {
        public static TToken? TryTokenize<TToken>(this ITokenizer<TToken> tokenizer, IStringSpanIterator stream)
            where TToken: class
        {
            try
            {
                return tokenizer.Tokenize(stream);
            }
            catch { throw; }
        }
    }

    public abstract class SimpleTokenizer<TToken>: ITokenizer<TToken>
        where TToken : class
    {
        public int Precedence { get; set; } = 0;
        public Func<string, TToken>? Creator { get; set; }

        public abstract TToken? Tokenize(IStringSpanIterator stream);

        protected TToken Create(string data)
        {
            if(Creator == null)
            {
                throw new ApplicationException("No creator registered in tokenizer. Did you add the creator?");
            }

            return Creator(data);
        }

        // TODO this is kinda ugly, we should move this.
        protected bool MatchLiteral(IStringSpanIterator stream, string literal)
        {
            for (int index = 0; index < literal.Length; index++, stream.Next())
            {
                if (stream.Current() != literal[index]) return false;
            }

            return true;
        }
    }

    public class LiteralTokenizer<TToken> : SimpleTokenizer<TToken>
        where TToken : class
    {
        public string Literal { get; set; }

        public LiteralTokenizer(string match)
        {
            Literal = match;
        }

        public override TToken? Tokenize(IStringSpanIterator stream)
        {
            if (!MatchLiteral(stream, Literal))
            {
                return null;
            }

            return Create(Literal);
        }
    }

    public class OpenCloseTokenizer<TToken> : SimpleTokenizer<TToken>
        where TToken : class
    {
        public ITokenizer<string> OpenTokenizer { get; set; }
        public ITokenizer<string> CloseTokenizer { get; set; }
        public ITokenizer<string>? EscapeTokenizer { get; set; }

        public OpenCloseTokenizer(ITokenizer<string> open, ITokenizer<string> close, ITokenizer<string>? escape = null)
        {
            OpenTokenizer = open;
            CloseTokenizer = close;
            EscapeTokenizer = escape;
        }

        public override TToken? Tokenize(IStringSpanIterator stream)
        {
            var openResult = OpenTokenizer.TryTokenize(stream);

            if (openResult == null) return null;

            StringBuilder builder = new StringBuilder();

            builder.Append(openResult);

            while (stream.HasCurrent())
            {
                var escapeResult = EscapeTokenizer?.TryTokenize(stream.Fork());

                if (escapeResult != null)
                {
                    stream.Next(escapeResult.Length);
                    builder.Append(stream.Current());
                    stream.Next();
                    continue;
                }

                var closeResult = CloseTokenizer.TryTokenize(stream.Fork());

                if(closeResult != null)
                {
                    stream.Next(closeResult.Length);
                    builder.Append(closeResult);
                    break;
                }

                builder.Append(stream.Current());
                stream.Next();
            }

            return Create(builder.ToString());
        }
    }

    // I'm not sure if I really want this one. It is kinda exposing the innerworkings somewhat.
    // Although I do want to expose the innerworkings in some ways, so maybe?
    public class LamdaTokenizer<TToken> : SimpleTokenizer<TToken>
        where TToken : class
    {
        public Func<IStringSpanIterator, string?> Func { get; set; }

        public LamdaTokenizer(Func<IStringSpanIterator, string?> func)
        {
            Func = func;
        }

        public override TToken? Tokenize(IStringSpanIterator stream)
        {
            var res = Func(stream);

            if (res == null) return null;

            return Create(res);
        }
    }

    public class SequenceTokenizer<TToken> : SimpleTokenizer<TToken>
        where TToken : class
    {
        private Func<char, bool> _matcher;
        private Func<char, bool>? _startsWith;

        public SequenceTokenizer(Func<char, bool> matcher)
        {
            _matcher = matcher;
        }

        public SequenceTokenizer<TToken> StartsWith(Func<char, bool> startingFunc)
        {
            _startsWith = startingFunc;
            return this;
        }

        public override TToken? Tokenize(IStringSpanIterator stream)
        {
            if(_startsWith != null && !_startsWith(stream.Current()))
            {
                return null;
            }

            else if (!_matcher(stream.Current()))
            {
                return null;
            }

            StringBuilder builder = new StringBuilder();

            while (stream.HasCurrent() && _matcher(stream.Current()))
            {
                builder.Append(stream.Current());

                stream.Next();
            }

            return Create(builder.ToString());
        }
    }

    public static class SimpleTokenizerExtensions
    {
        public static T Creates<T, TToken>(this T tokenizer, Func<string, TToken> creator)
            where T : SimpleTokenizer<TToken>
            where TToken : class
        {
            tokenizer.Creator = creator;
            return tokenizer;
        }
        public static SimpleTokenizer<T> WithPrecedence<T>(this SimpleTokenizer<T> tokenizer, int precedence)
            where T : class
        {
            tokenizer.Precedence = precedence;
            return tokenizer;
        }
    }
    
    // TODO split these off into seperate extension methods
    public static class LexerTokenizerExtensions
    {
        public static SequenceTokenizer<TToken> SequenceOf<TToken>(this Lexer<TToken> lexer, Func<char, bool> matcher)
            where TToken : class
        {
            return lexer.AddTokenizer(new SequenceTokenizer<TToken>(matcher));
        }

        public static LiteralTokenizer<TToken> Literal<TToken>(this Lexer<TToken> lexer, string literal)
            where TToken : class
        {
            return lexer.AddTokenizer(new LiteralTokenizer<TToken>(literal));
        }

        public static LamdaTokenizer<TToken> Lambda<TToken>(this Lexer<TToken> lexer, Func<IStringSpanIterator, string?> func)
            where TToken : class
        {
            return lexer.AddTokenizer(new LamdaTokenizer<TToken>(func));
        }

        public static OpenCloseTokenizer<TToken> OpenClose<TToken>(this Lexer<TToken> lexer, string open, string close, string escape)
            where TToken : class
        {
            return lexer.AddTokenizer(new OpenCloseTokenizer<TToken>(
                open: new LiteralTokenizer<string>(open).Creates(x => x),
                close: new LiteralTokenizer<string>(close).Creates(x => x),
                escape: new LiteralTokenizer<string>(escape).Creates(x => x)));
        }
        public static OpenCloseTokenizer<TToken> OpenClose<TToken>(this Lexer<TToken> lexer, string open, string close)
            where TToken : class
        {
            return lexer.AddTokenizer(new OpenCloseTokenizer<TToken>(
                open: new LiteralTokenizer<string>(open).Creates(x => x),
                close: new LiteralTokenizer<string>(close).Creates(x => x)));
        }
    }

}