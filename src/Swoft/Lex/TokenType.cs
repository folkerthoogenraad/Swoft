using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swoft.Lex
{
    public enum TokenType
    {
        Unknown,

        KeywordFunction,
        KeywordVar,
        KeywordStruct,
        KeywordProcedure,
        KeywordIf,
        KeywordElse,
        KeywordWhile,
        KeywordWhen,
        KeywordAwait,

        ModifierPublic,
        ModifierExtern,

        OperatorAdd,
        OperatorSub,
        OperatorMul,
        OperatorDiv,
        OperatorMod,
        OperatorEq,
        OperatorEqAdd,
        OperatorEqSub,
        OperatorEqMul,
        OperatorEqDiv,
        OperatorEqMod,
        OperatorIsEq,
        OperatorIsGt,
        OperatorIsLt,
        OperatorIsGtEq,
        OperatorIsLtEq,
        OperatorOr,
        OperatorAnd,
        OperatorNot,
        OperatorBitwiseAnd,
        OperatorBitwiseOr,
        OperatorBitwiseNot,
        OperatorBitwiseShiftRight,
        OperatorBitwiseShiftLeft,
        OperatorAddOne,
        OperatorSubOne,

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

    public static class TokenTypeExtensions
    {
        public static bool IsKeyword(this TokenType type)
        {
            switch (type)
            {
                case TokenType.KeywordFunction:
                case TokenType.KeywordStruct:
                case TokenType.KeywordProcedure:
                case TokenType.KeywordIf:
                case TokenType.KeywordElse:
                case TokenType.KeywordWhile:
                case TokenType.KeywordWhen:
                case TokenType.KeywordAwait:
                    return true;

                default: return false;
            }
        }

        public static bool IsOperator(this TokenType type)
        {
            switch(type)
            {
                case TokenType.OperatorAdd:
                case TokenType.OperatorSub:
                case TokenType.OperatorMul:
                case TokenType.OperatorDiv:
                case TokenType.OperatorMod:
                case TokenType.OperatorEq:
                case TokenType.OperatorEqAdd:
                case TokenType.OperatorEqSub:
                case TokenType.OperatorEqMul:
                case TokenType.OperatorEqDiv:
                case TokenType.OperatorEqMod:
                case TokenType.OperatorIsEq:
                case TokenType.OperatorIsGt:
                case TokenType.OperatorIsLt:
                case TokenType.OperatorIsGtEq:
                case TokenType.OperatorIsLtEq:
                case TokenType.OperatorOr:
                case TokenType.OperatorAnd:
                case TokenType.OperatorNot:
                case TokenType.OperatorBitwiseAnd:
                case TokenType.OperatorBitwiseOr:
                case TokenType.OperatorBitwiseNot:
                case TokenType.OperatorBitwiseShiftRight:
                case TokenType.OperatorBitwiseShiftLeft:
                case TokenType.OperatorAddOne:
                case TokenType.OperatorSubOne:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsMeta(this TokenType type)
        {
            switch(type)
            {
                case TokenType.Unknown:
                case TokenType.Comment:
                case TokenType.Whitespace:
                    return true;
                default:
                    return false;
            }
        }
        
        public static bool IsModifier(this TokenType type)
        {
            switch(type)
            {
                case TokenType.ModifierPublic:
                case TokenType.ModifierExtern:
                    return true;
                default:
                    return false;
            }
        }
    }
}
