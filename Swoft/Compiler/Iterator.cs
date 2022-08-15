using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swoft.Compiler
{
    internal class Iterator<T>
    {
        private T[] _array;
        private int _index = 0;

        public Iterator(IEnumerable<T> input)
        {
            _array = input.ToArray();
        }

        public bool HasCurrent => _index <= _array.Length - 1;

        public T Next()
        {
            _index++;

            return Current();
        }

        public T Current()
        {
            if (!HasCurrent) return default(T);

            return _array[_index];
        }
    }
}
