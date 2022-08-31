using Swoft.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swoft.Semantic
{
    public class Symbol
    {
    }

    public interface INamedSymbol
    {
        public string Name { get; }
    }

    public abstract class ScopeSymbol : Symbol
    {
        public List<FunctionSymbol> Functions { get; set; }
        public List<VariableSymbol> Variables { get; set; }
        public List<ScopeSymbol> Children { get; set; }
        
        public ScopeSymbol? ParentScope { get; set; }
        public byte[]? ExecuteableCode { get; set; } = null;

        public ScopeSymbol()
        {
            Functions = new List<FunctionSymbol>();
            Variables = new List<VariableSymbol>();
            Children = new List<ScopeSymbol>();

            ParentScope = null;
            ExecuteableCode = null;
        }

        public INamedSymbol? GetNamedType(string name)
        {
            var variable = Variables.FirstOrDefault(x => x.Name == name);

            if (variable != null) return variable;

            var func = Functions.FirstOrDefault(x => x.Name == name);

            if (func != null) return func;

            return ParentScope?.GetNamedType(name);
        }

        public int IndexOfChildScope(ScopeSymbol scope)
        {
            return Children.IndexOf(scope);
        }

        public abstract string GetScopeIdentifier();
    }

    public class FileSymbol : ScopeSymbol
    {
        public override string GetScopeIdentifier()
        {
            return "$";
        }
    }

    public class BlockSymbol : ScopeSymbol
    {
        public override string GetScopeIdentifier()
        {
            if (ParentScope == null) throw new Exception("Block symbol cannot live without a parent scope.");

            return "child#" + ParentScope.IndexOfChildScope(this);
        }
    }

    public class FunctionSymbol : ScopeSymbol, INamedSymbol
    {
        public string Name { get; set; }

        public bool IsExtern { get; set; }

        // TODO signature and whatnot :)
        public override string GetScopeIdentifier()
        {
            return Name;
        }
    }
    public class VariableSymbol : Symbol, INamedSymbol
    {
        public string Name { get; set; }
    }

}
