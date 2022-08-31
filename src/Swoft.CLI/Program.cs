
using CommandLine;
using Swoft.AST;
using Swoft.Debug;
using Swoft.Generator;
using Swoft.IL;
using Swoft.Lex;
using Swoft.Parsers;
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

// This whole thing is super hacked and whatnot,
// its just a proof of concept
namespace Swoft.CLI
{
    [Verb("run")]
    class CLIRunOptions
    {
        [Value(0)]
        public string? File { get; set; }

        [Option('f', "frame", Required = false, HelpText = "Sets the frame file location to write")]
        public string? FrameFile { get; set; }

        [Option('h', "no-halt", Required = false, HelpText = "Sets halt to true or false.")]
        public bool NoHalt { get; set; }
    }

    [Verb("resume")]
    class CLIResumeOptions
    {
        [Value(0)]
        public string? File { get; set; }

        [Option('h', "no-halt", Required = false, HelpText = "Sets halt to true or false.")]
        public bool NoHalt { get; set; }
    }

    class Program
    {
        public static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<CLIRunOptions, CLIResumeOptions>(args);
            
            result.WithParsed<CLIRunOptions>(options => {
                
            });

            result.WithParsed<CLIResumeOptions>(options => {
            
            });

            string file = "..\\..\\..\\..\\..\\test\\helloworld.apx";
            RunFile(file);
        }

        public ScopeSymbol ParseFile(string filename)
        {
            string input = File.ReadAllText(filename);

            var lexer = new SwoftLexer();
            var result = lexer.Tokenize(input);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine(error.Message);
                }
            }

            var parser = new SwoftParser(result.Tokens);
            var root = parser.ParseFile();

            var semanticGenerator = new SemanticGenerator();
            semanticGenerator.Analyze(root);

            var table = semanticGenerator.Table;

            var ilGenerator = new ILGenerator(table);
            ilGenerator.GenerateIL(root);
            ilGenerator.Commit();

            return table.GetRootScope();
        }

        public static void RunFile(string filename)
        {
            string input = File.ReadAllText(filename);

            var lexer = new SwoftLexer();
            var result = lexer.Tokenize(input);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine(error.Message);
                }
            }

            var parser = new SwoftParser(result.Tokens);
            var root = parser.ParseFile();

            var semanticGenerator = new SemanticGenerator();
            semanticGenerator.Analyze(root);

            var table = semanticGenerator.Table;

            var ilGenerator = new ILGenerator(table);
            ilGenerator.GenerateIL(root);
            ilGenerator.Commit();

            var rootScope = table.GetRootScope();

            if (rootScope.ExecuteableCode != null)
            {
                //Console.WriteLine(ILDebug.ILToString(rootScope.ExecuteableCode));
            }

            var runtime = new DebugRuntime(rootScope);

            runtime.NativeFunctions["print"] = (frame, arguments) => {
                Console.WriteLine(arguments[0]);
                return (null, true);
            };
            runtime.NativeFunctions["readLine"] = (frame, arguments) => {
                var line = Console.ReadLine();
                return (line, true);
            };
            runtime.NativeFunctions["halt"] = (frame, arguments) => {
                Console.WriteLine("HALTING.");
                return (null, false);
            };

            runtime.Run();
        }
    }
}