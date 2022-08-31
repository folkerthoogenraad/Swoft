using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swoft.IL
{
    public class ILReader
    {
        private MemoryStream _stream;
        private BinaryReader _reader;

        public ILReader(byte[] bytes, long offset = 0)
        {
            _stream = new MemoryStream(bytes);
            _stream.Position = offset;
            _reader = new BinaryReader(_stream);
        }

        public long Offset => _stream.Position;

        public bool CanRead()
        {
            return _stream.Position < _stream.Length;
        }

        public ILCode ReadIL()
        {
            return (ILCode) _reader.ReadByte();
        }

        public string ReadString()
        {
            var length = _reader.ReadUInt16();

            var bytes = _reader.ReadBytes(length);

            return Encoding.UTF8.GetString(bytes);
        }

        public int ReadInt()
        {
            return _reader.ReadInt32();
        }

        public void Dispose()
        {
            _reader.Dispose();
            _stream.Dispose();
        }
    }
}
