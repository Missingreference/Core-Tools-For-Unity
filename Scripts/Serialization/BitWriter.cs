using System;
using System.IO;

using Elanetic.Tools.Serialization.Internal;

namespace Elanetic.Tools.Serialization
{
    /// <summary>
    /// A simple binary writer that will pack values as much as possible. It is recommended to only use BitReader to read data written by this class to reliably retrieve data.
    /// </summary>
    public class BitWriter
    {
        private Stream m_Stream;

        /// <summary>
        /// Create a BitWriter with a stream as it's target of writing.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        public BitWriter(Stream stream)
        {
#if DEBUG
            if(stream == null)
                throw new ArgumentNullException(nameof(stream), "Inputted stream is null.");
#endif

            m_Stream = stream;
        }

        /// <summary>
        /// Write a byte to the stream.
        /// </summary>
        /// <param name="value">The byte to write to the stream.</param>
        public void WriteByte(byte value) => m_Stream.WriteByte(value);

        /// <summary>
        /// Write a bool to the stream.
        /// </summary>
        /// <param name="value">The bool to write to the stream.</param>
        public void WriteBool(bool value) => m_Stream.WriteByte(value ? (byte)1 : (byte)0);

        /// <summary>
        /// Write a float to the stream.
        /// </summary>
        /// <param name="value">The float to write to the stream.</param>
        public void WriteFloat(float value)
        {
            WriteUInt(new UIntFloat
            {
                floatValue = value
            }.uintValue);
        }

        /// <summary>
        /// Write a char to the stream.
        /// </summary>
        /// <param name="value">The char to write to the stream.</param>
        public void WriteChar(char value) => WriteULong(value);

        /// <summary>
        /// Write a signed short integer to the stream.
        /// </summary>
        /// <param name="value">The short to write to the stream.</param>
        public void WriteShort(short value) => WriteULong(BitConversion.ZigZagEncode(value));

        /// <summary>
        /// Write an unsigned short integer to the stream.
        /// </summary>
        /// <param name="value">The unsigned short to write to the stream.</param>
        public void WriteUShort(ushort value) => WriteULong(value);

        /// <summary>
        /// Write a signed integer to the stream.
        /// </summary>
        /// <param name="value">The int to write to the stream.</param>
        public void WriteInt(int value) => WriteULong(BitConversion.ZigZagEncode(value));

        /// <summary>
        /// Write an unsigned integer to the stream.
        /// </summary>
        /// <param name="value">The unsigned int to write to the stream.</param>
        public void WriteUInt(uint value) => WriteULong(value);

        /// <summary>
        /// Write a double to the stream.
        /// </summary>
        /// <param name="value">The double to write to the stream.</param>
        public void WriteDouble(double value)
        {
            WriteULong(new UIntFloat
            {
                doubleValue = value
            }.ulongValue);
        }

        /// <summary>
        /// Write a signed long integer to the stream.
        /// </summary>
        /// <param name="value">The long to write to the stream.</param>
        public void WriteLong(long value) => WriteULong(BitConversion.ZigZagEncode(value));

        /// <summary>
        /// Write an unsigned long integer to the stream.
        /// </summary>
        /// <param name="value">The unsigned long to write to the stream.</param>
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

        /// <summary>
        /// Write a string to the stream. 2 bytes per character.
        /// </summary>
        /// <param name="value">The string to write to the stream.</param>
        public void WriteString(string value)
        {
            int length = value.Length;
            WriteUInt((uint)length);
            for (int i = 0; i < length; i++)
            {
                WriteByte((byte)value[i]);
                WriteByte((byte)(value[i] >> 8));
            }
        }

        /// <summary>
        /// Write a byte array to the stream.
        /// </summary>
        /// <param name="byteArray">The array to write to the stream.</param>
        public void WriteByteArray(byte[] byteArray) => WriteByteArray(byteArray, byteArray.LongLength);

        /// <summary>
        /// Write a byte array to the stream but limited to inputted count.
        /// </summary>
        /// <param name="byteArray">The array to write to the stream.</param>
        /// <param name="count">The amount of bytes to write starting from the zero index.</param>
        public void WriteByteArray(byte[] byteArray, long count)
        {
            ulong length = (ulong)byteArray.LongLength;
            if(length > (ulong)count) length = (ulong)count;
            WriteULong(length);
            for (ulong i = 0; i < length; ++i) WriteByte(byteArray[i]);
        }
    }
}