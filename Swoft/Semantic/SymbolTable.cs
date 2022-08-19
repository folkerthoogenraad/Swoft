using Swoft.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swoft.Semantic
{
    public class FunctionSignature
    {
        public TypeReference ReturnType { get; set; }
        public IList<TypeReference> Parameters { get; set; }

        public FunctionSignature(TypeReference returnType)
        {
            ReturnType = returnType;
            Parameters = new List<TypeReference>();
        }
    }
    public abstract class NameAndTypeDefinition
    {
        public string Name { get; set; }
        public TypeReference Type { get; set; }

        protected NameAndTypeDefinition(string name, TypeReference type)
        {
            Name = name;
            Type = type;
        }
    }
    public class VariableDefinition : NameAndTypeDefinition
    {

        public VariableDefinition(string name, TypeReference type) : base(name, type)
        {
        }
    }
    public class FieldDefinition : NameAndTypeDefinition
    {
        public FieldDefinition(string name, TypeReference type) : base(name, type)
        {
        }
    }
    public class ParameterDefinition : NameAndTypeDefinition
    {
        public ParameterDefinition(string name, TypeReference type) : base(name, type)
        {
        }
    }

    public abstract class TypeReference
    {
    }

    public class StructTypeReference : TypeReference
    {
        public StructDefinition Definition { get; set; }

        public StructTypeReference(StructDefinition definition)
        {
            Definition = definition;
        }
    }
    public class FunctionTypeReference : TypeReference
    {
        public FunctionDefinition Definition { get; set; }

        public FunctionTypeReference(FunctionDefinition definition)
        {
            Definition = definition;
        }
    }
    public class LambdaTypeReference : TypeReference
    {
        public FunctionSignature Signature { get; set; }

        public LambdaTypeReference(FunctionSignature signature)
        {
            Signature = signature;
        }
    }
    public class VoidTypeReference : TypeReference
    {

    }

    public class FunctionDefinition : IDefinitionContext
    {
        public IDefinitionContext? Parent { get; set; }
        public FunctionSignature Signature { get; set; }
        public bool IsStatic { get; set; } = false;
        public bool IsExtern { get; set; } = false;
        public string Name { get; set; }
        public IList<ParameterDefinition> Parameters { get; set; }
        public IList<VariableDefinition> Variables { get; set; }

        public FunctionDefinition(IDefinitionContext? context, string name, FunctionSignature signature)
        {
            Name = name;
            Signature = signature;

            Parameters = new List<ParameterDefinition>();
            Variables = new List<VariableDefinition>();

            Parent = context;
        }

        public IEnumerable<StructDefinition> ResolveStructsByName(string name)
        {
            return Parent?.ResolveStructsByName(name) ?? Array.Empty<StructDefinition>();
        }
        public IEnumerable<FunctionDefinition> ResolveFunctionsByName(string name)
        {
            return Parent?.ResolveFunctionsByName(name) ?? Array.Empty<FunctionDefinition>();
        }
        public IEnumerable<NameAndTypeDefinition> ResolveNameAndTypes(string name)
        {
            var variables = Variables.Where(f => f.Name == name);

            foreach (var f in variables)
            {
                yield return f;
            }

            var parameters = Parameters.Where(f => f.Name == name);

            foreach (var f in parameters)
            {
                yield return f;
            }

            var parentNames = Parent?.ResolveNameAndTypes(name);

            if (parentNames == null) yield break;

            foreach (var f in parentNames)
            {
                yield return f;
            }
        }
    }

    public class StructDefinition : IDefinitionContext
    {
        public IDefinitionContext? Parent { get; set; }
        public string Name { get; set; }
        public IList<FieldDefinition> Fields { get; set; }
        public IList<FunctionDefinition> Functions { get; set; }

        public StructDefinition(IDefinitionContext? parent, string name)
        {
            Name = name;
            Fields = new List<FieldDefinition>();
            Functions = new List<FunctionDefinition>();
            Parent = parent;
        }

        public IEnumerable<StructDefinition> ResolveStructsByName(string name)
        {
            return Parent?.ResolveStructsByName(name) ?? Array.Empty<StructDefinition>();
        }

        public IEnumerable<FunctionDefinition> ResolveFunctionsByName(string name)
        {
            var self = Functions.Where(f => f.Name == name);

            foreach (var f  in self)
            {
                yield return f;
            }

            var parentFunctions = Parent?.ResolveFunctionsByName(name);

            if(parentFunctions == null) yield break;

            foreach (var f in parentFunctions)
            {
                yield return f;
            }
        }

        public IEnumerable<NameAndTypeDefinition> ResolveNameAndTypes(string name)
        {
            return Fields.Where(f => f.Name == name);
        }
    }

    public class AssemblyDefinition : IDefinitionContext
    {
        public IList<StructDefinition> Structs { get; set; }
        public IList<FunctionDefinition> Functions { get; set; }
        public IList<VariableDefinition> Variables { get; set; }

        public AssemblyDefinition()
        {
            Structs = new List<StructDefinition>();
            Functions = new List<FunctionDefinition>();
            Variables = new List<VariableDefinition>();
        }

        public IEnumerable<StructDefinition> ResolveStructsByName(string name)
        {
            return Structs.Where(f => f.Name == name);
        }

        public IEnumerable<FunctionDefinition> ResolveFunctionsByName(string name)
        {
            return Functions.Where(f => f.Name == name);
        }

        public IEnumerable<NameAndTypeDefinition> ResolveNameAndTypes(string name)
        {
            return Variables.Where(f => f.Name == name);
        }
    }

    public interface IDefinitionContext
    {
        public IEnumerable<StructDefinition> ResolveStructsByName(string name);
        public IEnumerable<FunctionDefinition> ResolveFunctionsByName(string name);
        public IEnumerable<NameAndTypeDefinition> ResolveNameAndTypes(string name);
    }

    public class ExpressionSymbol
    {
        public TypeReference? ResolvedType { get; set; }
        public IList<TypeReference> CandidateTypes { get; set; }

        public ExpressionSymbol(TypeReference resolved)
        {
            ResolvedType = resolved;
            CandidateTypes = new List<TypeReference>() { resolved };
        }
        public ExpressionSymbol(IEnumerable<TypeReference> candidates)
        {
            ResolvedType = null;
            CandidateTypes = candidates.ToList();
        }

        public ExpressionSymbol()
        {
            ResolvedType = null;
            CandidateTypes = new List<TypeReference>();
        }
    }

    public class SymbolTable
    {
        public Dictionary<Expression, ExpressionSymbol> ExpressionSymbols { get; set; }
        public Dictionary<FunctionStatement, FunctionDefinition> FunctionDefinitions { get; set; }

        public SymbolTable()
        {
            ExpressionSymbols = new Dictionary<Expression, ExpressionSymbol>();
            FunctionDefinitions = new Dictionary<FunctionStatement, FunctionDefinition>();
        }
    }



    // var f = 0;
    // f >> int
    // var f = (float a) => return 0;
    // f >> function(float): int
}
