using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Swoft.Compiler
{
    public class Lexer
    {
        private Iterator<char> _characters;

        private List<string> _errorMessages;

        // TODO use some kind of messages struct where there is also a message type.
        public IEnumerable<string> Messages => _errorMessages;

        public Lexer(string data)
        {
            _characters = new Iterator<char>(data.ToCharArray());
            _errorMessages = new List<string>();
        }

        public IEnumerable<Token> Lex()
        {
            while (_characters.HasCurrent)
            {
                char c = _characters.Current();

                // Strings
                if (c == '"')
                {
                    StringBuilder builder = new StringBuilder();

                    bool addNext = true;

                    while (_characters.HasCurrent && (c != '"' || addNext))
                    {
                        addNext = false;
                        if (c == '\\') addNext = true;
                        
                        builder.Append(c);
                        c = _characters.Next();
                    }

                    builder.Append(c);

                    if (_characters.HasCurrent) _characters.Next();



                    yield return new Token()
                    {
                        Type = Token.TokenType.String,
                        Data = builder.ToString(),
                    };
                }
                else if (c == '-')
                {
                    c = _characters.Next();

                    // Arrow
                    if (c == '>')
                    {
                        c = _characters.Next();

                        yield return new Token()
                        {
                            Type = Token.TokenType.Store,
                            Data = "->"
                        };
                    }

                    // Param
                    else
                    {
                        StringBuilder builder = new StringBuilder();

                        builder.Append("-"); // Consumed character

                        while (_characters.HasCurrent && IsContinueCharacter(c))
                        {
                            builder.Append(c);

                            c = _characters.Next();
                        }

                        yield return new Token()
                        {
                            Type = Token.TokenType.Param,
                            Data = builder.ToString()
                        };

                    }

                }
                else if (c == '.')
                {
                    c = _characters.Next();

                    StringBuilder builder = new StringBuilder();

                    builder.Append(".");

                    while (_characters.HasCurrent && IsContinueCharacter(c))
                    {
                        builder.Append(c);

                        c = _characters.Next();
                    }

                    yield return new Token()
                    {
                        Type = Token.TokenType.Lookup,
                        Data = builder.ToString()
                    };
                }
                else if (c == '$')
                {
                    c = _characters.Next();

                    StringBuilder builder = new StringBuilder();

                    builder.Append("$");

                    while (_characters.HasCurrent && IsContinueCharacter(c))
                    {
                        builder.Append(c);

                        c = _characters.Next();
                    }

                    yield return new Token()
                    {
                        Type = Token.TokenType.Variable,
                        Data = builder.ToString()
                    };
                }
                else if (c == '=')
                {
                    _characters.Next();

                    yield return new Token()
                    {
                        Type = Token.TokenType.Equals,
                        Data = "="
                    };
                }
                else if (c == ',')
                {
                    _characters.Next();

                    yield return new Token()
                    {
                        Type = Token.TokenType.Seperator,
                        Data = ","
                    };
                }
                else if (c == '@')
                {
                    _characters.Next();

                    yield return new Token()
                    {
                        Type = Token.TokenType.At,
                        Data = "@"
                    };
                }
                else if (c == '?')
                {
                    _characters.Next();

                    yield return new Token()
                    {
                        Type = Token.TokenType.Question,
                        Data = "?"
                    };
                }
                else if (c == ';')
                {
                    _characters.Next();

                    yield return new Token()
                    {
                        Type = Token.TokenType.LineEnd,
                        Data = ";"
                    };
                }
                else if (c == '(')
                {
                    _characters.Next();

                    yield return new Token()
                    {
                        Type = Token.TokenType.BracketOpen,
                        Data = "("
                    };
                }
                else if (c == ')')
                {
                    _characters.Next();

                    yield return new Token()
                    {
                        Type = Token.TokenType.BracketClose,
                        Data = ")"
                    };
                }

                else if (c == '{')
                {
                    _characters.Next();

                    yield return new Token()
                    {
                        Type = Token.TokenType.CurlyOpen,
                        Data = "{"
                    };
                }
                else if (c == '}')
                {
                    _characters.Next();

                    yield return new Token()
                    {
                        Type = Token.TokenType.CurlyClose,
                        Data = "}"
                    };
                }
                else if (c == '[')
                {
                    _characters.Next();

                    yield return new Token()
                    {
                        Type = Token.TokenType.ArrayOpen,
                        Data = "["
                    };
                }
                else if (c == ']')
                {
                    _characters.Next();

                    yield return new Token()
                    {
                        Type = Token.TokenType.ArrayClose,
                        Data = "]"
                    };
                }
                else if (IsCharacter(c))
                {
                    StringBuilder builder = new StringBuilder();

                    while (_characters.HasCurrent && IsContinueCharacter(c))
                    {
                        builder.Append(c);

                        c = _characters.Next();
                    }

                    var data = builder.ToString();

                    if (data == "true" || data == "false")
                    {
                        yield return new Token()
                        {
                            Type = Token.TokenType.Boolean,
                            Data = builder.ToString()
                        };
                    }
                    else if(data == "null")
                    {
                        yield return new Token()
                        {
                            Type = Token.TokenType.Null,
                            Data = builder.ToString()
                        };
                    }
                    else
                    {
                        yield return new Token()
                        {
                            Type = Token.TokenType.Identifier,
                            Data = builder.ToString()
                        };
                    }

                }
                else if (IsNumber(c))
                {
                    StringBuilder builder = new StringBuilder();

                    while (_characters.HasCurrent && IsNumber(c))
                    {
                        builder.Append(c);

                        c = _characters.Next();
                    }

                    if (c == '.')
                    {
                        builder.Append(c);
                        c = _characters.Next();

                        while (_characters.HasCurrent && IsNumber(c))
                        {
                            builder.Append(c);

                            c = _characters.Next();
                        }
                    }

                    yield return new Token()
                    {
                        Type = Token.TokenType.Number,
                        Data = builder.ToString()
                    };
                }
                // Comments!
                else if (c == '#')
                {
                    StringBuilder builder = new StringBuilder();

                    while (_characters.HasCurrent && c != '\n')
                    {
                        builder.Append(c);

                        c = _characters.Next();
                    }

                    yield return new Token()
                    {
                        Type = Token.TokenType.Comment,
                        Data = builder.ToString()
                    };
                }
                else if (IsWhiteSpace(c))
                {
                    StringBuilder builder = new StringBuilder();

                    while (_characters.HasCurrent && IsWhiteSpace(c))
                    {
                        builder.Append(c);

                        c = _characters.Next();
                    }

                    yield return new Token()
                    {
                        Type = Token.TokenType.WhiteSpace,
                        Data = builder.ToString()
                    };
                }
                else
                {
                    _errorMessages.Add($"Unexpected token: '{c}'. Ignoring.");

                    yield return new Token()
                    {
                        Type = Token.TokenType.Unknown,
                        Data = "" + c
                    };

                    _characters.Next();
                }
            }

            yield return new Token()
            {
                Type = Token.TokenType.EndOfFile,
                Data = ""
            };
        }

        public bool IsWhiteSpace(char c)
        {
            return c == ' ' || c == '\t' || c == '\r' || c == '\n';
        }
        public bool IsCharacter(char c)
        {
            return (c >= 'a' && c <= 'z')
                || (c >= 'A' && c <= 'Z')
                || c == '_';
        }
        public bool IsContinueCharacter(char c)
        {
            return IsCharacter(c)
                || IsNumber(c);
        }
        public bool IsNumber(char c)
        {
            return (c >= '0' && c <= '9');
        }

        public static Token[] Lex(string input)
        {
            Lexer lexer = new Lexer(input);
            Token[] tokens = lexer.Lex().ToArray();

            if(lexer.Messages.Count() > 0)
            {
                throw new Exception(lexer.Messages.ToArray().First());
            }

            return tokens;
        }
    }

}
