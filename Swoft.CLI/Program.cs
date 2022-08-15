
using LLVMSharp;

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

HelloWorld();