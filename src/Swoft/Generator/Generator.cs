using Mono.Cecil;
using Mono.Cecil.Cil;
using Swoft.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swoft.Generator
{
    public class Generator
    {
        public void Generate(StatementSyntax[] statements, string outputFile)
        {
            var swoftDLL = AssemblyDefinition.ReadAssembly("Swoft.BCL.dll");
            var swoftModule = swoftDLL.MainModule;

            
            // TODO this kind of importing should be done by annotations and shit
            // in extern function definitions :)
            // This could be rather easy actually
            var swoftSystem = swoftModule.GetType("Swoft.BCL.SwoftSystem");
            var printMethod = swoftSystem.Methods.Where(m => m.Name == "print").First();

            var myHelloWorldApp = AssemblyDefinition.CreateAssembly(
                new AssemblyNameDefinition("HelloWorld", new Version(1, 0, 0, 0)), "HelloWorld", ModuleKind.Console);

            var module = myHelloWorldApp.MainModule;

            var coreLibraryReference = new AssemblyNameReference("System.Runtime", new Version(6, 0, 0, 0))
            {
                PublicKeyToken = new byte[] { 0xb0, 0x3f, 0x5f, 0x7f, 0x11, 0xd5, 0x0a, 0x3a }
            };

            module.AssemblyReferences.Add(coreLibraryReference);

            var targetFrameworkConstructor = module.ImportReference(typeof(System.Runtime.Versioning.TargetFrameworkAttribute).GetConstructor(new Type[] { typeof(string) }));
            var customAttribute = new CustomAttribute(targetFrameworkConstructor);

            customAttribute.ConstructorArguments.Add(new CustomAttributeArgument(module.ImportReference(typeof(string)), ".NETCoreApp,Version=v6.0"));

            myHelloWorldApp.CustomAttributes.Add(customAttribute);

            // create the program type and add it to the module
            var programType = new TypeDefinition("HelloWorld", "Program",
                Mono.Cecil.TypeAttributes.Class | Mono.Cecil.TypeAttributes.Public);

            module.Types.Add(programType);

            // add an empty constructor
            var ctor = new MethodDefinition(".ctor", Mono.Cecil.MethodAttributes.Public | Mono.Cecil.MethodAttributes.HideBySig
                | Mono.Cecil.MethodAttributes.SpecialName | Mono.Cecil.MethodAttributes.RTSpecialName, module.TypeSystem.Void);

            // create the constructor's method body
            var il = ctor.Body.GetILProcessor();

            il.Append(il.Create(OpCodes.Ldarg_0));

            // call the base constructor
            il.Append(il.Create(OpCodes.Call, module.ImportReference(typeof(object).GetConstructor(Array.Empty<Type>()))));

            il.Append(il.Create(OpCodes.Nop));
            il.Append(il.Create(OpCodes.Ret));

            programType.Methods.Add(ctor);

            // define the 'Main' method and add it to 'Program'
            var mainMethod = new MethodDefinition("Main",
                Mono.Cecil.MethodAttributes.Public | Mono.Cecil.MethodAttributes.Static, module.TypeSystem.Void);

            programType.Methods.Add(mainMethod);

            // add the 'args' parameter
            var argsParameter = new ParameterDefinition("args",
                Mono.Cecil.ParameterAttributes.None, module.ImportReference(typeof(string[])));

            mainMethod.Parameters.Add(argsParameter);

            // create the method body
            il = mainMethod.Body.GetILProcessor();

            il.Append(il.Create(OpCodes.Nop));

            var writeLineCall = il.Create(OpCodes.Call, module.ImportReference(printMethod));

            // call the method
            il.Append(il.Create(OpCodes.Ldstr, "Hello World"));
            il.Append(writeLineCall);

            il.Append(il.Create(OpCodes.Call, module.ImportReference(typeof(System.Environment).GetMethod("get_Version", new Type[] { }))));
            il.Append(il.Create(OpCodes.Callvirt, module.ImportReference(typeof(object).GetMethod("ToString", new Type[] { }))));

            il.Append(writeLineCall);

            il.Append(il.Create(OpCodes.Nop));
            il.Append(il.Create(OpCodes.Ret));

            // set the entry point and save the module
            myHelloWorldApp.EntryPoint = mainMethod;
            myHelloWorldApp.Write("HelloWorld.dll");
        }

#if false
        public void GenerateExpression(ILGenerator generator, Expression expression)
        {
            GenerateExpressionInternal(generator, (dynamic)expression);
        }
        public void GenerateExpressionInternal(ILGenerator generator, Expression expression)
        {
            throw new NotImplementedException();
        }
        public void GenerateExpressionInternal(ILGenerator generator, CallExpression exp)
        {
            // TODO resolve method from parameters and stuff
            var writeLineMethod = typeof(System.Console).GetMethod(
                "WriteLine",
                BindingFlags.Public | BindingFlags.Static,
                new Type[] { typeof(string) });

            foreach (var argument in exp.Arguments)
            {
                GenerateExpression(generator, argument);
            }

            generator.Emit(OpCodes.Call, writeLineMethod!);
        }
        public void GenerateExpressionInternal(ILGenerator generator, StringExpression exp)
        {
            generator.Emit(OpCodes.Ldstr, exp.Value);
        }
#endif
    }
}
