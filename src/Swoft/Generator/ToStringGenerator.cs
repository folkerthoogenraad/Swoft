using Swoft.AST;
using Swoft.AST.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swoft.Generator
{
    public class ToStringGenerator : ISyntaxVisitor
    {
        public StringBuilder Builder { get; set; }

        public ToStringGenerator()
        {
            Builder = new StringBuilder();
        }

        public void Visit(ExpressionSyntax exp)
        {
            throw new NotImplementedException();
        }
        public void Visit(StatementSyntax exp)
        {
            throw new NotImplementedException();
        }
        public void Visit(UnknownStatement exp)
        {
            throw new NotImplementedException();
        }

        public void Visit(FileSyntax file)
        {
            foreach (var s in file.Statements)
            {
                Write(s);
            }
        }

        public void Visit(ExpressionStatementSyntax statement)
        {
            Write(statement.Expression);
            WriteLine(";");
        }
        public void Visit(ReturnStatementSyntax statement)
        {
            WriteLine("return ");
            Write(statement.Expression);
            WriteLine(";");
        }

        public void Visit(BlockStatementSyntax statement)
        {
            WriteLine("{");

            foreach(var s in statement.Statements)
            {
                Write(s);
            }

            WriteLine("}");
        }

        public void Visit(IfStatementSyntax statement)
        {
            Write("if ");

            statement.Condition.Accept(this);

            statement.Body.Accept(this);

            foreach(var el in statement.ElseStatements)
            {
                Write(el);
            }
        }

        public void Visit(ElseStatementSyntax statement)
        {
            Write("else ");

            if(statement.Condition != null)
            {
                Write("if ");
                Write(statement.Condition);
            }

            statement.Body.Accept(this);
        }

        public void Visit(VariableStatementSyntax statement)
        {
            Write("var ");

            Write(statement.NameAndType);

            if(statement.Value != null)
            {
                Write(" = ");

                Write(statement.Value);
            }

            Write(";");
        }

        public void Visit(FunctionStatementSyntax statement)
        {
            Write("function ");
            Write(statement.Name);
            Write("(");
            for(int i = 0; i < statement.Parameters.Length; i++)
            {
                Write(statement.Parameters[i]);

                if(i < statement.Parameters.Length - 1)
                {
                    Write(", ");
                }
            }
            Write(")");

            if(statement.ReturnType != null)
            {
                Write(" : ");
                Write(statement.ReturnType);
            }

            WriteLine();

            if (statement.Body != null)
            {
                Write(statement.Body);
            }
            else
            {
                Write(";");
            }
        }

        public void Visit(TypeSyntax type)
        {
            Write(type.Name);
        }

        public void Visit(NameAndTypeSyntax nameAndType)
        {
            Write(nameAndType.Name);

            if (nameAndType.Type != null)
            {
                Write(" : ");
                Write(nameAndType.Type);
            }
        }

        public void Visit(StringExpressionSyntax expression)
        {
            Write(expression.Value);
        }

        public void Visit(IntegerExpressionSyntax expression)
        {
            Write("" + expression.Value);
        }

        public void Visit(IdentifierExpressionSyntax expression)
        {
            Write(expression.Identifier);
        }

        public void Visit(BinaryOperatorExpression expression)
        {
            Write(expression.LeftHandSide);
            Write($"++ { expression.Operator } ++");
            Write(expression.RightHandSide);
        }

        public void Visit(LookupExpression expression)
        {
            Write(expression.LeftHandSide);
            Write(".");              
            Write(expression.Lookup);
        }

        public void Visit(CallExpression expression)
        {
            Write(expression.Invoke);

            Write("(");

            if(expression.Arguments != null)
            {
                Write(expression.Arguments);
            }

            Write(")");
        }

        public void Visit(BracketedExpressionSyntax expression)
        {
            Write("(");
            Write(expression.InBracket);
            Write(")");
        }

        public void Visit(ListExpressionSyntax expression)
        {
            for (int i = 0; i < expression.Syntax.Length; i++)
            {
                Write(expression.Syntax[i]);

                if (i < expression.Syntax.Length - 1)
                {
                    Write(", ");
                }
            }
        }

        public void WriteLine(string s)
        {
            Builder.AppendLine(s);
        }
        public void WriteLine()
        {
            Builder.AppendLine();
        }
        public void Write(string s)
        {
            Builder.Append(s);
        }

        public void Write(SyntaxNode syntax)
        {
            syntax.Accept(this);
        }

        public static string ToString(SyntaxNode node)
        {
            var generator = new ToStringGenerator();

            generator.Write(node);

            return generator.Builder.ToString();
        }
    }
}
