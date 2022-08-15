using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyLex
{
    public class StringOffsetCharacterStream : ICharacterStream
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

        public char Next()
        {
            Offset++;
            return Current();
        }

        public bool HasNext()
        {
            return Offset < InputData.Length - 1;
        }

        public ICharacterStream Fork()
        {
            return new StringOffsetCharacterStream(InputData, Offset);
        }

        public char Next(int count)
        {
            Offset += count;
            return Current();
        }
    }
}
