using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swoft.Compiler
{
    public class Token
    {
        public enum TokenType
        {
            Identifier,
            Equals,

            Number,
            String,
            Boolean,
            Null,

            Param,

            At,

            Seperator,

            Question,

            BracketOpen,
            BracketClose,

            ArrayOpen,
            ArrayClose,

            CurlyOpen,
            CurlyClose,

            LineEnd,

            Lookup,

            Variable,
            Store,

            // Others
            Comment,
            WhiteSpace,
            Unknown,

            EndOfFile
        }

        public TokenType Type { get; set; }
        public string Data { get; set; }

        public Token()
        {
            Type = TokenType.Unknown;
            Data = "";
        }

        public Token(TokenType type, string data)
        {
            Type = type;
            Data = data;
        }

        public override string ToString()
        {
            return string.Format("{0}: ({1})", Type, Data);
        }
    }
}
