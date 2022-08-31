using Swoft.AST.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swoft.AST
{
    public abstract class StatementSyntax : SyntaxNode
    {

    }

    public class UnknownStatement : StatementSyntax
    {
        public override void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
    public class FileSyntax : SyntaxNode
    {
        public StatementSyntax[] Statements { get; set; }

        public FileSyntax(StatementSyntax[] statements)
        {
            Statements = statements;
        }

        public override void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class ExpressionStatement : StatementSyntax
    {
        public ExpressionSyntax Expression { get; set; }

        public ExpressionStatement(ExpressionSyntax expression)
        {
            Expression = expression;
        }

        public override void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class BlockStatementSyntax : StatementSyntax
    {
        public StatementSyntax[] Statements { get; set; }

        public BlockStatementSyntax(StatementSyntax[] statements)
        {
            Statements = statements;
        }

        public override void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class TypeSyntax : SyntaxNode
    {
        public string Name { get; set; }

        public TypeSyntax(string name)
        {
            Name = name;
        }

        public override void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class NameAndTypeSyntax : SyntaxNode
    {
        public string Name { get; set; }
        public TypeSyntax? Type { get; set; }

        public NameAndTypeSyntax(string name, TypeSyntax? type)
        {
            Name = name;
            Type = type;
        }

        public override void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class IfStatementSyntax : StatementSyntax
    {
        public ExpressionSyntax Condition;
        public ElseStatementSyntax[] ElseStatements { get; set; }
        public StatementSyntax Body { get; set; }

        public IfStatementSyntax(ExpressionSyntax condition, StatementSyntax body, ElseStatementSyntax[] elseStatements)
        {
            Condition = condition;
            Body = body;
            ElseStatements = elseStatements;
        }

        public override void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class ElseStatementSyntax : StatementSyntax
    {
        public ExpressionSyntax? Condition;
        public StatementSyntax Body;

        public ElseStatementSyntax(ExpressionSyntax? condition, StatementSyntax body)
        {
            Condition = condition;
            Body = body;
        }

        public override void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class VariableStatementSyntax : StatementSyntax
    {
        public NameAndTypeSyntax NameAndType { get; set; }
        public ExpressionSyntax? Value { get; set; }

        public VariableStatementSyntax(NameAndTypeSyntax nameAndType, ExpressionSyntax? value)
        {
            this.NameAndType = nameAndType;
            this.Value = value;
        }

        public override void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class ModifiersSyntax
    {
        public bool IsStatic { get; set; }
        public bool IsExtern { get; set; }

        public ModifiersSyntax(bool isStatic, bool isExtern)
        {
            IsStatic = isStatic;
            IsExtern = isExtern;
        }
    }

    public class FunctionStatementSyntax : StatementSyntax
    {
        public string Name { get; set; }
        public TypeSyntax? ReturnType { get; set; }
        public NameAndTypeSyntax[] Parameters { get; set; }
        public StatementSyntax? Body { get; set; }

        public ModifiersSyntax? Modifiers;

        public FunctionStatementSyntax(string name, NameAndTypeSyntax[] parameters, StatementSyntax? body, TypeSyntax? returnType, ModifiersSyntax? modifiers)
        {
            Name = name;
            Body = body;
            Parameters = parameters;
            Modifiers = modifiers;
            ReturnType = returnType;
        }

        public override void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
