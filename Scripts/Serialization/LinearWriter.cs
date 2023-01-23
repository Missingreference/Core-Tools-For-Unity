using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Elanetic.Tools.Serialization
{
    public unsafe class LinearWriter : DataWriter
    {
        static LinearWriter()
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
        
        private bool m_OwnsTheTarget;
        
        /// <summary>
        /// Be aware that this constructor means that you are responsible for deallocating every memory allocation.
        /// If the writer runs out of room for the target a new allocation will be made.
        /// Subscribe to onResize event to know when the target has been replaced with a new allocation.
        /// </summary>
        public LinearWriter(IntPtr target, int capacity) : this((byte*)target, capacity)
        {
            
        }

        /// <summary>
        /// Be aware that this constructor means that you are responsible for deallocating every memory allocation.
        /// If the writer runs out of room for the target a new allocation will be made.
        /// Subscribe to onResize event to know when the target has been replaced with a new allocation.
        /// </summary>
        public LinearWriter(void* target, int capacity) : this((byte*)target, capacity)
        {
        }
        
        /// <summary>
        /// Be aware that this constructor means that you are responsible for deallocating every memory allocation.
        /// If the writer runs out of room for the target a new allocation will be made.
        /// Subscribe to onResize event to know when the target has been replaced with a new allocation.
        /// </summary>
        public LinearWriter(byte* target, int capacity) : base(capacity)
        {
            m_StartTarget = target;
            m_CurrentTarget = m_StartTarget;
            
            m_OwnsTheTarget = false;
        }
        
        /// <summary>
        /// This constructor will handle allocating and deallocating the memory. Call dispose to ensure no memory leaks occur. 
        /// </summary>
        public LinearWriter(int capacity) : base(capacity)
        {
            m_StartTarget = (byte*)Marshal.AllocHGlobal(capacity);
            m_CurrentTarget = m_StartTarget;
            
            m_OwnsTheTarget = true;
        }

        public override void Dispose()
        {
            if(m_OwnsTheTarget)
            {
                //Ensure that if this gets disposed again that no crash occurs.
                m_OwnsTheTarget = false; 
                
                Marshal.FreeHGlobal((IntPtr)m_StartTarget);
            }
        }

        public override void WriteByte(byte value)
        {
            int newPosition = m_Position + 1;
            CheckSafeWrite(newPosition);
            m_Position = newPosition;
            
            if(m_BitPosition > 0)
            {
                WriteByteOffset(value);
            }
            else
            {
                WriteByteDirect(value);
            }
        }

        public override void WriteBool(bool value)
        {
            CheckSafeWrite(m_Position);
            
            unchecked
            {
                if(m_BitPosition == 0)
                {
                    if(value)
                    {
                        *m_CurrentTarget = (byte)(*m_CurrentTarget | 0b1000_0000);
                    }
                    else
                    {
                        *m_CurrentTarget = (byte)(*m_CurrentTarget & 0b0111_1111);
                    }

                    m_BitPosition++;
                }
                else if(m_BitPosition == 7)
                {
                    if(value)
                    {
                        *m_CurrentTarget = (byte)(*m_CurrentTarget | 0b0000_0001);
                    }
                    else
                    {
                        *m_CurrentTarget = (byte)(*m_CurrentTarget & 0b1111_1110);
                    }

                    m_Position++;
                    m_BitPosition = 0;
                    m_CurrentTarget = m_StartTarget + m_Position;
                }
                else
                {
                    if(value)
                    {
                        *m_CurrentTarget = (byte)(*m_CurrentTarget | (0b1000_0000 >> m_BitPosition));
                    }
                    else
                    {
                        *m_CurrentTarget = (byte)(*m_CurrentTarget & ~(0b1000_0000 >> m_BitPosition));
                    }
                    
                    m_BitPosition++;
                }
                
                UpdateMasks();
            }
        }

        public override void WriteInt(int value)
        {
            WriteUInt(ZigZagEncodeInt(value));
        }

        public override void WriteUInt(uint value)
        {
            byte* valPtr = (byte*)(uint*)&value;
            if(m_BitPosition > 0)
            {
                switch(value)
                {
                    case < 0b11111100:
                        m_Position++;
                        CheckSafeWrite(m_Position);
                        WriteByteOffset(*(valPtr + m_Endian4ByteIndex0Offset));
                        break;
                    case < (1U << 8):
                        int targetPosition = m_Position + 2;
                        CheckSafeWrite(targetPosition);
                        m_Position++;
                        WriteByteOffset((byte)(0b11111100));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian4ByteIndex0Offset));
                        break;
                    
                    case < (1U << 16):
                        targetPosition = m_Position + 3;
                        CheckSafeWrite(targetPosition);
                        m_Position++;
                        WriteByteOffset((byte)(0b11111101));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian4ByteIndex1Offset));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian4ByteIndex0Offset));
                        break;
                    
                    case < (1U << 24):
                        targetPosition = m_Position + 4;
                        CheckSafeWrite(targetPosition);
                        m_Position++;
                        WriteByteOffset((byte)(0b11111110));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian4ByteIndex2Offset));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian4ByteIndex1Offset));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian4ByteIndex0Offset));
                        break;
                    default:
                        targetPosition = m_Position + 5;
                        CheckSafeWrite(targetPosition);
                        m_Position++;
                        WriteByteOffset((byte)(0b11111111));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian4ByteIndex3Offset));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian4ByteIndex2Offset));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian4ByteIndex1Offset));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian4ByteIndex0Offset));
                        break;
                }
            }
            else
            {
                switch(value)
                {
                    case < 0b11111100:
                        m_Position++;
                        CheckSafeWrite(m_Position);
                        WriteByteDirect(*(valPtr + m_Endian4ByteIndex0Offset));
                        break;
                    case < (1U << 8):
                        int targetPosition = m_Position + 2;
                        CheckSafeWrite(targetPosition);
                        m_Position++;
                        WriteByteDirect((byte)(0b11111100));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian4ByteIndex0Offset));
                        break;
                    
                    case < (1U << 16):
                        targetPosition = m_Position + 3;
                        CheckSafeWrite(targetPosition);
                        m_Position++;
                        WriteByteDirect((byte)(0b11111101));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian4ByteIndex1Offset));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian4ByteIndex0Offset));
                        break;
                    
                    case < (1U << 24):
                        targetPosition = m_Position + 4;
                        CheckSafeWrite(targetPosition);
                        m_Position++;
                        WriteByteDirect((byte)(0b11111110));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian4ByteIndex2Offset));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian4ByteIndex1Offset));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian4ByteIndex0Offset));
                        break;
                    default:
                        targetPosition = m_Position + 5;
                        CheckSafeWrite(targetPosition);
                        m_Position++;
                        WriteByteDirect((byte)(0b11111111));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian4ByteIndex3Offset));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian4ByteIndex2Offset));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian4ByteIndex1Offset));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian4ByteIndex0Offset));
                        break;
                }
            }
        }
        
        public override void WriteShort(short value)
        {
            int targetPosition = m_Position + 2;
            CheckSafeWrite(targetPosition);
            byte* bytePtr = (byte*)&value;
            if(m_BitPosition > 0)
            {
                m_Position++;
                WriteByteOffset(*(bytePtr + m_Endian2ByteIndex1Offset));
                m_Position++;
                WriteByteOffset(*(bytePtr + m_Endian2ByteIndex0Offset));
            }
            else
            {
                m_Position++;
                WriteByteDirect(*(bytePtr + m_Endian2ByteIndex1Offset));
                m_Position++;
                WriteByteDirect(*(bytePtr + m_Endian2ByteIndex0Offset));
            }
        }

        public override void WriteUShort(ushort value)
        {
            int targetPosition = m_Position + 2;
            CheckSafeWrite(targetPosition);
            byte* bytePtr = (byte*)&value;
            if(m_BitPosition > 0)
            {
                m_Position++;
                WriteByteOffset(*(bytePtr + m_Endian2ByteIndex1Offset));
                m_Position++;
                WriteByteOffset(*(bytePtr + m_Endian2ByteIndex0Offset));
            }
            else
            {
                m_Position++;
                WriteByteDirect(*(bytePtr + m_Endian2ByteIndex1Offset));
                m_Position++;
                WriteByteDirect(*(bytePtr + m_Endian2ByteIndex0Offset));
            }
        }

        public override void WriteChar(char value)
        {
            int targetPosition = m_Position + 2;
            CheckSafeWrite(targetPosition);
            byte* bytePtr = (byte*)&value;
            if(m_BitPosition > 0)
            {
                m_Position++;
                WriteByteOffset(*(bytePtr + m_Endian2ByteIndex1Offset));
                m_Position++;
                WriteByteOffset(*(bytePtr + m_Endian2ByteIndex0Offset));
            }
            else
            {
                m_Position++;
                WriteByteDirect(*(bytePtr + m_Endian2ByteIndex1Offset));
                m_Position++;
                WriteByteDirect(*(bytePtr + m_Endian2ByteIndex0Offset));
            }
        }

        public override void WriteFloat(float value)
        {
            int targetPosition = m_Position + 4;
            CheckSafeWrite(targetPosition);
            byte* bytePtr = (byte*)&value;
            if(m_BitPosition > 0)
            {
                m_Position++;
                WriteByteOffset(*(bytePtr + m_Endian4ByteIndex3Offset));
                m_Position++;
                WriteByteOffset(*(bytePtr + m_Endian4ByteIndex2Offset));
                m_Position++;
                WriteByteOffset(*(bytePtr + m_Endian4ByteIndex1Offset));
                m_Position++;
                WriteByteOffset(*(bytePtr + m_Endian4ByteIndex0Offset));
            }
            else
            {
                m_Position++;
                WriteByteDirect(*(bytePtr + m_Endian4ByteIndex3Offset));
                m_Position++;
                WriteByteDirect(*(bytePtr + m_Endian4ByteIndex2Offset));
                m_Position++;
                WriteByteDirect(*(bytePtr + m_Endian4ByteIndex1Offset));
                m_Position++;
                WriteByteDirect(*(bytePtr + m_Endian4ByteIndex0Offset));
            }
        }

        public override void WriteDouble(double value)
        {
            int targetPosition = m_Position + 4;
            CheckSafeWrite(targetPosition);
            byte* bytePtr = (byte*)&value;
            if(m_BitPosition > 0)
            {
                m_Position++;
                WriteByteOffset(*(bytePtr + m_Endian8ByteIndex7Offset));
                m_Position++;
                WriteByteOffset(*(bytePtr + m_Endian8ByteIndex6Offset));
                m_Position++;
                WriteByteOffset(*(bytePtr + m_Endian8ByteIndex5Offset));
                m_Position++;
                WriteByteOffset(*(bytePtr + m_Endian8ByteIndex4Offset));
                m_Position++;
                WriteByteOffset(*(bytePtr + m_Endian8ByteIndex3Offset));
                m_Position++;
                WriteByteOffset(*(bytePtr + m_Endian8ByteIndex2Offset));
                m_Position++;
                WriteByteOffset(*(bytePtr + m_Endian8ByteIndex1Offset));
                m_Position++;
                WriteByteOffset(*(bytePtr + m_Endian8ByteIndex0Offset));
            }
            else
            {
                m_Position++;
                WriteByteDirect(*(bytePtr + m_Endian8ByteIndex7Offset));
                m_Position++;
                WriteByteDirect(*(bytePtr + m_Endian8ByteIndex6Offset));
                m_Position++;
                WriteByteDirect(*(bytePtr + m_Endian8ByteIndex5Offset));
                m_Position++;
                WriteByteDirect(*(bytePtr + m_Endian8ByteIndex4Offset));
                m_Position++;
                WriteByteDirect(*(bytePtr + m_Endian8ByteIndex3Offset));
                m_Position++;
                WriteByteDirect(*(bytePtr + m_Endian8ByteIndex2Offset));
                m_Position++;
                WriteByteDirect(*(bytePtr + m_Endian8ByteIndex1Offset));
                m_Position++;
                WriteByteDirect(*(bytePtr + m_Endian8ByteIndex0Offset));
            }
        }
        
        public override void WriteLong(long value)
        {
            WriteULong(ZigZagEncodeLong(value));
        }
        
        public override void WriteULong(ulong value)
        {
            byte* valPtr = (byte*)(ulong*)&value;
            if(m_BitPosition > 0)
            {
                switch(value)
                {
                    case < 0b11111000:
                        m_Position++;
                        CheckSafeWrite(m_Position);
                        WriteByteOffset(*(valPtr + m_Endian8ByteIndex0Offset));
                        break;
                    case < (1UL << 8):
                        int targetPosition = m_Position + 2;
                        CheckSafeWrite(targetPosition);
                        m_Position++;
                        WriteByteOffset((byte)(0b11111000));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian8ByteIndex0Offset));
                        break;
                    case < (1UL << 16):
                        targetPosition = m_Position + 3;
                        CheckSafeWrite(targetPosition);
                        m_Position++;
                        WriteByteOffset((byte)(0b11111001));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian8ByteIndex1Offset));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian8ByteIndex0Offset));
                        break;
                    case < (1UL << 24):
                        targetPosition = m_Position + 4;
                        CheckSafeWrite(targetPosition);
                        m_Position++;
                        WriteByteOffset((byte)(0b11111010));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian8ByteIndex2Offset));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian8ByteIndex1Offset));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian8ByteIndex0Offset));
                        break;
                    case < (1UL << 32):
                        targetPosition = m_Position + 5;
                        CheckSafeWrite(targetPosition);
                        m_Position++;
                        WriteByteOffset((byte)(0b11111011));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian8ByteIndex3Offset));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian8ByteIndex2Offset));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian8ByteIndex1Offset));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian8ByteIndex0Offset));
                        break;
                    case < (1UL << 40):
                        targetPosition = m_Position + 6;
                        CheckSafeWrite(targetPosition);
                        m_Position++;
                        WriteByteOffset((byte)(0b11111100));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian8ByteIndex4Offset));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian8ByteIndex3Offset));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian8ByteIndex2Offset));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian8ByteIndex1Offset));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian8ByteIndex0Offset));
                        break;
                    case < (1UL << 48):
                        targetPosition = m_Position + 7;
                        CheckSafeWrite(targetPosition);
                        m_Position++;
                        WriteByteOffset((byte)(0b11111101));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian8ByteIndex5Offset));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian8ByteIndex4Offset));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian8ByteIndex3Offset));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian8ByteIndex2Offset));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian8ByteIndex1Offset));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian8ByteIndex0Offset));
                        break;
                    case < (1UL << 56):
                        targetPosition = m_Position + 8;
                        CheckSafeWrite(targetPosition);
                        m_Position++;
                        WriteByteOffset((byte)(0b11111110));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian8ByteIndex6Offset));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian8ByteIndex5Offset));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian8ByteIndex4Offset));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian8ByteIndex3Offset));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian8ByteIndex2Offset));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian8ByteIndex1Offset));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian8ByteIndex0Offset));
                        break;
                    default:
                        targetPosition = m_Position + 9;
                        CheckSafeWrite(targetPosition);
                        m_Position++;
                        WriteByteOffset((byte)(0b11111111));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian8ByteIndex7Offset));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian8ByteIndex6Offset));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian8ByteIndex5Offset));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian8ByteIndex4Offset));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian8ByteIndex3Offset));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian8ByteIndex2Offset));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian8ByteIndex1Offset));
                        m_Position++;
                        WriteByteOffset(*(valPtr + m_Endian8ByteIndex0Offset));
                        break;
                }
            }
            else
            {
                switch(value)
                {
                    case < 0b11111000:
                        m_Position++;
                        CheckSafeWrite(m_Position);
                        WriteByteDirect(*(valPtr + m_Endian8ByteIndex0Offset));
                        break;
                    case < (1UL << 8):
                        int targetPosition = m_Position + 2;
                        CheckSafeWrite(targetPosition);
                        m_Position++;
                        WriteByteDirect((byte)(0b11111000));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian8ByteIndex0Offset));
                        break;
                    case < (1UL << 16):
                        targetPosition = m_Position + 3;
                        CheckSafeWrite(targetPosition);
                        m_Position++;
                        WriteByteDirect((byte)(0b11111001));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian8ByteIndex1Offset));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian8ByteIndex0Offset));
                        break;
                    case < (1UL << 24):
                        targetPosition = m_Position + 4;
                        CheckSafeWrite(targetPosition);
                        m_Position++;
                        WriteByteDirect((byte)(0b11111010));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian8ByteIndex2Offset));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian8ByteIndex1Offset));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian8ByteIndex0Offset));
                        break;
                    case < (1UL << 32):
                        targetPosition = m_Position + 5;
                        CheckSafeWrite(targetPosition);
                        m_Position++;
                        WriteByteDirect((byte)(0b11111011));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian8ByteIndex3Offset));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian8ByteIndex2Offset));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian8ByteIndex1Offset));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian8ByteIndex0Offset));
                        break;
                    case < (1UL << 40):
                        targetPosition = m_Position + 6;
                        CheckSafeWrite(targetPosition);
                        m_Position++;
                        WriteByteDirect((byte)(0b11111100));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian8ByteIndex4Offset));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian8ByteIndex3Offset));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian8ByteIndex2Offset));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian8ByteIndex1Offset));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian8ByteIndex0Offset));
                        break;
                    case < (1UL << 48):
                        targetPosition = m_Position + 7;
                        CheckSafeWrite(targetPosition);
                        m_Position++;
                        WriteByteDirect((byte)(0b11111101));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian8ByteIndex5Offset));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian8ByteIndex4Offset));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian8ByteIndex3Offset));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian8ByteIndex2Offset));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian8ByteIndex1Offset));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian8ByteIndex0Offset));
                        break;
                    case < (1UL << 56):
                        targetPosition = m_Position + 8;
                        CheckSafeWrite(targetPosition);
                        m_Position++;
                        WriteByteDirect((byte)(0b11111110));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian8ByteIndex6Offset));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian8ByteIndex5Offset));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian8ByteIndex4Offset));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian8ByteIndex3Offset));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian8ByteIndex2Offset));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian8ByteIndex1Offset));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian8ByteIndex0Offset));
                        break;
                    default:
                        targetPosition = m_Position + 9;
                        CheckSafeWrite(targetPosition);
                        m_Position++;
                        WriteByteDirect((byte)(0b11111111));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian8ByteIndex7Offset));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian8ByteIndex6Offset));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian8ByteIndex5Offset));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian8ByteIndex4Offset));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian8ByteIndex3Offset));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian8ByteIndex2Offset));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian8ByteIndex1Offset));
                        m_Position++;
                        WriteByteDirect(*(valPtr + m_Endian8ByteIndex0Offset));
                        break;
                }
            }
        }
        
        /*
        public override void WriteULong(ulong value)
        {
            if(value < 0b11111000)
            {
                WriteByte((byte)value);
            }
            else
            {
                const ulong HIGH_MASK = (ulong)0b11111111 << 56;
                int i = 0;
                for(; i < 8; i++)
                {
                    if((HIGH_MASK & (value << (i * 8))) > 0)
                    {
                        break;
                    }
                }
                
                
                int byteCount = (8 - i) - 1;
                
                byte header = (byte)(0b11111000 | byteCount);
                
                WriteByte(header);
                
                const ulong LOW_MASK = (ulong)0b11111111;
                for(i = byteCount; i >= 0; i--)
                {
                    WriteByte((byte)((value >> (i * 8)) & LOW_MASK));
                }
            }
        }
        */

        public override void WriteString(string value)
        {
            int stringLength = value.Length;
            WriteInt(stringLength);
            int sizeInBytes = stringLength * 2;
            
            if(m_BitPosition > 0)
            {
                //Shift to next whole byte. Faster memcpy.
                m_Position++;
                m_BitPosition = 0;
                UpdateMasks();
                m_CurrentTarget = m_StartTarget + m_Position;
            }
            
            CheckSafeWrite(m_Position);
            
            if(BitConverter.IsLittleEndian)
            {
                fixed(char* srcPtr = value)
                {
                    Buffer.MemoryCopy(srcPtr, m_CurrentTarget, capacity, sizeInBytes);
                }
                
                m_Position += (stringLength * 2);
                m_CurrentTarget = m_StartTarget + m_Position;
            }
            else
            {
                fixed(char* srcPtr = value)
                {
                    byte* bytePtr = (byte*)(srcPtr);
                    //Convert to Little Endian
                    for(int i = 0; i < stringLength; i++)
                    {
                        m_Position++;
                        WriteByteDirect(*(bytePtr + 1));
                        m_Position++;
                        WriteByteDirect(*bytePtr);
                    }
                }
            }
        }

        //Does not make any changes to the array based on endianess.
        public override void WriteByteArray(byte[] byteArray)
        {
#if DEBUG
            if(byteArray == null)
                throw new ArgumentNullException(nameof(byteArray), "Inputted byte array is null.");
#endif
            int dataLength = byteArray.Length;
            WriteInt(dataLength);
            
            CheckSafeWrite(m_Position + dataLength);
            
            if(dataLength <= 256) //Arbitrary number of bytes. Replace with whatever is more optimal and make the appropriate changes in the reader
            {
                if(m_BitPosition > 0)
                {
                    for(int i = 0; i < dataLength; i++)
                    {
                        m_Position++;
                        WriteByteOffset(byteArray[i]);
                    }
                }
                else
                {
                    for(int i = 0; i < dataLength; i++)
                    {
                        m_Position++;
                        WriteByteDirect(byteArray[i]);
                    }
                }
                
                m_CurrentTarget = m_StartTarget + m_Position;
            }
            else
            {
                if(m_BitPosition > 0)
                {
                    m_Position++;
                    m_BitPosition = 0;
                    UpdateMasks();
                    m_CurrentTarget = m_StartTarget + m_Position;
                }
                
                Marshal.Copy(byteArray, 0, (IntPtr)m_CurrentTarget, dataLength);
                m_Position += dataLength;
                m_CurrentTarget = m_StartTarget + m_Position;
            }
            
        }

        //NOTE: Not Endian safe. Use at own discretion.
        public override void WriteData(void* data, int length)
        {
            /*
            int newPosition = CheckSafeWrite(length);

            Buffer.MemoryCopy(data, m_CurrentTarget, length, length);

            m_Position = newPosition;
            m_CurrentTarget = m_StartTarget + m_Position;
            */
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteByteOffset(byte value)
        {
            unchecked
            {
                int valueInt = ((int)value) << m_InverseBitPosition;
                valueInt |= ((((int)*m_CurrentTarget) & m_Current8BitMask) << 8);
                byte* ptr = (byte*)&valueInt;
                *m_CurrentTarget = *(ptr+m_Endian4ByteIndex1Offset);
                m_CurrentTarget = m_StartTarget + m_Position;
                *m_CurrentTarget = *(ptr+m_Endian4ByteIndex0Offset);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteByteDirect(byte value)
        {
            *m_CurrentTarget = value;
            m_CurrentTarget = m_StartTarget + m_Position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckSafeWrite(int newPosition)
        {
            while(newPosition >= capacity)
            {
                if(newPosition == capacity && m_BitPosition == 0)
                    return;
                //Not enough room
                Expand();
            }
        }

        protected override void OnResize(int newSize)
        {
            IntPtr newAlloc = Marshal.AllocHGlobal(newSize);
            int amountToCopy = capacity;
            if(amountToCopy > newSize)
                amountToCopy = newSize;
            Buffer.MemoryCopy(m_StartTarget, (void*)newAlloc, newSize, amountToCopy);

            if(m_OwnsTheTarget)
            {
                Marshal.FreeHGlobal((IntPtr)m_StartTarget);
            }

            m_StartTarget = (byte*)newAlloc;
            m_CurrentTarget = m_StartTarget + m_Position;
        }
    }
}