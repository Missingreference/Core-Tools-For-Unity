using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Elanetic.Tools.Serialization
{
    public unsafe class UnsafeWriter
    {
        public int position
        {
            get => m_Position;
            set
            {
                if(value < 0 || value > length)
                    throw new IndexOutOfRangeException("Position of Unsafe Writer must be between 0 and less than or equal the length. Inputted: " + value.ToString());

                m_Position = value;
                m_CurrentTarget = m_StartTarget + m_Position;
            }
        }
        public int length => m_Length;

        public delegate void ResizeRequiredCallback();
        /// <summary>
        /// Called when attempting to write to the target but there is no more room at destination position. Use this callback to be given a chance to call SetTarget with a new destination otherwise an exception will be thrown. Old data will be copied to the new location.
        /// Failing to properly pass in a sufficient new size will result in an exception being thrown.
        /// </summary>
        public event ResizeRequiredCallback OnRequireResize;

        private byte* m_StartTarget;
        private byte* m_CurrentTarget;
        private int m_Position;
        private int m_Length;

        public UnsafeWriter(IntPtr target, int length) : this((byte*)target, length)
        {
        }

        public UnsafeWriter(void* target, int length) : this((byte*)target, length)
        {
        }

        public UnsafeWriter(byte* target, int length)
        {
            m_StartTarget = target;
            m_CurrentTarget = m_StartTarget;
            m_Position = 0;
            m_Length = length;
        }

        /// <summary>
        /// Set the new destination for the data to be written to. Typically used to resize the underlying destination allocation. The old target will be copied to the new target.
        /// It is safe to dealloc the old target upon the returning of this function.
        /// </summary>
        public void SetTarget(IntPtr newTarget, int newLength)
        {
            if(m_Position >= newLength)
            {
                //Set to a valid position before trying to resize.
                throw new IndexOutOfRangeException("Position is currently set outside the bounds the new target's length.");
            }

            int amountToCopy = m_Length;
            if(newLength < m_Length)
            {
                amountToCopy = newLength;
            }

            Buffer.MemoryCopy(m_StartTarget, (void*)newTarget, newLength, amountToCopy);
            m_Length = newLength;
            m_StartTarget = (byte*)newTarget;
            m_CurrentTarget = m_StartTarget + m_Position;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteByte(byte value)
        {
            int newPosition = CheckSafeWrite(1);

            *m_CurrentTarget = value;

            m_Position = newPosition;
            m_CurrentTarget = m_StartTarget + m_Position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBool(bool value)
        {
            if(value)
            {
                WriteByte(1);
            }
            else
            {
                WriteByte(0);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt(int value)
        {
            int newPosition = CheckSafeWrite(4);

            *(int*)m_CurrentTarget = value;

            m_Position = newPosition;
            m_CurrentTarget = m_StartTarget + m_Position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt(uint value)
        {
            int newPosition = CheckSafeWrite(4);

            *(uint*)m_CurrentTarget = value;

            m_Position = newPosition;
            m_CurrentTarget = m_StartTarget + m_Position;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteShort(short value)
        {
            int newPosition = CheckSafeWrite(2);

            *(short*)m_CurrentTarget = value;

            m_Position = newPosition;
            m_CurrentTarget = m_StartTarget + m_Position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUShort(ushort value)
        {
            int newPosition = CheckSafeWrite(2);

            *(ushort*)m_CurrentTarget = value;

            m_Position = newPosition;
            m_CurrentTarget = m_StartTarget + m_Position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteChar(char value)
        {
            int newPosition = CheckSafeWrite(2);

            *(char*)m_CurrentTarget = value;

            m_Position = newPosition;
            m_CurrentTarget = m_StartTarget + m_Position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteFloat(float value)
        {
            int newPosition = CheckSafeWrite(4);

            *(float*)m_CurrentTarget = value;

            m_Position = newPosition;
            m_CurrentTarget = m_StartTarget + m_Position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteDouble(double value)
        {
            int newPosition = CheckSafeWrite(8);

            *(double*)m_CurrentTarget = value;

            m_Position = newPosition;
            m_CurrentTarget = m_StartTarget + m_Position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteLong(long value)
        {
            int newPosition = CheckSafeWrite(8);

            *(long*)m_CurrentTarget = value;

            m_Position = newPosition;
            m_CurrentTarget = m_StartTarget + m_Position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteULong(ulong value)
        {
            int newPosition = CheckSafeWrite(8);

            *(ulong*)m_CurrentTarget = value;

            m_Position = newPosition;
            m_CurrentTarget = m_StartTarget + m_Position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteString(string value)
        {
            int expectedLength = value.Length;
            WriteInt(expectedLength);
            int sizeInBytes = 2 * expectedLength;
            int newPosition = CheckSafeWrite(sizeInBytes);

            fixed(char* srcPtr = value)
            {
                Buffer.MemoryCopy(srcPtr, m_CurrentTarget, m_Length, sizeInBytes);
            }

            m_Position = newPosition;
            m_CurrentTarget = m_StartTarget + m_Position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteByteArray(byte[] byteArray)
        {
#if DEBUG
            if(byteArray == null)
                throw new ArgumentNullException(nameof(byteArray), "Inputted byte array is null.");
#endif
            int dataLength = byteArray.Length;
            WriteInt(dataLength);

            int newPosition = CheckSafeWrite(dataLength);

            Marshal.Copy(byteArray, 0, (IntPtr)m_CurrentTarget, dataLength);

            m_Position = newPosition;
            m_CurrentTarget = m_StartTarget + m_Position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int CheckSafeWrite(int size)
        {
            int newSize = m_Position + size;
            if(newSize > m_Length)
            {
                //Not enough room, give the user a chance to resize the target.
                if(OnRequireResize == null)
                {
                    throw new IndexOutOfRangeException("Trying to write outside the bounds of the data. Position: " + m_Position.ToString() + " Length: " + m_Length + " Trying to write " + size + " bytes. Optionally you can use OnRequireResize callback to resize new target.");
                }

                OnRequireResize();

                if(newSize > m_Length)
                {
                    throw new IndexOutOfRangeException("Target was not resized to a valid target. Use SetTarget to resize. Trying to read outside the bounds of the data. Position: " + m_Position.ToString() + " Length: " + m_Length + " Trying to read " + size + " bytes.");
                }
            }
            return newSize;
        }
    }
}