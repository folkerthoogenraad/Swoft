using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swoft.IL
{
    public enum ILCode : byte
    {
        PushConstString,            // PushConstString value: string
        PushConstInt,               // PushConstInt value: int

        PushVariable,               // PushVariable name: string
        PushFunction,               // PushFunction name: string

        StoreVariable,              // StoreVariable name: string

        JumpToScope,                // JumpToScope name: string <- Not great, but whatever :)
        Return,                     // Return

        Add,                        // PopScope

        Pop,                        // Pop <- pops a value from the stack.

        Invoke,                     // Invoke argCount: int
    }
}
