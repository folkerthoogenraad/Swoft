using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swoft.AST.Visitor
{
    public class AllSyntaxVisitor : ISyntaxVisitor
    {
        // Error/unimplemented visits
        public virtual void Visit(ExpressionSyntax exp)
        {
            throw new NotImplementedException();
        }
        public virtual void Visit(StatementSyntax exp)
        {
            throw new NotImplementedException();
        }
        public virtual void Visit(UnknownStatement exp)
        {
            throw new NotImplementedException();
        }

        public virtual void Visit(FileSyntax file)
        {
            foreach (var st in file.Statements)
            {
                st.Accept(this);
            }
        }

        // Statements
        public virtual void Visit(ExpressionStatement statement)
        {
            statement.Expression.Accept(this);
        }
        public virtual void Visit(BlockStatementSyntax statement)
        {
            foreach(var st in statement.Statements)
            {
                st.Accept(this);
            }
        }
        public virtual void Visit(IfStatementSyntax statement)
        {
            statement.Condition.Accept(this);
            statement.Body.Accept(this);

            foreach(var el in statement.ElseStatements)
            {
                el.Accept(this);
            }
        }
        public virtual void Visit(ElseStatementSyntax statement)
        {
            statement.Condition?.Accept(this);
            statement.Body.Accept(this);
        }
        public virtual void Visit(VariableStatementSyntax statement)
        {
            statement.NameAndType.Accept(this);
            statement.Value?.Accept(this);
        }
        public virtual void Visit(FunctionStatementSyntax statement)
        {
            foreach(var parameter in statement.Parameters)
            {
                parameter.Accept(this);
            }
            statement.ReturnType?.Accept(this);

            statement.Body?.Accept(this);
        }

        // Misc
        public virtual void Visit(TypeSyntax syntax)
        {
            
        }
        public virtual void Visit(NameAndTypeSyntax syntax)
        {
            syntax.Type?.Accept(this);
        }

        // Expressions
        public virtual void Visit(StringExpressionSyntax expression)
        {

        }
        public virtual void Visit(IntegerExpressionSyntax expression)
        {

        }
        public virtual void Visit(IdentifierExpressionSyntax expression)
        {

        }
        public virtual void Visit(BinaryOperatorExpression expression)
        {
            expression.LeftHandSide.Accept(this);
            expression.RightHandSide.Accept(this);
        }
        public virtual void Visit(LookupExpression expression)
        {
            expression.LeftHandSide.Accept(this);
        }
        public virtual void Visit(CallExpression expression)
        {
            expression.Invoke.Accept(this);
            expression.Arguments?.Accept(this);
        }
        public virtual void Visit(BracketedExpressionSyntax expression)
        {
            expression.InBracket.Accept(this);
        }
        public virtual void Visit(ListExpressionSyntax expression)
        {
            foreach (var item in expression.Syntax)
            {
                item.Accept(this);
            }
        }
    }
}
