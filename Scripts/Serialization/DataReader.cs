using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Elanetic.Tools.Serialization
{
    public abstract unsafe class DataReader
    {
        /// <summary>
        /// The byte position of the reader.
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
                    throw new IndexOutOfRangeException("Position of Unsafe Reader must be between 0 and less than or equal the capacity including a bit position of 0.");
                }

                m_BitPosition = value;
                
                UpdateMasks();
            }
        }
        
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

        protected DataReader(int capacity)
        {
            if(capacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be more than zero.");
            }
            m_Capacity = capacity;
            UpdateMasks();
        }
        
        #region Read Types
        
        public abstract byte ReadByte();
        
        public abstract bool ReadBool();

        public abstract int ReadInt();

        public abstract uint ReadUInt();

        public abstract short ReadShort();

        public abstract ushort ReadUShort();

        public abstract char ReadChar();

        public abstract float ReadFloat();

        public abstract double ReadDouble();

        public abstract long ReadLong();

        public abstract ulong ReadULong();

        public abstract string ReadString();

        public abstract void ReadByteArray(byte[] destinationArray);

        public abstract void ReadData(void* destination, int length);
        
        #endregion
        
        protected void UpdateMasks()
        {
            m_InverseBitPosition = 8 - m_BitPosition;
            m_Current8BitMask = (~(-1 << m_BitPosition)) << m_InverseBitPosition;
            m_CurrentInverse8BitMask = ~(-1 << m_InverseBitPosition);
            m_Current16BitMask = m_Current8BitMask << 8;
            m_Current32BitMask = m_Current8BitMask << 24;
            m_Current64BitMask = ((long)m_Current8BitMask) << 56;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected int ZigZagDecodeInt(uint value)
        {
            return (((int)(value >> 1) & int.MaxValue) ^ ((int)(value << 31) >> 31));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected long ZigZagDecodeLong(ulong value)
        {
            return (((long)(value >> 1) & long.MaxValue) ^ ((long)(value << 63) >> 63));
        }

    }
}