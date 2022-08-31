using Swoft.AST;
using Swoft.AST.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swoft.Semantic
{
    public class SemanticGenerator : ISyntaxVisitor
    {
        public SymbolTable Table { get; set; }
        public ScopeSymbol CurrentScope { get; set; }

        public SemanticGenerator()
        {
            Table = new SymbolTable();

            // This is to stop the thing from complaining...
            // This is in fact incorrect as the FileSyntax now doesn't push
            // its scope. If you analyze multiple files it will be in the 
            // same scope.
            CurrentScope = new FileSymbol();
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
            Table.RegisterSymbol(file, CurrentScope);

            // Scope already exists because its created at the start.
            foreach (var s in file.Statements)
            {
                Analyze(s);
            }
        }

        public void Visit(ExpressionStatement statement)
        {
            // For now we don't analyze expressions.
            // Analyze(statement.Expression);
        }

        public void Visit(BlockStatementSyntax statement)
        {
            var symbol = new BlockSymbol();

            Table.RegisterSymbol(statement, symbol);

            PushScope(symbol);

            foreach(var s in statement.Statements)
            {
                Analyze(s);
            }

            PopScope();
        }

        public void Visit(IfStatementSyntax statement)
        {
            // TODO if statements gun be weird, I tell you.
            Analyze(statement.Condition);
            Analyze(statement.Body);

            foreach(var el in statement.ElseStatements)
            {
                Analyze(el);
            }
        }

        public void Visit(ElseStatementSyntax statement)
        {
            if(statement.Condition != null)
            {
                Analyze(statement.Condition);
            }

            Analyze(statement.Body);
        }

        public void Visit(VariableStatementSyntax statement)
        {
            Analyze(statement.NameAndType);

            // Value is generated in a different pass and can be ignored for now.
        }

        public void Visit(FunctionStatementSyntax statement)
        {
            // Funcions have functions scope, but if they ahve a block they also ahve block scope. This is intended.
            var functionSymbol = new FunctionSymbol();
            functionSymbol.Name = statement.Name;
            functionSymbol.IsExtern = statement.Modifiers?.IsExtern ?? false;

            CurrentScope.Functions.Add(functionSymbol);

            Table.RegisterSymbol(statement, functionSymbol);

            PushScope(functionSymbol);

            foreach(var param in statement.Parameters)
            {
                // Analyze the parameter within the function scope :)
                Analyze(param);
            }

            if(statement.Body != null)
            {
                Analyze(statement.Body);
            }

            PopScope();
        }

        public void Visit(TypeSyntax type)
        {
            // No need for now? :) 
        }

        public void Visit(NameAndTypeSyntax nameAndType)
        {
            VariableSymbol symbol = new VariableSymbol();
            symbol.Name = nameAndType.Name;

            // For now we don't need this but at some point I want _all_ the symbols, not just this
            Table.RegisterSymbol(nameAndType, symbol);

            CurrentScope.Variables.Add(symbol);
        }

        // ========================================================= //
        // This all should never happen in this pass!
        // ========================================================= //
        public void Visit(StringExpressionSyntax expression)
        {
            throw new NotImplementedException();
        }

        public void Visit(IntegerExpressionSyntax expression)
        {
            throw new NotImplementedException();
        }

        public void Visit(IdentifierExpressionSyntax expression)
        {
            throw new NotImplementedException();
        }

        public void Visit(BinaryOperatorExpression expression)
        {
            throw new NotImplementedException();
        }

        public void Visit(LookupExpression expression)
        {
            throw new NotImplementedException();
        }

        public void Visit(CallExpression expression)
        {
            throw new NotImplementedException();
        }

        public void Visit(BracketedExpressionSyntax expression)
        {
            throw new NotImplementedException();
        }

        public void Visit(ListExpressionSyntax expression)
        {
            throw new NotImplementedException();
        }

        // ========================================================= //
        // Pushing and popping scopes :)
        // ========================================================= //
        public void PushScope(ScopeSymbol scope)
        {
            scope.ParentScope = CurrentScope;
            CurrentScope.Children.Add(scope);

            CurrentScope = scope;
        }

        public void PopScope()
        {
            if(CurrentScope.ParentScope == null)
            {
                throw new InvalidOperationException("Cannot pop scope when no parent scope is available.");
            }

            CurrentScope = CurrentScope.ParentScope;
        }

        public void Analyze(SyntaxNode syntax)
        {
            syntax.Accept(this);
        }
    }
}
