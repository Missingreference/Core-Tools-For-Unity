using System;
using System.IO;

namespace Elanetic.Tools.Serialization.Tests
{

    public static class BitReadWriteTest
    {
        static public bool RunTest(out string result)
        {
            if (!ByteTest())
            {
                result = "Byte Test Failed";
                return false;
            }

            if(!IntTest())
            {
                result = "Int Test Failed";
                return false;
            }

            if(!BoolTest())
            {
                result = "Bool Test Failed";
                return false;
            }

            if(!StringTest())
            {
                result = "String Test Failed";
                return false;
            }

            result = "Test Completed Successfully";
            return true;
        }

        #region Test Cases

        static private bool ByteTest()
        {
            TestStream stream = new TestStream();
            BitWriter bitWriter = new BitWriter(stream);
            BitReader bitReader = new BitReader(stream);

            //Write
            bitWriter.WriteByte(byte.MinValue);
            bitWriter.WriteByte(1);
            bitWriter.WriteByte(5);
            bitWriter.WriteByte(128);
            bitWriter.WriteByte(199);
            bitWriter.WriteByte(byte.MaxValue);
            byte[] expectedBytes = new byte[1000];
            Random random = new Random();
            for (int i = 0; i < expectedBytes.Length; i++)
            {
                expectedBytes[i] = (byte)random.Next(byte.MinValue, byte.MaxValue);
                bitWriter.WriteByte(expectedBytes[i]);
            }

            stream.Position = 0;

            //Read
            if (bitReader.ReadByte() != byte.MinValue) return false;
            if (bitReader.ReadByte() != 1) return false;
            if (bitReader.ReadByte() != 5) return false;
            if (bitReader.ReadByte() != 128) return false;
            if (bitReader.ReadByte() != 199) return false;
            if (bitReader.ReadByte() != byte.MaxValue) return false;
            for (int i = 0; i < expectedBytes.Length; i++)
            {
                if (bitReader.ReadByte() != expectedBytes[i]) return false;
            }

            return true;
        }

        static private bool IntTest()
        {
            TestStream stream = new TestStream();
            BitWriter bitWriter = new BitWriter(stream);
            BitReader bitReader = new BitReader(stream);

            //Write
            bitWriter.WriteInt(0);
            bitWriter.WriteInt(1);
            bitWriter.WriteInt(5);
            bitWriter.WriteInt(128);
            bitWriter.WriteInt(10000);
            bitWriter.WriteInt(-1);
            bitWriter.WriteInt(-10000);
            bitWriter.WriteInt(int.MinValue);
            bitWriter.WriteInt(int.MaxValue);
            int[] expectedInts = new int[1000];
            Random random = new Random();
            for (int i = 0; i < expectedInts.Length; i++)
            {
                expectedInts[i] = random.Next(int.MinValue, int.MaxValue);
                bitWriter.WriteInt(expectedInts[i]);
            }

            stream.Position = 0;

            //Read
            if (bitReader.ReadInt() != 0) return false;
            if (bitReader.ReadInt() != 1) return false;
            if (bitReader.ReadInt() != 5) return false;
            if (bitReader.ReadInt() != 128) return false;
            if (bitReader.ReadInt() != 10000) return false;
            if (bitReader.ReadInt() != -1) return false;
            if (bitReader.ReadInt() != -10000) return false;
            if (bitReader.ReadInt() != int.MinValue) return false;
            if (bitReader.ReadInt() != int.MaxValue) return false;
            for (int i = 0; i < expectedInts.Length; i++)
            {
                if (bitReader.ReadInt() != expectedInts[i]) return false;
            }

            return true;
        }

        static private bool BoolTest()
        {
            TestStream stream = new TestStream();
            BitWriter bitWriter = new BitWriter(stream);
            BitReader bitReader = new BitReader(stream);

            //Write
            bitWriter.WriteBool(true);
            bitWriter.WriteBool(false);
            bitWriter.WriteBool(false);
            bitWriter.WriteBool(true);
            bitWriter.WriteBool(true);

            bool[] expectedBools = new bool[1000];
            Random random = new Random();
            for (int i = 0; i < expectedBools.Length; i++)
            {
                expectedBools[i] = random.Next(0, 1) == 0;
                bitWriter.WriteBool(expectedBools[i]);
            }

            stream.Position = 0;

            //Read
            if (!bitReader.ReadBool()) return false;
            if (bitReader.ReadBool()) return false;
            if (bitReader.ReadBool()) return false;
            if (!bitReader.ReadBool()) return false;
            if (!bitReader.ReadBool()) return false;

            for (int i = 0; i < expectedBools.Length; i++)
            {
                if(bitReader.ReadBool() != expectedBools[i]) return false;
            }

            return true;
        }

        static private bool StringTest()
        {
            TestStream stream = new TestStream();
            BitWriter bitWriter = new BitWriter(stream);
            BitReader bitReader = new BitReader(stream);

            //Write
            bitWriter.WriteString("My Test String");
            bitWriter.WriteString("@_'!1231 test");
            bitWriter.WriteString(string.Empty);
            bitWriter.WriteString(" ");
            bitWriter.WriteString("F");

            stream.Position = 0;

            //Write
            if(bitReader.ReadString() != "My Test String") return false;
            if(bitReader.ReadString() != "@_'!1231 test") return false;
            if(bitReader.ReadString() != string.Empty) return false;
            if(bitReader.ReadString() != " ") return false;
            if(bitReader.ReadString() != "F") return false;

            return true;
        }

        #endregion Test Cases

        private class TestStream : Stream
        {
            public override long Position { get; set; }
            public override long Length { get; }

            public override bool CanWrite => true;
            public override bool CanRead => true;
            public override bool CanSeek => true;

            
            private byte[] m_Target;

            public TestStream()
            {
                m_Target = new byte[16];
            }

            public override void Flush()
            {
                throw new System.NotImplementedException();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new System.NotImplementedException();
            }

            public override void SetLength(long value)
            {
                throw new System.NotImplementedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                if (Position + count >= m_Target.Length) Grow(count);
                Buffer.BlockCopy(buffer, offset, m_Target, (int)Position, count);
                Position += count;
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                int tLen = Math.Min(count, (int)(m_Target.LongLength - Position)); //- ((BitPosition & 7) == 0 ? 0 : 1));
                for (int i = 0; i < tLen; ++i) buffer[offset + i] = m_Target[Position++];
                return tLen;
            }


            private void Grow(long newContent)
            {
                long value = newContent + m_Target.LongLength;
                long newCapacity = value;

                if (newCapacity < 256)
                    newCapacity = 256;
                // We are ok with this overflowing since the next statement will deal
                // with the cases where _capacity*2 overflows.
                if (newCapacity < m_Target.LongLength * 2)
                    newCapacity = m_Target.LongLength * 2;
                // We want to expand the array up to Array.MaxArrayLengthOneDimensional
                // And we want to give the user the value that they asked for
                if ((uint)(m_Target.LongLength * 2) > int.MaxValue)
                    newCapacity = value > int.MaxValue ? value : int.MaxValue;

                SetCapacity(newCapacity);
            }


            private void SetCapacity(long value)
            {
                byte[] newTarg = new byte[value];
                long len = Math.Min(value, m_Target.LongLength);
                Buffer.BlockCopy(m_Target, 0, newTarg, 0, (int)len);
                m_Target = newTarg;
            }
        }
    }
}
