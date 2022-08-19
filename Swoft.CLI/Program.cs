
using Swoft.AST;
using Swoft.Generator;
using Swoft.Lex;
using Swoft.Semantic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using TinyLex;

#if false
void Test()
{
    LLVM.LinkInMCJIT();
    LLVM.InitializeNativeTarget();

    var module = LLVM.ModuleCreateWithName("FolkertModule");
    var func = LLVM.AddFunction(module, "main", LLVM.FunctionType(LLVM.DoubleType(), new[] { LLVM.DoubleType(), LLVM.DoubleType() }, IsVarArg: false));

    LLVM.SetFunctionCallConv(func, (uint)LLVMCallConv.LLVMCCallConv);

    var paramA = LLVM.GetParam(func, 0);
    var paramB = LLVM.GetParam(func, 1);

    LLVM.SetValueName(paramA, "first");
    LLVM.SetValueName(paramB, "second");

    var builder = LLVM.CreateBuilder();

    LLVMBasicBlockRef entry = LLVM.AppendBasicBlock(func, "entry");

    LLVM.PositionBuilderAtEnd(builder, entry);
    LLVM.BuildAdd(builder, paramA, paramB, "first + second");
}

void HelloWorld()
{
    // create context, module and builder
    LLVMContextRef context = LLVM.ContextCreate();
    LLVMModuleRef module = LLVM.ModuleCreateWithNameInContext("hello", context);
    LLVMBuilderRef builder = LLVM.CreateBuilderInContext(context);

    // types
    LLVMTypeRef int_8_type = LLVM.Int8TypeInContext(context);
    LLVMTypeRef int_8_type_ptr = LLVM.PointerType(int_8_type, 0);
    LLVMTypeRef int_32_type = LLVM.Int32TypeInContext(context);

    // puts function
    LLVMTypeRef[] puts_function_args_type = new[] {
        int_8_type_ptr
    };

    LLVMTypeRef puts_function_type = LLVM.FunctionType(int_32_type, puts_function_args_type, false);
    LLVMValueRef puts_function = LLVM.AddFunction(module, "puts", puts_function_type);
    // end

    // main function
    LLVMTypeRef main_function_type = LLVM.FunctionType(int_32_type, new LLVMTypeRef[0], false);
    LLVMValueRef main_function = LLVM.AddFunction(module, "main", main_function_type);

    LLVMBasicBlockRef entry = LLVM.AppendBasicBlockInContext(context, main_function, "entry");
    LLVM.PositionBuilderAtEnd(builder, entry);

    LLVMValueRef[] puts_function_args = new[]{
        LLVM.BuildPointerCast(builder, // cast [14 x i8] type to int8 pointer
                LLVM.BuildGlobalString(builder, "Hello, World!", "hello"), // build hello string constant
                int_8_type_ptr, "0")
    };

    LLVM.BuildCall(builder, puts_function, puts_function_args, "i");
    LLVM.BuildRet(builder, LLVM.ConstInt(int_32_type, 0, false));
    // end

    LLVM.VerifyModule(module, LLVMVerifierFailureAction.LLVMPrintMessageAction, out string message);

    LLVM.DumpModule(module); // dump module to STDOUT
                             //LLVMPrintModuleToFile(module, "hello.ll", nullptr);

    LLVMPassManagerRef pass = LLVMCreatePassManager();

    LLVM.AddTargetLibraryInfo(, pass);

    // Execution
    LLVMExecutionEngineRef engine;
    LLVMModuleProviderRef provider = LLVM.CreateModuleProviderForExistingModule(module);
    
    if (!LLVM.CreateJITCompilerForModule(out engine, module, 2, out string error))
    {
        Console.WriteLine("Failed to created jit compiler");
    }

    // LLVM.RunFunction(engine, main_function, new LLVMGenericValueRef[0]);

    // clean memory
    LLVM.DisposeBuilder(builder);
    LLVM.DisposeModule(module);
    LLVM.ContextDispose(context);
}
#endif


void Test2()
{
    SwoftLexer lexer = new SwoftLexer();

    var input = "var str = \"this is a string.\";\nvar test = 43 + 4 * (4 + 2);";
    var result = lexer.Tokenize(input);
    var output = result.Tokens.Select(x => x.Data).Aggregate((prev, current) => prev + current);

    Console.WriteLine(input);
    Console.WriteLine();

    if (!result.Succeeded)
    {
        foreach(var error in result.Errors)
        {
            Console.WriteLine(error.Message);
        }
    }

    foreach (var token in result.Tokens)
    {
        Console.WriteLine(token.Type + " " + token.Data);
    }

    //Console.WriteLine(JsonSerializer.Serialize(result.Tokens));

}


// Syntax tree
IdentifierExpression print = new IdentifierExpression("print");
StringExpression helloWorld = new StringExpression("hello world");

CallExpression call = new CallExpression(print, new Expression[] {helloWorld});
ExpressionStatement body = new ExpressionStatement(call);

FunctionStatement printFunction = new FunctionStatement("print", body: null, isExtern: true, isStatic: true);
FunctionStatement mainFunction = new FunctionStatement("__main", body, isExtern: true, isStatic: true);

// Backbone model stuff
AssemblyDefinition assembly = new AssemblyDefinition();

assembly.Structs.Add(new StructDefinition(assembly, "string"));
assembly.Structs.Add(new StructDefinition(assembly, "float"));
assembly.Structs.Add(new StructDefinition(assembly, "int"));

var printFunctionSignature = new FunctionSignature(new VoidTypeReference());
printFunctionSignature.Parameters.Add(new StructTypeReference(assembly.ResolveStructsByName("string").First()));

var mainFunctionSignature = new FunctionSignature(new VoidTypeReference());

FunctionDefinition printFunctionDefinition = new FunctionDefinition(assembly, printFunction.Name, printFunctionSignature);
FunctionDefinition mainFunctionDefinition = new FunctionDefinition(assembly, mainFunction.Name, mainFunctionSignature);

// Symbols
var symbolTable = new SymbolTable();

symbolTable.FunctionDefinitions.Add(printFunction, printFunctionDefinition);
symbolTable.FunctionDefinitions.Add(mainFunction, mainFunctionDefinition);

{
    var expressionSymbol = new ExpressionSymbol();
    var functions = assembly.ResolveFunctionsByName(print.Identifier);

    foreach(var function in functions)
    {
        var typeA = new FunctionTypeReference(function);
        var typeB = new LambdaTypeReference(function.Signature);

        expressionSymbol.CandidateTypes.Add(typeA);
        expressionSymbol.CandidateTypes.Add(typeB);
    }

    expressionSymbol.ResolvedType = expressionSymbol.CandidateTypes.FirstOrDefault();
}

{
    var expressionSymbol = new ExpressionSymbol();
    var typeA = new StructTypeReference(assembly.ResolveStructsByName("string").First());

    expressionSymbol.CandidateTypes.Add(typeA);

    expressionSymbol.ResolvedType = typeA;

    symbolTable.ExpressionSymbols.Add(helloWorld, expressionSymbol);
}

// Generator generator = new Generator();
// generator.Generate(new Statement[] { body }, "HelloWorld.dll");