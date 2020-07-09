using System;
using System.IO;

using Elanetic.Tools.Serialization.Internal;

namespace Elanetic.Tools.Serialization
{
    public class BitWriter
    {
        private Stream stream;

        public BitWriter(Stream stream)
        {
            if(stream == null) throw new ArgumentNullException(nameof(stream));

            this.stream = stream;
        }

        public void WriteByte(byte value) => stream.WriteByte(value);
        public void WriteBool(bool value) => stream.WriteByte(value ? (byte)1 : (byte)0);

        public void WriteFloat(float value)
        {
            WriteUInt(new UIntFloat
            {
                floatValue = value
            }.uintValue);
        }

        public void WriteChar(char value) => WriteULong(value);

        public void WriteShort(short value) => WriteULong(BitConversion.ZigZagEncode(value));

        public void WriteUShort(ushort value) => WriteULong(value);

        public void WriteInt(int value) => WriteULong(BitConversion.ZigZagEncode(value));

        public void WriteUInt(uint value) => WriteULong(value);

        public void WriteDouble(double value)
        {
            WriteULong(new UIntFloat
            {
                doubleValue = value
            }.ulongValue);
        }

        public void WriteLong(long value) => WriteULong(BitConversion.ZigZagEncode(value));

        public void WriteULong(ulong value)
        {
            if (value <= 240) WriteByte((byte)value);
            else if (value <= 2287)
            {
                WriteByte((byte)(((value - 240) >> 8) + 241));
                WriteByte((byte)(value - 240));
            }
            else if (value <= 67823)
            {
                WriteByte((byte)249);
                WriteByte((byte)((value - 2288) >> 8));
                WriteByte((byte)(value - 2288));
            }
            else
            {
                ulong header = 255;
                ulong match = 0x00FF_FFFF_FFFF_FFFFUL;
                while (value <= match)
                {
                    --header;
                    match >>= 8;
                }
                WriteByte((byte)header);
                int max = (int)(header - 247);
                for (int i = 0; i < max; ++i) WriteByte((byte)(value >> (i << 3)));
            }
        }

        public void WriteString(string value)
        {
            int length = value.Length;
            WriteUInt((uint)length);
            for (int i = 0; i < length; i++)
            {
                WriteByte((byte)value[i]);
                WriteByte((byte)(value[i] >> 8));
                //WriteChar(value[i]);
            }
        }
    }
}