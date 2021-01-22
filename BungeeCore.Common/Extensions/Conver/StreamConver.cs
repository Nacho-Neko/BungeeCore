using System;
using System.IO;
using System.Text;

namespace BungeeCore.Common.Extensions.Conver
{
    public static class StreamConver
    {
        public static int ReadVarInt(this Stream stream)
        {
            int numRead = 0;
            int result = 0;
            sbyte read;
            do
            {
                read = (sbyte)stream.ReadByte();
                int value = read & 0b01111111;
                result |= (value << (7 * numRead));
                numRead++;
                if (numRead > 5)
                {
                    throw new Exception("VarInt is too big");
                }
            } while ((read & 0b10000000) != 0);

            return result;
        }
        public static void WriteString(this Stream stream, string value, bool longString)
        {
            byte[] arrayOfByte = Encoding.UTF8.GetBytes(value);
            int Length = arrayOfByte.Length;
            if (longString)
                WriteInt(stream, Length);
            stream.Write(arrayOfByte, 0, arrayOfByte.Length);
        }
        public static void WriteInt(this Stream stream, int value)
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
        public static void WriteUShort(this Stream stream, ushort value)
        {
            stream.WriteByte((byte)(value >> 8 & 0xFF));
            stream.WriteByte((byte)(value & 0xFF));
        }
        public static void WriteLong(this Stream stream, long value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            stream.Write(bytes);
        }
    }
}
