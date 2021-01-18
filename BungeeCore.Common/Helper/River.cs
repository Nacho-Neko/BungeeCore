using System;
using System.IO;
using System.Text;

namespace BungeeCore.Common.Helper
{
    public class River : IDisposable
    {
        public MemoryStream stream;
        public River()
        {
            stream = new MemoryStream();
        }
        public River(int Position)
        {
            stream = new MemoryStream();
            stream.Position = Position;
        }
        public byte[] GetBytes()
        {
            return stream.ToArray();
        }
        public void WriteInt(int value)
        {
            do
            {
                byte b = (byte)(value & 0x7F);
                value >>= 7;
                if (value != 0)
                    b = (byte)(b | 0x80);
                stream.WriteByte(b);
            } while (value != 0);
        }
        public void WriteUShort(ushort value)
        {
            stream.WriteByte((byte)(value >> 8 & 0xFF));
            stream.WriteByte((byte)(value & 0xFF));
        }
        public void WriteShort(short value)
        {
            stream.WriteByte((byte)(value >> 8 & 0xFF));
            stream.WriteByte((byte)(value & 0xFF));
        }
        public void WriteString(string data, bool longString)
        {
            byte[] arrayOfByte = Encoding.UTF8.GetBytes(data);
            int Length = arrayOfByte.Length;
            if (longString)
                WriteInt(Length);
            stream.Write(arrayOfByte, 0, arrayOfByte.Length);
        }
        public void WriteLong(long value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            stream.Write(bytes);
        }
        public void Dispose()
        {
            if (stream != null)
                stream.Dispose();
        }
    }
}
