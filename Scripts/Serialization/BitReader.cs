using System;
using System.IO;
using System.Text;

using Elanetic.Tools.Serialization.Internal;

namespace Elanetic.Tools.Serialization
{
    public class BitReader
    {
        private Stream stream;

        public BitReader(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            this.stream = stream;
        }

        public byte ReadByte() => (byte)stream.ReadByte();
        public bool ReadBool() => stream.ReadByte() != 0;

        public float ReadFloat()
        {
            return new UIntFloat
            {
                uintValue = ReadUInt()
            }.floatValue;
        }

        public char ReadChar() => (char)ReadShort();

        public short ReadShort() => (short)BitConversion.ZigZagDecode(ReadULong());

        public ushort ReadUShort() => (ushort)ReadULong();

        public int ReadInt() => (int)BitConversion.ZigZagDecode(ReadULong());

        public uint ReadUInt() => (uint)ReadULong();

        public double ReadDouble()
        {
            return new UIntFloat
            {
                ulongValue = ReadULong()
            }.doubleValue;
        }

        public long ReadLong() => BitConversion.ZigZagDecode(ReadULong());

        public ulong ReadULong()
        {
            ulong header = ReadByte();
            if (header <= 240) return header;
            if (header <= 248) return 240 + ((header - 241) << 8) + ReadByte();
            if (header == 249) return 2288UL + (ulong)(stream.ReadByte() << 8) + ReadByte();
            ulong res = ReadByte() | ((ulong)ReadByte() << 8) | ((ulong)stream.ReadByte() << 16);
            int cmp = 2;
            int hdr = (int)(header - 247);
            while (hdr > ++cmp) res |= (ulong)stream.ReadByte() << (cmp << 3);
            return res;
        }

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
    }
}