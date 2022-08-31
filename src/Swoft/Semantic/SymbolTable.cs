using Swoft.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swoft.Semantic
{
    public class SymbolTable
    {
        private Dictionary<SyntaxNode, Symbol> _symbols;
        private ScopeSymbol _root;

        public SymbolTable()
        {
            _symbols = new Dictionary<SyntaxNode, Symbol>();
        }

        public void RegisterSymbol(SyntaxNode node, Symbol symbol)
        {
            // hacky but works for now
            if(node is FileSyntax)
            {
                _root = (FileSymbol) symbol;
            }

            _symbols.Add(node, symbol);
        }

        public ScopeSymbol GetRootScope()
        {
            return _root;
        }

        public Symbol GetSymbol(SyntaxNode node)
        {
            if(_symbols.TryGetValue(node, out var symbol)){
                return symbol;
            }

            throw new InvalidOperationException("Symbol doesn't exist.");
        }

        public ScopeSymbol GetScope(SyntaxNode node)
        {
            return GetSymbol<ScopeSymbol>(node);
        }

        public T GetSymbol<T>(SyntaxNode node)
            where T : Symbol
        {
            var s = GetSymbol(node);

            if(s is not T)
            {
                throw new InvalidOperationException("Symbol is not of requested type.");
            }
            else
            {
                return (T)s;
            }
        }

        public bool HasSymbolFor(SyntaxNode node)
        {
            return _symbols.ContainsKey(node);
        }
    }
}
