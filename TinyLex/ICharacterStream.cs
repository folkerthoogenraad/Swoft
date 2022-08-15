using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyLex
{
    public interface ICharacterStream
    {
        public int Offset { get; }
        public char Current();
        public void Next();
        public bool HasNext();
        public bool HasCurrent();
        public char Next(int count);
        public ICharacterStream Fork();
    }
}
