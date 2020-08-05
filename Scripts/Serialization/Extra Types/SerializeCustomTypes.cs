namespace Elanetic.Tools.Serialization
{
    public static class SerializeCustomTypes
    {
        #region BoundsInt2D

        /// <summary>
        /// Write a BoundsInt2D to the stream.
        /// </summary>
        /// <param name="boundsInt2D">The BoundsInt2D to write to the stream.</param>
        static public void WriteBoundsInt2D(this BitWriter writer, BoundsInt2D boundsInt2D)
        {
            writer.WriteVector2Int(boundsInt2D.min);
            writer.WriteVector2Int(boundsInt2D.max);
        }

        /// <summary>
        /// Read a BoundsInt2D from the stream.
        /// </summary>
        static public void ReadBoundsInt2D(this BitReader reader) => new BoundsInt2D(reader.ReadVector2Int(), reader.ReadVector2Int());

        #endregion BoundsInt2D
    }
}
