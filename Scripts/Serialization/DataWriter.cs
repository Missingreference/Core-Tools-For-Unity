using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Elanetic.Tools.Serialization
{
    public abstract unsafe class DataWriter : IDisposable
    {
        /// <summary>
        /// The byte position of the writer.
        /// </summary>
        public int position
        {
            get => m_Position;
            set
            {
                if(value < 0 || value > m_Capacity)
                    throw new IndexOutOfRangeException("Position of Unsafe Writer must be between 0 and less than or equal the capacity. Inputted: " + value.ToString());

                m_Position = value;
                m_CurrentTarget = m_StartTarget + m_Position;
            }
        }
        
        /// <summary>
        /// The bit position of the writer. The index of the bit within the byte position.
        /// </summary>
        public int bitPosition
        {
            get => m_BitPosition;
            set
            {
                if(value < 0 || value >= 8)
                {
                    throw new InvalidOperationException("The sub position must be a value from 0 to 7 (the index of the bit in a byte).");
                }

                if(m_BitPosition > 0 && m_Position == m_Capacity)
                {
                    throw new IndexOutOfRangeException("Position of Unsafe Writer must be between 0 and less than or equal the capacity including a bit position of 0.");
                }

                m_BitPosition = value;
                
                UpdateMasks();
            }
        }

        public delegate void OnResizeCallback();
        
        /// <summary>
        /// Called when attempting to write to the target but there is no more room at destination position.
        /// Before this invocation this class will allocate new memory and copy over the old contents.
        /// This class will not free the old allocation unless the allocation was allocated
        /// </summary>
        public event OnResizeCallback onResize;

        /// <summary>
        /// The start of the memory allocation that the writing will target.
        /// </summary>
        public byte* target => m_StartTarget;

        /// <summary>
        /// The capacity of the memory target.
        /// </summary>
        public int capacity => m_Capacity;
        
        protected byte* m_StartTarget;
        protected byte* m_CurrentTarget;
        protected int m_Position = 0;
        protected int m_BitPosition = 0;
        protected int m_InverseBitPosition = 8;
        protected int m_Current8BitMask;
        protected int m_CurrentInverse8BitMask;
        protected int m_Current16BitMask;
        protected int m_CurrentInverse16BitMask;
        protected int m_Current32BitMask;
        protected int m_CurrentInverse32BitMask;
        protected long m_Current64BitMask;
        protected int m_CurrentInverse64BitMask;
        
        private int m_Capacity;

        protected DataWriter(int capacity)
        {
            if(capacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be more than zero.");
            }
            m_Capacity = capacity;
            UpdateMasks();
        }
        
        /// <summary>
        /// Set the capacity of the internal memory buffer. Position must be within the bounds of the new capacity.
        /// Does not call the onResize event.
        /// </summary>
        public void SetCapacity(int size)
        {
            if(m_Position > capacity)
            {
                throw new IndexOutOfRangeException("The position must be within the bounds of the new capacity before resizing.");
            }
            if(size <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size), "New capacity must be more than zero.");
            }
            
            OnResize(size);
            m_Capacity = size;
        }

        public abstract void Dispose();

        #region Write Types

        public abstract void WriteByte(byte value);
        
        public abstract void WriteBool(bool value);

        public abstract void WriteInt(int value);

        public abstract void WriteUInt(uint value);

        public abstract void WriteShort(short value);

        public abstract void WriteUShort(ushort value);

        public abstract void WriteChar(char value);

        public abstract void WriteFloat(float value);

        public abstract void WriteDouble(double value);

        public abstract void WriteLong(long value);

        public abstract void WriteULong(ulong value);

        public abstract void WriteString(string value);

        public abstract void WriteByteArray(byte[] byteArray);

        public abstract void WriteData(void* data, int length);

        #endregion Write Types
        
        protected void UpdateMasks()
        {
            m_InverseBitPosition = 8 - m_BitPosition;
            m_Current8BitMask = (~(-1 << m_BitPosition)) << m_InverseBitPosition;
            m_CurrentInverse8BitMask = ~(-1 << m_InverseBitPosition);
            m_Current16BitMask = m_Current8BitMask << 8;
            m_Current32BitMask = m_Current8BitMask << 24;
            m_Current64BitMask = ((long)m_Current8BitMask) << 56;
        }
        
        protected void Expand()
        {
            int newCapacity = m_Capacity * 2;
            OnResize(newCapacity);
            m_Capacity = newCapacity;
            onResize?.Invoke();
        }

        protected abstract void OnResize(int newSize);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected uint ZigZagEncodeInt(int value)
        {
            return (uint)((value >> 31) ^ (value << 1));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected ulong ZigZagEncodeLong(long value)
        {
            return (ulong)((value >> 63) ^ (value << 1));
        }
        
        
        [StructLayout(LayoutKind.Explicit)]
        protected struct FloatUIntLayout
        {
            [FieldOffset(0)]
            public float valueAsFloat;

            [FieldOffset(0)]
            public uint valueAsUint;
        }
        
        [StructLayout(LayoutKind.Explicit)]
        protected struct DoubleULongLayout
        {
            [FieldOffset(0)]
            public double valueAsDouble;

            [FieldOffset(0)]
            public ulong valueAsUlong;
        }
    }
}