using System;

namespace BungeeCore.Common.Extensions
{
    public readonly struct VarInt
    {
        public VarInt(int m_value)
        {
            value = m_value;
        }
        public static bool operator !=(VarInt left, int right)
        {
            return !right.Equals(left.value);
        }
        public static bool operator ==(VarInt left, int right)
        {
            return right.Equals(left.value);
        }
        public static int operator +(int left, VarInt right)
        {
            return left + right.value;
        }
        public static int operator -(int left, VarInt right)
        {
            return left - right.value;
        }
        public static implicit operator VarInt(int v)
        {
            return new VarInt(v);
        }
        public static implicit operator Int32(VarInt v)
        {
            return v.value;
        }
        private readonly int value;
        public const int MaxValue = 2147483647;
        public const int MinValue = -2147483648;
    }
}
