using Swoft.AST;
using Swoft.IL;
using Swoft.Debug;
using Swoft.Semantic;

namespace Swoft.Debug
{
    public class StackFrame
    {
        public StackFrame? Parent;
        public Dictionary<string, object?> Variables = new Dictionary<string, object?>();
        public Stack<object?> Stack = new Stack<object?>();
        public long Offset = 0;

        // Honestly scope identifiers could also be programmatically assigned to the symbol table somewhere
        // That might be easier than whatever this is currently, but whatever.
        // Then we can keep an hashmap in the symboltable as well, making lookups easier.
        // Its basically just a list of executable code in a table somewhere :)
        public string SymbolScopePath;

        public StackFrame(StackFrame? parent = null)
        {
            Parent = parent;
            SymbolScopePath = "";
        }
    }

    public class DebugRuntime
    {
        public ScopeSymbol Root { get; set; }
        public Dictionary<string, Func<StackFrame, object?[], (object? Result, bool ShouldContinue)>> NativeFunctions { get; set; }

        public DebugRuntime(ScopeSymbol root)
        {
            Root = root;
            NativeFunctions = new Dictionary<string, Func<StackFrame, object?[], (object? Result, bool ShouldContinue)>>();

            
        }

        public void Run()
        {
            StackFrame frame = new StackFrame();

            Run(frame, Root, new ILReader(Root.ExecuteableCode!));
        }

        public void Resume(StackFrame frame)
        {
            var scope = ScopePath.Resolve(Root, frame.SymbolScopePath);
            var reader = new ILReader(scope.ExecuteableCode!, frame.Offset);

            Run(frame, scope, reader);
        }

        public void Run(StackFrame frame, ScopeSymbol scope, ILReader reader)
        {
            // TODO this doesn't need to be set here, but whatever.
            frame.SymbolScopePath = ScopePath.Create(scope);

            while (reader.CanRead())
            {
                var opcode = reader.ReadIL();

                switch (opcode)
                {
                    case ILCode.PushConstString:
                        frame.Stack.Push(reader.ReadString());
                        break;
                    case ILCode.PushConstInt:
                        frame.Stack.Push(reader.ReadInt());
                        break;
                    case ILCode.PushVariable:
                        frame.Stack.Push(frame.Variables[reader.ReadString()]);
                        break;
                    case ILCode.PushFunction:
                        string name = reader.ReadString();
                        var func = scope.GetNamedType(name);

                        // We might want to change this function call stuff to something else
                        // Then we can get rid of the SymbolScope stuff
                        // We can just push a description of the function instead.
                        // Actual invocation resolution will be done later anyway, in the invoke 
                        // callsite descriptor.
                        frame.Stack.Push(func);
                        break;
                    case ILCode.StoreVariable:
                        var v = frame.Stack.Pop();
                        frame.Variables[reader.ReadString()] = v;
                        break;
                    case ILCode.JumpToScope:
                        {
                            var scopePath = reader.ReadString();

                            frame.Offset = reader.Offset;

                            scope = ScopePath.Resolve(Root, scopePath);
                            reader = new ILReader(scope.ExecuteableCode!);

                            frame = new StackFrame(frame);
                            frame.SymbolScopePath = ScopePath.Create(scope);
                        }
                        break;
                    case ILCode.Invoke:
                        int argumentCount = reader.ReadInt();

                        var function = frame.Stack.Pop() as FunctionSymbol;
                        var arguments = new List<object?>();

                        System.Diagnostics.Debug.Assert(function != null);

                        for (int i = 0; i < argumentCount; i++)
                        {
                            arguments.Add(frame.Stack.Pop());
                        }

                        // Invoke native
                        if (function.IsExtern)
                        {
                            // for invoking native functions we want to already offset the pointer
                            // for reasons :)
                            frame.Offset = reader.Offset;
                            var nativeResult = NativeFunctions[function.Name](frame, arguments.ToArray());

                            if (!nativeResult.ShouldContinue)
                            {
                                return;
                            }
                            else
                            {
                                frame.Stack.Push(nativeResult.Result);
                            }
                        }
                        
                        // Invoke non native
                        else
                        {
                            // TODO jumping to other function
                            frame.Stack.Push(null);
                        }

                        break;
                    case ILCode.Return:
                        {
                            var returnValue = frame.Stack.Pop();

                            frame = frame.Parent!;
                            scope = ScopePath.Resolve(Root, frame.SymbolScopePath);
                            reader = new ILReader(scope.ExecuteableCode!, frame.Offset);

                            frame.Stack.Push(returnValue);
                        }
                        break;
                    case ILCode.Pop:
                        frame.Stack.Pop();
                        break;
                    case ILCode.Add:
                    default:
                        throw new NotImplementedException("Not implemented (yet)");
                }

                // Sync the frame offset :)
                frame.Offset = reader.Offset;
            }
        }
    }
}