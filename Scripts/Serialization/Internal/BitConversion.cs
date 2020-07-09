using System.Runtime.InteropServices;

namespace Elanetic.Tools.Serialization.Internal
{
    static public class BitConversion
    {
        /// <summary>
        /// ZigZag encodes a signed integer and maps it to a unsigned integer
        /// </summary>
        /// <param name="value">The signed integer to encode</param>
        /// <returns>A ZigZag encoded version of the integer</returns>
        public static ulong ZigZagEncode(long value) => (ulong)((value >> 63) ^ (value << 1));

        /// <summary>
        /// Decides a ZigZag encoded integer back to a signed integer
        /// </summary>
        /// <param name="value">The unsigned integer</param>
        /// <returns>The signed version of the integer</returns>
        static public long ZigZagDecode(ulong value) => (((long)(value >> 1) & 0x7FFFFFFFFFFFFFFFL) ^ ((long)(value << 63) >> 63));
    }

    /// <summary>
    /// A struct with a explicit memory layout. The struct has 4 fields. float,uint,double and ulong.
    /// Every field has the same starting point in memory. If you insert a float value, it can be extracted as a uint.
    /// This is to allow for lockless and garbage free conversion from float to uint and double to ulong.
    /// This allows for VarInt encoding and other integer encodings.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct UIntFloat
    {
        [FieldOffset(0)]
        public float floatValue;

        [FieldOffset(0)]
        public uint uintValue;

        [FieldOffset(0)]
        public double doubleValue;

        [FieldOffset(0)]
        public ulong ulongValue;
    }
}