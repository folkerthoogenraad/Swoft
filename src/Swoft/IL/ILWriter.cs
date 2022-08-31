using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swoft.IL
{
    public class ILWriter : IDisposable
    {
        private MemoryStream _stream;
        private BinaryWriter _writer;

        public ILWriter()
        {
            _stream = new MemoryStream();
            _writer = new BinaryWriter(_stream);
        }

        public void WriteIL(ILCode il)
        {
            _writer.Write((byte)il);
        }

        public void WriteString(string value)
        {
            if(value.Length > ushort.MaxValue)
            {
                throw new Exception("string too long.");
            }

            _writer.Write((ushort)value.Length);
            _writer.Write(Encoding.UTF8.GetBytes(value));
        }

        public void WriteInt(int value)
        {
            _writer.Write(value);
        }

        public void Dispose()
        {
            _writer.Dispose();
            _stream.Dispose();
        }

        public byte[] GetBytes()
        {
            return _stream.ToArray();
        }
    }
}
