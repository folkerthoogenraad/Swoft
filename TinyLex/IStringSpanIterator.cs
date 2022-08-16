using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyLex
{
    public interface IStringSpanIterator
    {
        public int StartingOffset { get; }
        public int StartingLine { get; }
        public int StartingColumn { get; }

        public int Offset { get; }
        public char Current();
        public bool Next();
        public bool HasNext();
        public bool HasCurrent();
        public bool Next(int count);
        public IStringSpanIterator Fork();
    }
}
