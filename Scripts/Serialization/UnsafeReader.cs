using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Elanetic.Tools.Serialization
{
    public unsafe class UnsafeReader
    {
        public int position
        {
            get => m_Position;
            set
            {
#if DEBUG
                if(value < 0 || value >= length)
                    throw new IndexOutOfRangeException("Position of Unsafe Reader must be between 0 and less than the length.");
#endif
                m_Position = value;
                m_CurrentTarget = m_StartTarget + m_Position;
            }
        }
        public int length => m_Length;

        private byte* m_StartTarget;
        private byte* m_CurrentTarget;
        private int m_Position;
        private int m_Length;

        public UnsafeReader(IntPtr target, int length) : this((byte*)target, length)
        {
        }

        public UnsafeReader(void* target, int length) : this((byte*)target, length)
        {
        }

        public UnsafeReader(byte* target, int length)
        {
            m_StartTarget = target;
            m_CurrentTarget = m_StartTarget;
            m_Position = 0;
            m_Length = length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadByte()
        {
            int newPosition = CheckSafeRead(1);

            byte value = *m_CurrentTarget;

            m_Position = newPosition;
            m_CurrentTarget = m_StartTarget + m_Position;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadBool() => ReadByte() != 0;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt()
        {
            int newPosition = CheckSafeRead(4);

            int value = *(int*)m_CurrentTarget;

            m_Position = newPosition;
            m_CurrentTarget = m_StartTarget + m_Position;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadUInt()
        {
            int newPosition = CheckSafeRead(4);

            uint value = *(uint*)m_CurrentTarget;

            m_Position = newPosition;
            m_CurrentTarget = m_StartTarget + m_Position;
            return value;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ReadShort()
        {
            int newPosition = CheckSafeRead(2);

            short value = *(short*)m_CurrentTarget;

            m_Position = newPosition;
            m_CurrentTarget = m_StartTarget + m_Position;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadUShort()
        {
            int newPosition = CheckSafeRead(2);

            ushort value = *(ushort*)m_CurrentTarget;

            m_Position = newPosition;
            m_CurrentTarget = m_StartTarget + m_Position;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public char ReadChar()
        {
            int newPosition = CheckSafeRead(2);

            char value = *(char*)m_CurrentTarget;

            m_Position = newPosition;
            m_CurrentTarget = m_StartTarget + m_Position;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float ReadFloat()
        {
            int newPosition = CheckSafeRead(4);

            float value = *(float*)m_CurrentTarget;

            m_Position = newPosition;
            m_CurrentTarget = m_StartTarget + m_Position;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double ReadDouble()
        {
            int newPosition = CheckSafeRead(8);

            double value = *(double*)m_CurrentTarget;

            m_Position = newPosition;
            m_CurrentTarget = m_StartTarget + m_Position;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadLong()
        {
            int newPosition = CheckSafeRead(8);

            long value = *(long*)m_CurrentTarget;

            m_Position = newPosition;
            m_CurrentTarget = m_StartTarget + m_Position;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ReadULong()
        {
            int newPosition = CheckSafeRead(8);

            ulong value = *(ulong*)m_CurrentTarget;

            m_Position = newPosition;
            m_CurrentTarget = m_StartTarget + m_Position;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ReadString()
        {
            int expectedLength = ReadInt();

            int newPosition = CheckSafeRead(2 * expectedLength);
            StringBuilder stringBuilder = new StringBuilder(expectedLength);

            stringBuilder.Append((char*)m_CurrentTarget, expectedLength);

            m_Position = newPosition;
            m_CurrentTarget = m_StartTarget + m_Position;
            return stringBuilder.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadByteArray(byte[] byteArray)
        {
#if DEBUG
            if(byteArray == null)
                throw new ArgumentNullException(nameof(byteArray), "Inputted byte array is null.");
#endif

            int dataLength = ReadInt();
            int newPosition = CheckSafeRead(dataLength);
            int destinationLength = byteArray.Length;

            if(destinationLength < dataLength)
            {
                Marshal.Copy((IntPtr)m_CurrentTarget, byteArray, 0, destinationLength);
            }
            else
            {
                Marshal.Copy((IntPtr)m_CurrentTarget, byteArray, 0, dataLength);
            }

            m_Position = newPosition;
            m_CurrentTarget = m_StartTarget + m_Position;
            return dataLength;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int CheckSafeRead(int size)
        {
            int newSize = m_Position + size;
            if(newSize > m_Length)
            {
                throw new IndexOutOfRangeException("Trying to read outside the bounds of the data. Position: " + m_Position.ToString() + " Length: " + m_Length + " Trying to read " + size + " bytes.");
            }
            return newSize;
        }
    }
}