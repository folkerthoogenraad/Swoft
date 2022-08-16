using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swoft.Lex
{
    public enum SwoftTokenType
    {
        Unknown,

        Keyword,

        UnaryOperator,
        BinaryOperator,

        Colon,
        Seperator,
        LineEnd,
        Lookup,

        Arrow,

        BracketOpen,
        BracketClose,
        CurlyOpen,
        CurlyClose,
        ArrayOpen,
        ArrayClose,

        String,

        Integer,
        Float,

        Identifier,

        Whitespace,
        Comment,
    }
}
