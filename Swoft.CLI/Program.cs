
using LLVMSharp;
using TinyLex;

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
    LLVMTypeRef main_function_type = LLVM.FunctionType(int_32_type, null, false);
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

    LLVM.DumpModule(module); // dump module to STDOUT
    //LLVMPrintModuleToFile(module, "hello.ll", nullptr);

    // clean memory
    LLVM.DisposeBuilder(builder);
    LLVM.DisposeModule(module);
    LLVM.ContextDispose(context);

}

void Test2()
{
    Lexer<Token> lexer = new Lexer<Token>();

    // Whitespace etc
    //lexer.SequenceOf(char.IsWhiteSpace).Creates(data => new Token(TokenType.Whitespace, data));
    //lexer.OpenClose("//", "\n").Creates(data => new Token(TokenType.Comment, data));
    //lexer.OpenClose("/*", "*/").Creates(data => new Token(TokenType.Comment, data));
    lexer.OpenClose("\"", "\"", "\\").Creates(data => new Token(TokenType.String, data));

    //lexer.SequenceOf(char.IsLetterOrDigit).StartsWith(char.IsLetter).Creates(data => new Token(TokenType.Identifier, data));

    //lexer.SequenceOf(char.IsDigit).Creates(data => new Token(TokenType.Integer, data));

    var result = lexer.Tokenize("\"this is a string with \\\"escaped\\\" characters.\"");
    //var result = lexer.Tokenize("this");

    if (!result.Succeeded)
    {
        foreach(var error in result.Errors)
        {
            Console.WriteLine(error.Message);
        }
    }

    foreach (var token in result.Tokens)
    {
        Console.WriteLine(token);
    }

    Console.WriteLine("And with filters");

    foreach (var token in result.Tokens.Where(x => x.Type != TokenType.Whitespace && x.Type != TokenType.Comment))
    {
        Console.WriteLine(token);
    }
}

Test2();

public enum TokenType
{
    Keyword,
    Operator,

    String,

    Integer,
    Float,

    Identifier,
    
    Whitespace,
    Comment,
}
public record Token(TokenType Type, string Data);