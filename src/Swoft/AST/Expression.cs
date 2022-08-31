using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swoft.AST
{
    public abstract class Expression : SyntaxNode
    {

    }

    public class StringExpression : Expression
    {
        public string Value { get; set; }

        public StringExpression(string value)
        {
            Value = value;
        }
    }

    public class IdentifierExpression : Expression
    {
        public string Identifier { get; set; }

        public IdentifierExpression(string identifier)
        {
            Identifier = identifier;
        }
    }

    public class IntegerExpression : Expression
    {
        public int Value { get; set; }

        public IntegerExpression(int value)
        {
            Value = value;
        }
    }

    public class BinaryOperatorExpression : Expression
    {
        public Expression LeftHandSide { get; set; }
        public Expression RightHandSide { get; set; }

        public BinaryOperatorExpression(Expression leftHandSide, Expression rightHandSide)
        {
            LeftHandSide = leftHandSide;
            RightHandSide = rightHandSide;
        }
    }


    public class CallExpression : Expression
    {
        public Expression Invoke { get; set; }
        public Expression[] Arguments { get; set; }

        // TODO generics

        public CallExpression(Expression functionCall, Expression[] arguments)
        {
            Invoke = functionCall;
            Arguments = arguments;
        }
    }
}
