using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swoft.IL
{
    public class ILDebug
    {
        public static string ILToString(byte[] bytes)
        {
            ILReader reader = new ILReader(bytes);

            StringBuilder builder = new StringBuilder();

            while (reader.CanRead())
            {
                var opcode = reader.ReadIL();

                builder.Append(opcode.ToString());
                builder.Append(" ");

                switch (opcode)
                {
                    case ILCode.PushConstString:
                        builder.Append("\"");
                        builder.Append(reader.ReadString());
                        builder.Append("\"");
                        break;
                    case ILCode.PushConstInt:
                        builder.Append(reader.ReadInt());
                        break;
                    case ILCode.PushVariable:
                    case ILCode.PushFunction:
                        builder.Append(reader.ReadString());
                        break;
                    case ILCode.StoreVariable:
                        builder.Append(reader.ReadString());
                        break;
                    case ILCode.JumpToScope:
                        builder.Append(reader.ReadString());
                        break;
                    case ILCode.Invoke:
                        // Int should really be "callsite descriptor"
                        // to make sure we can actually fetch the right function :)
                        builder.Append(reader.ReadInt());
                        break;
                    case ILCode.Return:
                    case ILCode.Pop:
                    case ILCode.Add:
                        break;
                    default:
                        throw new NotImplementedException("Not implemented (yet)");
                }

                builder.AppendLine();
            }

            return builder.ToString();
        }
    }
}
