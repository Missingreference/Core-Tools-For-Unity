using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Elanetic.Tools.Serialization
{
    public unsafe class LinearReader : DataReader
    {
        
        static LinearReader()
        {
            if(BitConverter.IsLittleEndian)
            {
                m_Endian2ByteIndex0Offset = 0;
                m_Endian2ByteIndex1Offset = 1;
                m_Endian4ByteIndex0Offset = 0;
                m_Endian4ByteIndex1Offset = 1;
                m_Endian4ByteIndex2Offset = 2;
                m_Endian4ByteIndex3Offset = 3;
                m_Endian8ByteIndex0Offset = 0;
                m_Endian8ByteIndex1Offset = 1;
                m_Endian8ByteIndex2Offset = 2;
                m_Endian8ByteIndex3Offset = 3;
                m_Endian8ByteIndex4Offset = 4;
                m_Endian8ByteIndex5Offset = 5;
                m_Endian8ByteIndex6Offset = 6;
                m_Endian8ByteIndex7Offset = 7;
            }
            else
            {
                m_Endian2ByteIndex0Offset = 1;
                m_Endian2ByteIndex1Offset = 0;
                m_Endian4ByteIndex0Offset = 3;
                m_Endian4ByteIndex1Offset = 2;
                m_Endian4ByteIndex2Offset = 1;
                m_Endian4ByteIndex3Offset = 0;
                m_Endian8ByteIndex0Offset = 7;
                m_Endian8ByteIndex1Offset = 6;
                m_Endian8ByteIndex2Offset = 5;
                m_Endian8ByteIndex3Offset = 4;
                m_Endian8ByteIndex4Offset = 3;
                m_Endian8ByteIndex5Offset = 2;
                m_Endian8ByteIndex6Offset = 1;
                m_Endian8ByteIndex7Offset = 0;
            }
        }
        
        static private int m_Endian2ByteIndex0Offset;
        static private int m_Endian2ByteIndex1Offset;
        static private int m_Endian4ByteIndex0Offset;
        static private int m_Endian4ByteIndex1Offset;
        static private int m_Endian4ByteIndex2Offset;
        static private int m_Endian4ByteIndex3Offset;
        static private int m_Endian8ByteIndex0Offset;
        static private int m_Endian8ByteIndex1Offset;
        static private int m_Endian8ByteIndex2Offset;
        static private int m_Endian8ByteIndex3Offset;
        static private int m_Endian8ByteIndex4Offset;
        static private int m_Endian8ByteIndex5Offset;
        static private int m_Endian8ByteIndex6Offset;
        static private int m_Endian8ByteIndex7Offset;
        
        public LinearReader(IntPtr target, int capacity) : this((byte*)target, capacity)
        {
        }

        public LinearReader(void* target, int capacity) : this((byte*)target, capacity)
        {
        }

        public LinearReader(byte* target, int capacity) : base(capacity)
        {
            m_StartTarget = target;
            m_CurrentTarget = m_StartTarget;
        }

        public override byte ReadByte()
        {
            CheckSafeRead(1);

            if(m_BitPosition > 0)
            {
                return ReadByteOffset();
            }
            else
            {
                return ReadByteDirect();
            }
        }

        public override bool ReadBool()
        {
            if(m_BitPosition == 0)
                CheckSafeRead(1);

            byte b = *m_CurrentTarget;

            bool value = (b & (0b10000000 >> m_BitPosition)) != 0;
            
            AdvanceBits(1);

            return value;
        }

        public override int ReadInt()
        {
            return ZigZagDecodeInt(ReadUInt());
        }

        public override uint ReadUInt()
        {
            int header = ReadByte();

            if(header < 0b11111100) return (uint)header;
            else
            {
                int byteCount = (header & 0b000011) + 1;
                CheckSafeRead(byteCount);
                uint value = (uint)ReadByte();
                for(int i = 1; i < byteCount; i++)
                {
                    value <<= 8;
                    value |= (uint)ReadByte();
                }

                return value;
            }
        }

        public override short ReadShort()
        {
            CheckSafeRead(2);
            short value;
            byte* valuePtr = (byte*)&value;

            if(m_BitPosition > 0)
            {
                *(valuePtr + m_Endian2ByteIndex1Offset) = ReadByteOffset();
                *(valuePtr + m_Endian2ByteIndex0Offset) = ReadByteOffset();
            }
            else
            {
                *(valuePtr + m_Endian2ByteIndex1Offset) = ReadByteDirect();
                *(valuePtr + m_Endian2ByteIndex0Offset) = ReadByteDirect();
            }
            return value;
        }

        public override ushort ReadUShort()
        {
            CheckSafeRead(2);
            ushort value;
            byte* valuePtr = (byte*)&value;

            if(m_BitPosition > 0)
            {
                *(valuePtr + m_Endian2ByteIndex1Offset) = ReadByteOffset();
                *(valuePtr + m_Endian2ByteIndex0Offset) = ReadByteOffset();
            }
            else
            {
                *(valuePtr + m_Endian2ByteIndex1Offset) = ReadByteDirect();
                *(valuePtr + m_Endian2ByteIndex0Offset) = ReadByteDirect();
            }
            return value;
        }

        public override char ReadChar()
        {
            CheckSafeRead(2);
            char value;
            byte* valuePtr = (byte*)&value;

            if(m_BitPosition > 0)
            {
                *(valuePtr + m_Endian2ByteIndex1Offset) = ReadByteOffset();
                *(valuePtr + m_Endian2ByteIndex0Offset) = ReadByteOffset();
            }
            else
            {
                *(valuePtr + m_Endian2ByteIndex1Offset) = ReadByteDirect();
                *(valuePtr + m_Endian2ByteIndex0Offset) = ReadByteDirect();
            }
            return value;
        }

        public override float ReadFloat()
        {
            CheckSafeRead(4);
            float value;
            byte* valuePtr = (byte*)&value;

            if(m_BitPosition > 0)
            {
                *(valuePtr + m_Endian4ByteIndex3Offset) = ReadByteOffset();
                *(valuePtr + m_Endian4ByteIndex2Offset) = ReadByteOffset();
                *(valuePtr + m_Endian4ByteIndex1Offset) = ReadByteOffset();
                *(valuePtr + m_Endian4ByteIndex0Offset) = ReadByteOffset();
            }
            else
            {
                *(valuePtr + m_Endian4ByteIndex3Offset) = ReadByteDirect();
                *(valuePtr + m_Endian4ByteIndex2Offset) = ReadByteDirect();
                *(valuePtr + m_Endian4ByteIndex1Offset) = ReadByteDirect();
                *(valuePtr + m_Endian4ByteIndex0Offset) = ReadByteDirect();
            }
            
            return value;
        }

        public override double ReadDouble()
        {
            CheckSafeRead(8);
            double value;
            byte* valuePtr = (byte*)&value;

            if(m_BitPosition > 0)
            {
                *(valuePtr + m_Endian8ByteIndex7Offset) = ReadByteOffset();
                *(valuePtr + m_Endian8ByteIndex6Offset) = ReadByteOffset();
                *(valuePtr + m_Endian8ByteIndex5Offset) = ReadByteOffset();
                *(valuePtr + m_Endian8ByteIndex4Offset) = ReadByteOffset();
                *(valuePtr + m_Endian8ByteIndex3Offset) = ReadByteOffset();
                *(valuePtr + m_Endian8ByteIndex2Offset) = ReadByteOffset();
                *(valuePtr + m_Endian8ByteIndex1Offset) = ReadByteOffset();
                *(valuePtr + m_Endian8ByteIndex0Offset) = ReadByteOffset();
            }
            else
            {
                *(valuePtr + m_Endian8ByteIndex7Offset) = ReadByteDirect();
                *(valuePtr + m_Endian8ByteIndex6Offset) = ReadByteDirect();
                *(valuePtr + m_Endian8ByteIndex5Offset) = ReadByteDirect();
                *(valuePtr + m_Endian8ByteIndex4Offset) = ReadByteDirect();
                *(valuePtr + m_Endian8ByteIndex3Offset) = ReadByteDirect();
                *(valuePtr + m_Endian8ByteIndex2Offset) = ReadByteDirect();
                *(valuePtr + m_Endian8ByteIndex1Offset) = ReadByteDirect();
                *(valuePtr + m_Endian8ByteIndex0Offset) = ReadByteDirect();
            }
            
            return value;
        }

        public override long ReadLong()
        {
            return ZigZagDecodeLong(ReadULong());
        }

        public override ulong ReadULong()
        {
            int header = ReadByte();

            if(header < 0b11111000) return (ulong)header;
            else
            {
                int byteCount = (header & 0b0000111) + 1;
                CheckSafeRead(byteCount);
                ulong value = (ulong)ReadByte();
                for(int i = 1; i < byteCount; i++)
                {
                    value <<= 8;
                    value |= (ulong)ReadByte();
                }

                return value;
            }
        }

        public override string ReadString()
        {
            int stringLength = ReadInt();
            if(m_BitPosition > 0)
            {
                AdvanceBits(8 - m_BitPosition);
            }

            int sizeInBytes = stringLength * 2;
            CheckSafeRead(sizeInBytes);
            
            if(BitConverter.IsLittleEndian)
            {
                string value = new string((char*)m_CurrentTarget, 0, stringLength);
                AdvanceBytes(sizeInBytes);
                return value;
            }
            else
            {
                char[] flippedData = new char[stringLength];

                fixed(char* charPtr = &flippedData[0])
                {
                    byte* target = (byte*)charPtr;
                    for(int i = 0; i < stringLength; i++)
                    {
                        *target = *(m_CurrentTarget + 1);
                        target++;
                        *target = *m_CurrentTarget;
                        target++;
                        
                        m_CurrentTarget += 2;
                    }
                }

                m_Position += sizeInBytes;
                
                return new string(flippedData);
            }
            
        }

        public override void ReadByteArray(byte[] destinationArray)
        {
#if DEBUG
            if(destinationArray == null)
                throw new ArgumentNullException(nameof(destinationArray), "Inputted byte array is null.");
#endif
            int dataLength = ReadInt();
            int startPosition = m_Position;
            int amountToRead = dataLength;
            if(amountToRead > destinationArray.Length)
            {
                amountToRead = destinationArray.Length;
            }

            CheckSafeRead(dataLength);
            
            if(dataLength <= 256) //Arbitrary number of bytes. Replace with whatever is more optimal and make the appropriate changes in the writer
            {
                if(m_BitPosition > 0)
                {
                    for(int i = 0; i < amountToRead; i++)
                    {
                        destinationArray[i] = ReadByteOffset();
                    }
                }
                else
                {
                    for(int i = 0; i < amountToRead; i++)
                    {
                        destinationArray[i] = ReadByteDirect();
                    }
                }
            }
            else
            {
                if(m_BitPosition > 0)
                {
                    AdvanceBits(8 - m_BitPosition);
                }
                
                Marshal.Copy((IntPtr)m_CurrentTarget, destinationArray, 0, amountToRead);
            }
            
            m_Position = startPosition + dataLength;
            m_CurrentTarget = m_StartTarget + m_Position;
        }

        public override void ReadData(void* destination, int length)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte ReadByteOffset()
        {
            int startValue = (*m_CurrentTarget & m_CurrentInverse8BitMask) << m_BitPosition;
            AdvanceBytes(1);
            int endValue = (*m_CurrentTarget & m_Current8BitMask) >> m_InverseBitPosition;
            return (byte)(startValue | endValue);
        }

        private byte ReadByteDirect()
        {
            byte value = *m_CurrentTarget;
            AdvanceBytes(1);
            return value;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckSafeRead(int amountToRead)
        {
            int newPosition = m_Position + amountToRead;
            if(newPosition >= capacity)
            {
                if(newPosition == capacity && m_BitPosition == 0)
                    return;

                throw new IndexOutOfRangeException("Unable to read from DataReader. Nothing left to read at position: " + newPosition + ". Current Position: " + m_Position + " Current Bit Position: " + m_BitPosition + " Capacity: " + capacity);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AdvanceBits(int amountToRead)
        {
            m_BitPosition += amountToRead;
            if(m_BitPosition > 7)
            {
                m_Position += m_BitPosition / 8;
                m_BitPosition %= 8;
                m_CurrentTarget = m_StartTarget + m_Position;
            }
            
            UpdateMasks();
        }
        
        private void AdvanceBytes(int amountToRead)
        {
            m_Position += amountToRead;
            m_CurrentTarget = m_StartTarget + m_Position;
        }
    }
}