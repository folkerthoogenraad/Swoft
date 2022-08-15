using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyLex
{
    public class Lexer<TToken>
        where TToken : class
    {
        private record TokenizerOutput(ITokenizer<TToken> Tokenizer, ICharacterStream Stream, TToken? Token)
        {
            public int Precedence => Tokenizer.Precedence;
            public bool Succeeded => Token != null;
            public int Offset => Stream.Offset;
        }

        private List<ITokenizer<TToken>> _tokenizers;
        private IEnumerable<ITokenizer<TToken>> Tokenizers => _tokenizers;

        public Lexer()
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
                    offset++;
                    continue;
                }

                Debug.Assert(result.Token != null);

                tokens.Add(result.Token);
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
            catch { }

            return new TokenizerOutput(tokenizer, stream, token);
        }
    }
}
