
using LLVMSharp;
using System.Diagnostics;
using System.Text;
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

string? LexFloat(IStringSpanIterator stream)
{
    if (!char.IsDigit(stream.Current()))
    {
        return null;
    }

    StringBuilder builder = new StringBuilder();

    builder.Append(stream.Current());

    while (stream.Next() && char.IsDigit(stream.Current()))
    {
        builder.Append(stream.Current());
    }

    if(!stream.HasCurrent() || stream.Current() != '.')
    {
        return builder.ToString();
    }

    Debug.Assert(stream.Current() == '.');
    builder.Append('.');

    while (stream.Next() && char.IsDigit(stream.Current()))
    {
        builder.Append(stream.Current());
    }

    return builder.ToString();
    
}

void Test2()
{
    Lexer<Token> lexer = new Lexer<Token>();

    // Error setup
    lexer.SetErrorProcessor(data => new Token(TokenType.Unknown, data));

    // Whitespace etc
    lexer.SequenceOf(char.IsWhiteSpace).Creates(data => new Token(TokenType.Whitespace, data));

    // Keywords
    lexer.Literal("function").Creates(data => new Token(TokenType.Keyword, data));
    lexer.Literal("class").Creates(data => new Token(TokenType.Keyword, data));
    lexer.Literal("struct").Creates(data => new Token(TokenType.Keyword, data));
    
    // Identfiers
    lexer.SequenceOf(char.IsLetterOrDigit)
        .StartsWith(char.IsLetter)
        .Creates(data => new Token(TokenType.Identifier, data));

    // Operators, etc
    lexer.Literal("(").Creates(d => new Token(TokenType.BracketOpen, d));
    lexer.Literal(")").Creates(d => new Token(TokenType.BracketClose, d));
    lexer.Literal("[").Creates(d => new Token(TokenType.ArrayOpen, d));
    lexer.Literal("]").Creates(d => new Token(TokenType.ArrayClose, d));
    lexer.Literal("{").Creates(d => new Token(TokenType.CurlyOpen, d));
    lexer.Literal("}").Creates(d => new Token(TokenType.CurlyClose, d));

    lexer.Literal("+").Creates(d => new Token(TokenType.BinaryOperator, d));
    lexer.Literal("-").Creates(d => new Token(TokenType.BinaryOperator, d));
    lexer.Literal("*").Creates(d => new Token(TokenType.BinaryOperator, d));
    lexer.Literal("/").Creates(d => new Token(TokenType.BinaryOperator, d));
    lexer.Literal("%").Creates(d => new Token(TokenType.BinaryOperator, d));

    lexer.Literal("=").Creates(d => new Token(TokenType.BinaryOperator, d));
    lexer.Literal("+=").Creates(d => new Token(TokenType.BinaryOperator, d));
    lexer.Literal("-=").Creates(d => new Token(TokenType.BinaryOperator, d));
    lexer.Literal("*=").Creates(d => new Token(TokenType.BinaryOperator, d));
    lexer.Literal("/=").Creates(d => new Token(TokenType.BinaryOperator, d));
    lexer.Literal("%=").Creates(d => new Token(TokenType.BinaryOperator, d));

    lexer.Literal("==").Creates(d => new Token(TokenType.BinaryOperator, d));
    lexer.Literal(">").Creates(d => new Token(TokenType.BinaryOperator, d));
    lexer.Literal("<").Creates(d => new Token(TokenType.BinaryOperator, d));
    lexer.Literal(">=").Creates(d => new Token(TokenType.BinaryOperator, d));
    lexer.Literal("<=").Creates(d => new Token(TokenType.BinaryOperator, d));

    lexer.Literal("||").Creates(d => new Token(TokenType.BinaryOperator, d));
    lexer.Literal("&&").Creates(d => new Token(TokenType.BinaryOperator, d));
    lexer.Literal("!").Creates(d => new Token(TokenType.BinaryOperator, d));
    
    lexer.Literal("&").Creates(d => new Token(TokenType.BinaryOperator, d));
    lexer.Literal("|").Creates(d => new Token(TokenType.BinaryOperator, d));
    lexer.Literal("^").Creates(d => new Token(TokenType.BinaryOperator, d));
    lexer.Literal(">>").Creates(d => new Token(TokenType.BinaryOperator, d));
    lexer.Literal("<<").Creates(d => new Token(TokenType.BinaryOperator, d));

    lexer.Literal("++").Creates(d => new Token(TokenType.UnaryOperator, d));
    lexer.Literal("--").Creates(d => new Token(TokenType.UnaryOperator, d));

    lexer.Literal("=>").Creates(d => new Token(TokenType.Arrow, d));
    lexer.Literal(":").Creates(d => new Token(TokenType.Colon, d));
    lexer.Literal(".").Creates(d => new Token(TokenType.Lookup, d));
    lexer.Literal(",").Creates(d => new Token(TokenType.Seperator, d));
    lexer.Literal(";").Creates(d => new Token(TokenType.LineEnd, d));

    // Value literals
    lexer.OpenClose(open: "\"", close: "\"", escape: "\\").Creates(data => new Token(TokenType.String, data));
    lexer.OpenClose(open: "'", close: "'", escape: "\\").Creates(data => new Token(TokenType.String, data));
    lexer.SequenceOf(char.IsDigit).Creates(data => new Token(TokenType.Integer, data));
    lexer.Lambda(LexFloat).Creates(data => new Token(TokenType.Float, data));

    // Comments
    lexer.OpenClose("//", "\n").Creates(data => new Token(TokenType.Comment, data));
    lexer.OpenClose("/*", "*/").Creates(data => new Token(TokenType.Comment, data));


    var input = "\"this is a string with \\\"escaped\\\" characters.\" but this is 4 3.4 32.4352 35653.3423 ++ - + 8* ^ && => .,";
    var result = lexer.Tokenize(input);
    var output = result.Tokens.Select(x => x.Data).Aggregate((prev, current) => prev + current);


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
    Unknown,

    Keyword,

    UnaryOperator,
    BinaryOperator,

    Colon,
    Seperator,
    LineEnd,
    Lookup,

    Arrow,

    BracketOpen,
    BracketClose,
    CurlyOpen,
    CurlyClose,
    ArrayOpen,
    ArrayClose,

    String,

    Integer,
    Float,

    Identifier,
    
    Whitespace,
    Comment,
}
public record Token(TokenType Type, string Data);