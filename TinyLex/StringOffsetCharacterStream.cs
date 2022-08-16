using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyLex
{
    public class StringOffsetCharacterStream : IStringSpanIterator
    {
        public string InputData { get; set; }
        public int Offset { get; set; }

        public StringOffsetCharacterStream(string inputData, int offset)
        {
            InputData = inputData;
            Offset = offset;
        }

        public char Current()
        {
            if(Offset >= InputData.Length)
            {
                throw new IndexOutOfRangeException("Offset exceeds input length");
            }

            return InputData[Offset];
        }

        public bool Next()
        {
            Offset++;
            return HasCurrent();
        }

        public bool HasNext()
        {
            return Offset < InputData.Length - 1;
        }

        public bool HasCurrent()
        {
            return Offset < InputData.Length;
        }

        public IStringSpanIterator Fork()
        {
            return new StringOffsetCharacterStream(InputData, Offset);
        }

        public bool Next(int count)
        {
            Offset += count;
            return HasCurrent();
        }
    }
}
