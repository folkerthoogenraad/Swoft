using Swoft.AST.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swoft.AST
{
    public abstract class SyntaxNode
    {
        public abstract void Accept(ISyntaxVisitor visitor);
    }
}
