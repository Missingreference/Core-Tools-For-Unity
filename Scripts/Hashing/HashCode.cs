namespace Elanetic.Tools.Hashing
{
    /// <summary>
    /// Functions for converting to non-cryptographic stable hash codes.
    /// Will always return the same hash for the same string.
    /// An implementation of FNV-1 32 bit xor folded to 16 bit
    /// https://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function
    /// //Credit: MLAPI / TwoTenPvP
    /// </summary>
    public static class HashCode
    {

        private const uint FNV_offset_basis32 = 2166136261;
        private const uint FNV_prime32 = 16777619;

        private const ulong FNV_offset_basis64 = 14695981039346656037;
        private const ulong FNV_prime64 = 1099511628211;

        #region String

        /// <summary>
        /// Convert a string to a non-cryptographic stable hash code.
        /// </summary>
        /// <returns>A stable hash as an unsigned short</returns>
        /// <param name="inputString">The input string to be converted.</param>
        static public ushort HashStringShort(string inputString)
        {
            uint hash32 = HashString(inputString);

            return (ushort)((hash32 >> 16) ^ hash32);
        }

        /// <summary>
        /// Convert a string to a non-cryptographic stable hash code.
        /// </summary>
        /// <returns>A stable hash as an unsigned integer</returns>
        /// <param name="inputString">The input string to be converted.</param>
        static public uint HashString(string inputString)
        {
            unchecked
            {
                uint hash = FNV_offset_basis32;
                for (int i = 0; i < inputString.Length; i++)
                {
                    uint ch = inputString[i];
                    hash = hash * FNV_prime32;
                    hash = hash ^ ch;
                }
                return hash;
            }
        }

        /// <summary>
        /// Convert a string to a non-cryptographic stable hash code.
        /// </summary>
        /// <returns>A stable hash as an unsigned long integers</returns>
        /// <param name="inputString">The input string to be converted.</param>
        internal static ulong HashStringLong(string inputString)
        {
            unchecked
            {
                ulong hash = FNV_offset_basis64;
                for (int i = 0; i < inputString.Length; i++)
                {
                    ulong ch = inputString[i];
                    hash = hash * FNV_prime64;
                    hash = hash ^ ch;
                }
                return hash;
            }
        }

        #endregion String

        #region Bytes

        /// <summary>
        /// Convert a byte array to a non-cryptographic stable hash code.
        /// </summary>
        /// <returns>A stable hash as an unsigned short</returns>
        /// <param name="inputString">The input string to be converted.</param>
        static public ushort HashBytesShort(byte[] bytes)
        {
            uint hash32 = HashBytes(bytes);

            return (ushort)((hash32 >> 16) ^ hash32);
        }

        /// <summary>
        /// Convert a byte array to a non-cryptographic stable hash code.
        /// </summary>
        /// <returns>A stable hash as an unsigned integer</returns>
        /// <param name="inputString">The input string to be converted.</param>
        static public uint HashBytes(this byte[] bytes)
        {
            unchecked
            {
                uint hash = FNV_offset_basis32;
                for (int i = 0; i < bytes.Length; i++)
                {
                    uint bt = bytes[i];
                    hash = hash * FNV_prime32;
                    hash = hash ^ bt;
                }
                return hash;
            }
        }

        /// <summary>
        /// Convert a byte array to a non-cryptographic stable hash code.
        /// </summary>
        /// <returns>A stable hash as an unsigned long integer</returns>
        /// <param name="inputString">The input string to be converted.</param>
        static public ulong HashBytesLong(byte[] bytes)
        {
            unchecked
            {
                ulong hash = FNV_offset_basis64;
                for (int i = 0; i < bytes.Length; i++)
                {
                    ulong bt = bytes[i];
                    hash = hash * FNV_prime64;
                    hash = hash ^ bt;
                }
                return hash;
            }
        }

        #endregion Bytes
    }
}