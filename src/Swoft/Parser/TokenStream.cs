using Swoft.Lex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swoft.Parser
{
    public class TokenStream
    {
        private Token[] _tokens;
        private int _index = 0;

        public TokenStream(IEnumerable<Token> tokens)
        {
            _tokens = tokens.ToArray();
        }

        public Token Current()
        {
            return _tokens[_index];
        }

        public bool Next()
        {
            _index++;
            return HasCurrent();
        }

        public bool HasNext()
        {
            return _index < _tokens.Length - 2;
        }

        public bool HasCurrent()
        {
            return _index < _tokens.Length - 1;
        }
    }
}
