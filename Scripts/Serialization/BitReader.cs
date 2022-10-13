using System;
using System.IO;
using System.Text;

using Elanetic.Tools.Serialization.Internal;

namespace Elanetic.Tools.Serialization
{
    /// <summary>
    /// A simple binary reader. It is recommended to only read from a stream that was written to by the BitWriter otherwise data may be retrieved incorrectly.
    /// </summary>
    public class BitReader
    {
        private Stream m_Stream;

        public BitReader(Stream stream)
        {
#if DEBUG
            if (stream == null) 
                throw new ArgumentNullException(nameof(stream), "Inputted stream is null.");
#endif

            m_Stream = stream;
        }

        /// <summary>
        /// Read a byte from the stream.
        /// </summary>
        /// <returns>The byte retrieved from the stream.</returns>
        public byte ReadByte() => (byte)m_Stream.ReadByte();

        /// <summary>
        /// Read a bool from the stream.
        /// </summary>
        /// <returns>The bool retrieved from the stream.</returns>
        public bool ReadBool() => m_Stream.ReadByte() != 0;

        /// <summary>
        /// Read a float from the stream.
        /// </summary>
        /// <returns>The float retrieved from the stream.</returns>
        public float ReadFloat()
        {
            return new UIntFloat
            {
                uintValue = ReadUInt()
            }.floatValue;
        }

        /// <summary>
        /// Read a char from the stream.
        /// </summary>
        /// <returns>The char retrieved from the stream.</returns>
        public char ReadChar() => (char)ReadShort();

        /// <summary>
        /// Read a signed short integer from the stream.
        /// </summary>
        /// <returns>The short retrieved from the stream.</returns>
        public short ReadShort() => (short)BitConversion.ZigZagDecode(ReadULong());

        /// <summary>
        /// Read an unsigned short integer from the stream.
        /// </summary>
        /// <returns>The unsigned short retrieved from the stream.</returns>
        public ushort ReadUShort() => (ushort)ReadULong();

        /// <summary>
        /// Read a signed integer from the stream.
        /// </summary>
        /// <returns>The int retrieved from the stream.</returns>
        public int ReadInt() => (int)BitConversion.ZigZagDecode(ReadULong());

        /// <summary>
        /// Read an unsigned integer from the stream.
        /// </summary>
        /// <returns>The unsigned integer from the stream.</returns>
        public uint ReadUInt() => (uint)ReadULong();

        /// <summary>
        /// Read a double from the stream.
        /// </summary>
        /// <returns>The double retrieved from the stream.</returns>
        public double ReadDouble()
        {
            return new UIntFloat
            {
                ulongValue = ReadULong()
            }.doubleValue;
        }

        /// <summary>
        /// Read a long integer from the stream.
        /// </summary>
        /// <returns>The long retrieved from the stream.</returns>
        public long ReadLong() => BitConversion.ZigZagDecode(ReadULong());

        /// <summary>
        /// Read an unsigned long integer from the stream.
        /// </summary>
        /// <returns>The unsigned long retrieved from the stream.</returns>
        public ulong ReadULong()
        {
            ulong header = ReadByte();
            if (header <= 240) return header;
            if (header <= 248) return 240 + ((header - 241) << 8) + ReadByte();
            if (header == 249) return 2288UL + (ulong)(m_Stream.ReadByte() << 8) + ReadByte();
            ulong res = ReadByte() | ((ulong)ReadByte() << 8) | ((ulong)m_Stream.ReadByte() << 16);
            int cmp = 2;
            int hdr = (int)(header - 247);
            while (hdr > ++cmp) res |= (ulong)m_Stream.ReadByte() << (cmp << 3);
            return res;
        }

        /// <summary>
        /// Read a string from the stream. 2 bytes per character.
        /// </summary>
        /// <returns>The string retrieved from the stream.</returns>
        public string ReadString()
        {
            int expectedLength = (int)ReadUInt();
            StringBuilder stringBuilder = new StringBuilder(expectedLength);
            for (int i = 0; i < expectedLength; i++)
            {
                stringBuilder.Insert(i, (char)(ReadByte() | (ReadByte() << 8)));//ReadChar());
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Read a byte array from the stream.
        /// </summary>
        /// <returns>The byte array retrieved from the stream.</returns>
        public byte[] ReadByteArray()
        {
            ulong length = ReadULong();
            byte[] byteArray = new byte[length];
            for (ulong i = 0; i < length; i++) byteArray[i] = ReadByte();
            return byteArray;
        }

        /// <summary>
        /// Read a byte array from the stream. (Non-alloc). If the inputted array is smaller than the received data array, the read position will continue as if all the data received related to byte array has been read.
        /// </summary>
        /// <param name="byteArray">The array to be written to starting at index 0 and not overriding existing data that extends past the received amount of data.</param>
        /// <returns>The length of the array received from the stream.</returns>
        public long ReadByteArray(byte[] byteArray)
        {
#if DEBUG
            if(byteArray == null) 
                throw new ArgumentNullException(nameof(byteArray), "Inputted byte array is null.");
#endif

            ulong dataLength = ReadULong();
            ulong readLength = (ulong)byteArray.LongLength;
            if(dataLength < readLength) readLength = dataLength;
            for (ulong i = 0; i < readLength; i++) byteArray[i] = ReadByte();

            if(readLength < dataLength)
            {
                //"Read" the rest of the unread array
                for (ulong i = readLength; i < dataLength; i++) ReadByte();
            }

            return (long)dataLength;
        }
    }
}