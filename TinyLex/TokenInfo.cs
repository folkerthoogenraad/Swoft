using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyLex
{
    public record TokenInfo(string Data, int OffsetInDocument, int Line, int Column);
}
