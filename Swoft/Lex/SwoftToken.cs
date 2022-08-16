using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyLex;

namespace Swoft.Lex
{
    public record class SwoftToken(SwoftTokenType Type, TokenInfo info)
    {
        public string Data => info.Data;
    }
}
