using Swoft.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swoft.Semantic
{
    public class SemanticAnalyzer
    {
        public AssemblyDefinition AssemblyDefinition { get; set; }
        public SymbolTable SymbolTable { get; set; }

        public SemanticAnalyzer()
        {
            SymbolTable = new SymbolTable();
            AssemblyDefinition = new AssemblyDefinition();
        }

        public void Analyze(Statement[] statements)
        {
            foreach(var statement in statements)
            {
                Analyze(AssemblyDefinition, statement);
            }
        }

        public void Analyze(IDefinitionContext context, Statement statement)
        {
            if(statement is FunctionStatement func)
            {
                var name = func.Name;
                var returnType = ResolveType(context, func.ReturnType);
                var parameter = func.Parameters.Select(x => (Syntax: x, Type: ResolveType(context, x.Type)));

                var signature = new FunctionSignature(returnType);
                signature.Parameters = parameter.Select(x => x.Type).ToList(); // Add range but whatever

                var funcDefinition = new FunctionDefinition(context, name, signature);
                funcDefinition.IsExtern = func.IsExtern;
                funcDefinition.IsStatic = func.IsStatic;
                funcDefinition.Parameters = parameter.Select(p => new ParameterDefinition(p.Syntax.Name, p.Type)).ToList(); // Add range whatever

                // TODO all storts of modifiers and whatnot

                if(context is AssemblyDefinition assembly)
                {
                    assembly.Functions.Add(funcDefinition);
                }
                else if (context is StructDefinition struc)
                {
                    struc.Functions.Add(funcDefinition);
                }

                if(func.Body != null)
                {
                    Analyze(funcDefinition, func.Body);
                }
            }
            else if(statement is BlockStatement block)
            {
                foreach(var innerStatement in block.Statements)
                {
                    // TODO new inner scope stuff!
                    Analyze(context, statement);
                }
            }
            else if(statement is ExpressionStatement expressionStatement)
            {
                AnalyzeExpression(context, expressionStatement.Expression);
            }
        }

        public ExpressionSymbol AnalyzeExpression(IDefinitionContext context, Expression expression)
        {
            if(SymbolTable.ExpressionSymbols.TryGetValue(expression, out var symbol)){
                return symbol;
            }

            symbol = GetSymbol(context, expression);

            SymbolTable.ExpressionSymbols.Add(expression, symbol);

            return symbol;
        }

        public ExpressionSymbol GetSymbol(IDefinitionContext context, Expression expression)
        {
            if (expression is CallExpression callExpression)
            {
                var method = AnalyzeExpression(context, callExpression.Invoke);
                var arguments = callExpression.Arguments.Select(arg => AnalyzeExpression(context, arg)).ToArray();

                // Find the right overload
                if (method.ResolvedType != null)
                {
                    // Try this one, if failed -> fail!
                }
                else
                {
                    // Try multiple to get the right resolve. If fail -> fail!
                }
            }
            else if (expression is BinaryOperatorExpression binaryOperatorExpression)
            {
                // TODO operator "overloading"
                // function +(a: T1, b: T2): T3 
                // register these functions in the standard library and somehow inline them?

                var left = AnalyzeExpression(context, binaryOperatorExpression.LeftHandSide);
                var right = AnalyzeExpression(context, binaryOperatorExpression.RightHandSide);

                // Find overloads :) this should be pretty hard but for now its _fine_
            }
            else if (expression is StringExpression stringExpression)
            {
                // TODO better builtin resolving
                var type = context.ResolveStructsByName("string").First();

                return new ExpressionSymbol(new StructTypeReference(type));
            }
            else if (expression is IntegerExpression integerExpression)
            {
                // TODO better builtin resolving.
                var type = context.ResolveStructsByName("int").First();

                return new ExpressionSymbol(new StructTypeReference(type));
            }
            else if (expression is IdentifierExpression identifierExpression)
            {
                // TODO the symbol information with candidate stuff should also include
                // more information like: which symbol exactly, where is it defined, etc.
                var variables = context.ResolveNameAndTypes(identifierExpression.Identifier);

                var functions = context.ResolveFunctionsByName(identifierExpression.Identifier);
                var structs = context.ResolveStructsByName(identifierExpression.Identifier);

                var options = new List<TypeReference>();

                options.AddRange(variables.Select(v => v.Type));
                options.AddRange(functions.Select(f => new FunctionTypeReference(f)));
                options.AddRange(structs.Select(s => new StructTypeReference(s)));

                return new ExpressionSymbol(options);
            }

            throw new ApplicationException("Unkonwn expression type!");
        }

        public bool IsFunctionMatch(TypeReference type, IEnumerable<TypeReference> arguments)
        {
            if (type is FunctionTypeReference functionType)
            {
                return IsFunctionMatch(functionType.Definition.Signature, arguments);
            }
            else if (type is LambdaTypeReference lambdaType)
            {
                return IsFunctionMatch(lambdaType.Signature, arguments);
            }
            else
            {
                // Type not a function type at all.
                return false;
            }
        }

        public bool IsFunctionMatch(FunctionSignature signature, IEnumerable<TypeReference> arguments)
        {
            if (signature.Parameters.Count() != arguments.Count()) return false;

            return signature.Parameters.Zip(arguments).All(argumentAndParameter => argumentAndParameter.First == argumentAndParameter.Second);
        }

        public TypeReference ResolveType(IDefinitionContext context, TypeSyntax? type)
        {
            // TODO better handling of void, string, int, float etc
            if (type == null) return new VoidTypeReference();

            var typeDefinition = context.ResolveStructsByName(type.Name).First();

            return new StructTypeReference(typeDefinition);
        }
    }
}
