using Swoft.AST.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swoft.AST
{
    public enum OperatorType
    {
        Add,
        Sub,
        Mul,
        Div,
        Mod,
        Eq,
        EqAdd,
        EqSub,
        EqMul,
        EqDiv,
        EqMod,
        IsEq,
        IsGt,
        IsLt,
        IsGtEq,
        IsLtEq,
        Or,
        And,
        Not,
        BitwiseAnd,
        BitwiseOr,
        BitwiseNot,
        BitwiseShiftRight,
        BitwiseShiftLeft,
        AddOne,
        SubOne,
    }

    public abstract class ExpressionSyntax : SyntaxNode
    {

    }

    public class StringExpressionSyntax : ExpressionSyntax
    {
        public string Value { get; set; }

        public StringExpressionSyntax(string value)
        {
            Value = value;
        }

        public override void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class IdentifierExpressionSyntax : ExpressionSyntax
    {
        public string Identifier { get; set; }

        public IdentifierExpressionSyntax(string identifier)
        {
            Identifier = identifier;
        }

        public override void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class IntegerExpressionSyntax : ExpressionSyntax
    {
        public int Value { get; set; }

        public IntegerExpressionSyntax(int value)
        {
            Value = value;
        }

        public override void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class BinaryOperatorExpression : ExpressionSyntax
    {
        public ExpressionSyntax LeftHandSide { get; set; }
        public OperatorType Operator { get; set; }
        public ExpressionSyntax RightHandSide { get; set; }

        public BinaryOperatorExpression(ExpressionSyntax leftHandSide, OperatorType op, ExpressionSyntax rightHandSide)
        {
            LeftHandSide = leftHandSide;
            Operator = op;
            RightHandSide = rightHandSide;
        }

        public override void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class LookupExpression : ExpressionSyntax
    {
        public ExpressionSyntax LeftHandSide { get; set; }
        public string Lookup { get; set; }

        public LookupExpression(ExpressionSyntax leftHandSide, string lookup)
        {
            LeftHandSide = leftHandSide;
            Lookup = lookup;
        }

        public override void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }
    }


    public class BracketedExpressionSyntax : ExpressionSyntax
    {
        public ExpressionSyntax InBracket { get; set; }

        public BracketedExpressionSyntax(ExpressionSyntax inBracket)
        {
            InBracket = inBracket;
        }

        public override void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
    // TODO this is actually not really an expression
    public class ListExpressionSyntax : ExpressionSyntax
    {
        public ExpressionSyntax[] Syntax { get; set; }

        public ListExpressionSyntax(ExpressionSyntax[] syntax)
        {
            Syntax = syntax;
        }

        public override void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class CallExpression : ExpressionSyntax
    {
        public ExpressionSyntax Invoke { get; set; }
        public ListExpressionSyntax? Arguments { get; set; }

        public CallExpression(ExpressionSyntax functionCall, ListExpressionSyntax? arguments)
        {
            Invoke = functionCall;
            Arguments = arguments;
        }

        public override void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
