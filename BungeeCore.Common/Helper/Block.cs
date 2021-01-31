using System;
using System.Text;

namespace BungeeCore.Common.Helper
{
    public class Block : IDisposable
    {
        public byte[] buffer;
        public int step;
        public Block(byte[] buffer)
        {
            this.buffer = buffer;
            step = 0;
        }
        public Block(byte[] buffer, int step)
        {
            this.buffer = buffer;
            this.step = step;
        }
        public bool readBoolean()
        {
            if (step >= buffer.Length)
            {
                return false;
            }

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(buffer, step, 1);
            }
            bool temp = BitConverter.ToBoolean(buffer, step);
            step++;
            return temp;
        }
        public sbyte readSByte()
        {
            if (step >= buffer.Length)
            {
                return 0;
            }

            return (sbyte)buffer[step++];
        }
        public byte readByte()
        {
            if (step >= buffer.Length)
            {
                return 0;
            }

            return buffer[step++];
        }
        public short readShort()
        {
            if (step >= buffer.Length)
            {
                return 0;
            }

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(buffer, step, 2);
            }
            short temp = BitConverter.ToInt16(buffer, step);
            step += 2;
            return temp;
        }
        public ushort readUnsignedShort()
        {
            if (step >= buffer.Length)
            {
                return 0;
            }

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(buffer, step, 2);
            }
            ushort temp = BitConverter.ToUInt16(buffer, step);
            step += 2;
            return temp;
        }
        public int readInt()
        {
            if (step >= buffer.Length)
            {
                return 0;
            }

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(buffer, step, 4);
            }
            int temp = BitConverter.ToInt32(buffer, step);
            step += 4;
            return temp;
        }
        public long readLong()
        {
            if (step >= buffer.Length)
            {
                return 0;
            }

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(buffer, step, 8);
            }
            long temp = BitConverter.ToInt64(buffer, step);
            step += 8;
            return temp;
        }
        public float readFloat()
        {
            if (step >= buffer.Length)
            {
                return 0;
            }

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(buffer, step, 4);
            }
            float temp = BitConverter.ToSingle(buffer, step);
            step += 4;
            return temp;
        }
        public double readDouble()
        {
            if (step >= buffer.Length)
            {
                return 0;
            }

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(buffer, step, 8);
            }
            double temp = BitConverter.ToDouble(buffer, step);
            step += 8;
            return temp;
        }
        public string readString()
        {
            int jsonLength = readVarInt();
            string str = Encoding.UTF8.GetString(buffer, step, jsonLength);
            step += jsonLength;
            return str;
        }
        public string readString(int Length)
        {
            string str = Encoding.UTF8.GetString(buffer, step, Length);
            step += Length;
            return str;
        }
        public int readVarInt()
        {
            int numRead = 0;
            int result = 0;
            sbyte read;
            do
            {
                read = readSByte();
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
        public long readVarLong()
        {
            int numRead = 0;
            long result = 0;
            sbyte read;
            do
            {
                read = readSByte();
                long value = (read & 0b01111111);
                result |= (value << (7 * numRead));
                numRead++;
                if (numRead > 10)
                {
                    throw new Exception("VarLong is too big");
                }
            } while ((read & 0b10000000) != 0);

            return result;
        }
        public byte[] readBytes()
        {
            int length = readVarInt();
            return readPacket(length);
        }
        public byte[] readPacket()
        {
            int lenght = buffer.Length - step;
            byte[] vs = new byte[lenght];
            Array.Copy(buffer, step, vs, 0, lenght);
            return vs;
        }
        public byte[] readPacket(int Length)
        {
            byte[] data = new byte[Length];
            Array.Copy(buffer, step, data, 0, Length);
            step += Length;
            return data;
        }
        public void Dispose()
        {
            buffer = null;
        }
    }
}
