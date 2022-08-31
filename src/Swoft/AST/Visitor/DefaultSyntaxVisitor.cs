using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swoft.AST.Visitor
{
    public class DefaultSyntaxVisitor : ISyntaxVisitor
    {
        public virtual void Visit(ExpressionSyntax exp)
        {
        }
        public virtual void Visit(StatementSyntax exp)
        {
        }
        public virtual void Visit(UnknownStatement exp)
        {
        }
        public virtual void Visit(FileSyntax file)
        {
        }
        public virtual void Visit(ExpressionStatement statement)
        {
        }
        public virtual void Visit(BlockStatementSyntax statement)
        {
        }
        public virtual void Visit(IfStatementSyntax statement)
        {
        }
        public virtual void Visit(ElseStatementSyntax statement)
        {
        }
        public virtual void Visit(VariableStatementSyntax statement)
        {
        }
        public virtual void Visit(FunctionStatementSyntax statement)
        {
        }
        public virtual void Visit(TypeSyntax syntax)
        {
        }
        public virtual void Visit(NameAndTypeSyntax syntax)
        {
        }
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
        }
        public virtual void Visit(LookupExpression expression)
        {
        }
        public virtual void Visit(CallExpression expression)
        {
        }
        public virtual void Visit(BracketedExpressionSyntax expression)
        {
        }
        public virtual void Visit(ListExpressionSyntax expression)
        {
        }
    }
}
