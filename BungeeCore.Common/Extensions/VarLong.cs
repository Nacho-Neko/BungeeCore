using System;

namespace BungeeCore.Common.Extensions
{
    public readonly struct VarLong
    {
        public const long MaxValue = 9223372036854775807;
        public const long MinValue = -9223372036854775808;
        public readonly long value;
        public VarLong(long m_value)
        {
            this.value = m_value;
        }
        public static bool operator !=(VarLong left, long right)
        {
            return !right.Equals(left.value);
        }
        public static bool operator ==(VarLong left, long right)
        {
            return right.Equals(left.value);
        }
        public static long operator +(long left, VarLong right)
        {
            return left + right.value;
        }
        public static long operator -(long left, VarLong right)
        {
            return left - right.value;
        }

        public static implicit operator VarLong(long v)
        {
            return new VarLong(v);
        }

        public static implicit operator Int64(VarLong v)
        {
            return v.value;
        }
    }
}
