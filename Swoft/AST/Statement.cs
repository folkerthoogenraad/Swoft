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

    public class FunctionStatement : Statement
    {
        public string Name { get; set; }
        // TODO add signature
        public Statement? Body { get; set; }

        public bool IsExtern { get; set; }

        public FunctionStatement(string name, Statement? body, bool isExtern)
        {
            Name = name;
            Body = body;
            IsExtern = isExtern;
        }
    }
}
