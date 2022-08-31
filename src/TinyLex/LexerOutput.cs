using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyLex
{
    public record LexerError(int Offset, string Message);

    public record LexerOutput<TToken>(TToken[] Tokens, LexerError[] Errors)
    {
        public bool Succeeded => Errors.Length <= 0;
    }
}
