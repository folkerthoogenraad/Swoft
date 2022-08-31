using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyLex
{
    public class DefaultLexer<TToken>
        where TToken : class
    {
        private record TokenizerOutput(ITokenizer<TToken> Tokenizer, IStringSpanIterator Stream, TToken? Token)
        {
            public int Precedence => Tokenizer.Precedence;
            public bool Succeeded => Token != null;
            public int Offset => Stream.Offset;
        }

        private List<ITokenizer<TToken>> _tokenizers;
        private IEnumerable<ITokenizer<TToken>> Tokenizers => _tokenizers;
        private Func<TokenInfo, TToken?>? _default;

        public DefaultLexer()
        {
            _tokenizers = new List<ITokenizer<TToken>>();
        }

        public T AddTokenizer<T>(T tokenizer)
            where T: ITokenizer<TToken>
        {
            _tokenizers.Add(tokenizer);

            return tokenizer;
        }

        public void RemoveTokenizer(ITokenizer<TToken> tokenizer)
        {
            _tokenizers.Remove(tokenizer);
        }

        public void SetErrorProcessor(Func<TokenInfo, TToken?>? func)
        {
            _default = func;
        }

        public LexerOutput<TToken> Tokenize(string input)
        {
            List<TToken> tokens = new List<TToken>();
            List<LexerError> errors = new List<LexerError>();

            int offset = 0;

            while(offset < input.Length)
            {
                var result = Tokenizers
                    .Select(tokenizer => TryTokenize(tokenizer, input, offset))
                    .Where(output => output.Succeeded)
                    .OrderByDescending(output => output.Precedence)
                    .ThenByDescending(output => output.Offset)
                    .FirstOrDefault();

                if (result == null || !result.Succeeded)
                {
                    errors.Add(new LexerError(offset, $"Unknown character '{input[offset]}' No matching token found."));

                    var def = _default?.Invoke(new TokenInfo("" + input[offset], offset, 0, 0)); // TODO line and column

                    if(def != null)
                    {
                        tokens.Add(def);
                    }
                    
                    offset++;
                    continue;
                }

                Debug.Assert(result.Token != null);

                tokens.Add(result.Token);

                if(result.Offset <= offset)
                {
                    throw new ApplicationException($"Tokenizer ({ result.Tokenizer.GetType() }) is functioning incorrectly! A token is never allowed to consume 0 or fewer characters.");
                }

                offset = result.Offset;
            }

            return new LexerOutput<TToken>(tokens.ToArray(), errors.ToArray());
        }

        private TokenizerOutput TryTokenize(ITokenizer<TToken> tokenizer, string data, int offset)
        {
            var stream = new StringOffsetCharacterStream(data, offset);

            TToken? token = null;

            try
            {
                token = tokenizer.Tokenize(stream);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return new TokenizerOutput(tokenizer, stream, token);
        }
    }
}
