using Swoft.AST;
using Swoft.AST.Visitor;
using Swoft.Semantic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swoft.IL
{
    public class ILGenerator : AllSyntaxVisitor
    {
        private ILWriter? _writer;
        private Dictionary<ScopeSymbol, ILWriter> _writers;
        private SymbolTable _table;

        private ScopeSymbol? _scope;
        private Stack<ScopeSymbol> _scopeStack;

        public ILGenerator(SymbolTable table)
        {
            _scopeStack = new Stack<ScopeSymbol>();

            _writers = new Dictionary<ScopeSymbol, ILWriter>();
            _table = table;
        }

        // ===================================================== //
        // Scope changes
        // ===================================================== //
        public override void Visit(FileSyntax file)
        {
            PushScope(_table.GetScope(file));

            base.Visit(file);
            
            PopScope();
        }
        public override void Visit(BlockStatementSyntax block)
        {
            Debug.Assert(_writer != null);

            var targetScope = _table.GetScope(block);

            _writer.WriteIL(ILCode.JumpToScope);
            _writer.WriteString(ScopePath.Create(targetScope));

            PushScope(targetScope);

            base.Visit(block);

            _writer.WriteIL(ILCode.PushConstInt);
            _writer.WriteInt(0);
            _writer.WriteIL(ILCode.Return);
            
            PopScope();
        }

        public override void Visit(FunctionStatementSyntax statement)
        {
            Debug.Assert(_writer != null);

            var targetScope = _table.GetScope(statement);

            PushScope(targetScope);

            foreach(var param in statement.Parameters)
            {
                _writer.WriteIL(ILCode.StoreVariable);
                _writer.WriteString(param.Name);
            }

            base.Visit(statement);
            // HAck because we don't have anything to return
            _writer.WriteIL(ILCode.Return);

            PopScope();

        }
        public override void Visit(ReturnStatementSyntax statement)
        {
            Debug.Assert(_writer != null);

            base.Visit(statement);

            _writer.WriteIL(ILCode.Return);
        }
        public override void Visit(ExpressionStatementSyntax statement)
        {
            Debug.Assert(_writer != null);

            base.Visit(statement);

            _writer.WriteIL(ILCode.Pop);
        }

        // ===================================================== //
        // Misc
        // ===================================================== //
        public override void Visit(VariableStatementSyntax statement)
        {
            Debug.Assert(_writer != null);

            if (statement.Value != null)
            {
                GenerateIL(statement.Value);

                _writer.WriteIL(ILCode.StoreVariable);
                _writer.WriteString(statement.NameAndType.Name);
            }
        }

        // ===================================================== //
        // Expressions
        // ===================================================== //
        public override void Visit(StringExpressionSyntax expression)
        {
            Debug.Assert(_writer != null);

            _writer.WriteIL(ILCode.PushConstString);
            _writer.WriteString(expression.Value.Substring(1, expression.Value.Length - 2));
        }
        public override void Visit(IntegerExpressionSyntax expression)
        {
            Debug.Assert(_writer != null);

            _writer.WriteIL(ILCode.PushConstInt);
            _writer.WriteInt(expression.Value);
        }
        public override void Visit(IdentifierExpressionSyntax expression)
        {
            Debug.Assert(_writer != null);
            Debug.Assert(_scope != null);

            var namedType = _scope.GetNamedType(expression.Identifier);

            if(namedType is FunctionSymbol)
            {
                _writer.WriteIL(ILCode.PushFunction);
                _writer.WriteString(expression.Identifier);
            }
            else if(namedType is VariableSymbol)
            {
                _writer.WriteIL(ILCode.PushVariable);
                _writer.WriteString(expression.Identifier);
            }
            else
            {
                throw new ApplicationException("Unknown identifier type.");
            }
        }
        public override void Visit(BinaryOperatorExpression expression)
        {
            GenerateIL(expression.LeftHandSide);
            GenerateIL(expression.RightHandSide);

            throw new NotImplementedException();
        }
        public override void Visit(LookupExpression expression)
        {
            throw new NotImplementedException();
        }
        public override void Visit(CallExpression expression)
        {
            Debug.Assert(_writer != null);

            int count = 0;

            if(expression.Arguments != null)
            {
                GenerateIL(expression.Arguments);
                count = expression.Arguments.Syntax.Length;
            }

            GenerateIL(expression.Invoke);
            _writer.WriteIL(ILCode.Invoke);
            _writer.WriteInt(count);
        }
        public override void Visit(BracketedExpressionSyntax expression)
        {
            GenerateIL(expression.InBracket);
        }
        public override void Visit(ListExpressionSyntax expression)
        {
            foreach (var item in expression.Syntax)
            {
                GenerateIL(item);
            }
        }

        public void Commit()
        {
            foreach(var (context, writer) in _writers)
            {
                context.ExecuteableCode = writer.GetBytes();
            }
        }

        public void GenerateIL(SyntaxNode node)
        {
            node.Accept(this);
        }

        private void PushScope(ScopeSymbol scope)
        {
            _scope = scope;
            _scopeStack.Push(scope);

            SyncWriter();
        }

        private void PopScope()
        {
            _scopeStack.Pop();

            if(_scopeStack.Count > 0)
            {
                _scope = _scopeStack.Peek();
            }
            else
            {
                _scope = null;
            }

            SyncWriter();
        }

        private void SyncWriter()
        {
            if(_scope == null)
            {
                _writer = null;
                return;
            }

            if(_writers.TryGetValue(_scope, out var writer)) 
            {
                _writer = writer;
                return;
            }

            _writer = new ILWriter();
            _writers.Add(_scope, _writer);
        }

    }
}
