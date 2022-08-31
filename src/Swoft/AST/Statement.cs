using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swoft.AST
{
    public class Statement : SyntaxNode
    {

    }

    public class ExpressionStatement : Statement
    {
        public Expression Expression { get; set; }

        public ExpressionStatement(Expression expression)
        {
            Expression = expression;
        }
    }

    public class BlockStatement : Statement
    {
        public Statement[] Statements { get; set; }

        public BlockStatement(Statement[] statements)
        {
            Statements = statements;
        }
    }

    public class TypeSyntax
    {
        public string Name { get; set; }

        public TypeSyntax(string name)
        {
            Name = name;
        }
    }

    public class NameAndTypeSyntax
    {
        public string Name { get; set; }
        public TypeSyntax? Type { get; set; }

        public NameAndTypeSyntax(string name, TypeSyntax? type)
        {
            Name = name;
            Type = type;
        }
    }

    public class FunctionStatement : Statement
    {
        public string Name { get; set; }
        public TypeSyntax? ReturnType { get; set; }
        public IList<NameAndTypeSyntax> Parameters { get; set; }
        public Statement? Body { get; set; }

        public bool IsExtern { get; set; }
        public bool IsStatic { get; set; }

        public FunctionStatement(string name, Statement? body, bool isExtern, bool isStatic)
        {
            Name = name;
            Body = body;
            IsExtern = isExtern;
            IsStatic = isStatic;
            ReturnType = null;
            Parameters = new List<NameAndTypeSyntax>();
        }
    }
}
