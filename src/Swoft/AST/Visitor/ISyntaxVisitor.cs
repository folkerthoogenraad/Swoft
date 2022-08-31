using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swoft.AST.Visitor
{
    public interface ISyntaxVisitor
    {
        // Error/unimplemented visits
        public void Visit(ExpressionSyntax exp);
        public void Visit(StatementSyntax exp);
        public void Visit(UnknownStatement exp);

        public void Visit(FileSyntax exp);

        // Statements
        public void Visit(ExpressionStatementSyntax statement);
        public void Visit(ReturnStatementSyntax statement);
        public void Visit(BlockStatementSyntax statement);
        public void Visit(IfStatementSyntax statement);
        public void Visit(ElseStatementSyntax statement);
        public void Visit(VariableStatementSyntax statement);
        public void Visit(FunctionStatementSyntax statement);

        // Misc
        public void Visit(TypeSyntax type);
        public void Visit(NameAndTypeSyntax nameAndType);

        // Expressions
        public void Visit(StringExpressionSyntax expression);
        public void Visit(IntegerExpressionSyntax expression);
        public void Visit(IdentifierExpressionSyntax expression);
        public void Visit(BinaryOperatorExpression expression);
        public void Visit(LookupExpression expression);
        public void Visit(CallExpression expression);
        public void Visit(BracketedExpressionSyntax expression);
        public void Visit(ListExpressionSyntax expression); 
    }
}
