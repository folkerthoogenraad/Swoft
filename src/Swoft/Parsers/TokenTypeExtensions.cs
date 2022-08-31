using Swoft.AST;
using Swoft.Lex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swoft.Parsers
{
    public static class TokenTypeExtensions 
    {
        public static OperatorType ToOperatorType(TokenType type)
        {
            switch(type){
                case TokenType.OperatorAdd: return OperatorType.Add;
                case TokenType.OperatorSub: return OperatorType.Sub;
                case TokenType.OperatorMul: return OperatorType.Mul;
                case TokenType.OperatorDiv: return OperatorType.Div;
                case TokenType.OperatorMod: return OperatorType.Mod;
                case TokenType.OperatorEq: return OperatorType.Eq;
                case TokenType.OperatorEqAdd: return OperatorType.EqAdd;
                case TokenType.OperatorEqSub: return OperatorType.EqSub;
                case TokenType.OperatorEqMul: return OperatorType.EqMul;
                case TokenType.OperatorEqDiv: return OperatorType.EqDiv;
                case TokenType.OperatorEqMod: return OperatorType.EqMod;
                case TokenType.OperatorIsEq: return OperatorType.IsEq;
                case TokenType.OperatorIsGt: return OperatorType.IsGt;
                case TokenType.OperatorIsLt: return OperatorType.IsLt;
                case TokenType.OperatorIsGtEq: return OperatorType.IsGtEq;
                case TokenType.OperatorIsLtEq: return OperatorType.IsLtEq;
                case TokenType.OperatorOr: return OperatorType.Or;
                case TokenType.OperatorAnd: return OperatorType.And;
                case TokenType.OperatorNot: return OperatorType.Not;
                case TokenType.OperatorBitwiseAnd: return OperatorType.BitwiseAnd;
                case TokenType.OperatorBitwiseOr: return OperatorType.BitwiseOr;
                case TokenType.OperatorBitwiseNot: return OperatorType.BitwiseNot;
                case TokenType.OperatorBitwiseShiftRight: return OperatorType.BitwiseShiftRight;
                case TokenType.OperatorBitwiseShiftLeft: return OperatorType.BitwiseShiftLeft;
                case TokenType.OperatorAddOne: return OperatorType.AddOne;
                case TokenType.OperatorSubOne: return OperatorType.SubOne;
            }
            
            throw new ApplicationException("Not a (known) operator " + type);
        }
    }
}
