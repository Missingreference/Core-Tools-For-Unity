using System;
using UnityEngine;

namespace Elanetic.Tools.Serialization.Unity
{
    public static class SerializeUnityTypes
    {
        #region Texture2D

        /// <summary>
        /// Write a Texture2D to the stream.
        /// </summary>
        /// <param name="texture2D">The Texture2D to write to the stream.</param>
        public static void WriteTexture2D(this BitWriter writer, Texture2D texture2D)
        {
#if DEBUG
            if(texture2D == null) 
                throw new ArgumentNullException(nameof(texture2D), "Inputted texture is null.");
#endif

            writer.WriteInt(texture2D.width);
            writer.WriteInt(texture2D.height);
            writer.WriteInt((int)texture2D.format);

            writer.WriteByteArray(texture2D.GetRawTextureData());
        }

        /// <summary>
        /// Read a Texture2D from the stream.
        /// </summary>
        /// <returns>The Texture2D retrieved from the stream.</returns>
        public static Texture2D ReadTexture2D(this BitReader reader)
        {
            int width = reader.ReadInt();
            int height = reader.ReadInt();
            TextureFormat format = (TextureFormat)reader.ReadInt();

            Texture2D texture2D = new Texture2D(width, height, format, false);

            texture2D.LoadRawTextureData(reader.ReadByteArray());
            texture2D.Apply();

            return texture2D;
        }

        #endregion Texture2D

        #region Vector2

        /// <summary>
        /// Write a Vector2 to the stream.
        /// </summary>
        /// <param name="vector2">The Vector2 to write to the stream.</param>
        static public void WriteVector2(this BitWriter writer, Vector2 vector2)
        {
            writer.WriteFloat(vector2.x);
            writer.WriteFloat(vector2.y);
        }

        /// <summary>
        /// Read a Vector2 from the stream.
        /// </summary>
        /// <returns>The Vector2 retrieved from the stream.</returns>
        static public Vector2 ReadVector2(this BitReader reader) => new Vector2(reader.ReadFloat(), reader.ReadFloat());

        #endregion Vector2

        #region Vector3

        /// <summary>
        /// Write a Vector3 to the stream.
        /// </summary>
        /// <param name="vector3">The Vector3 to write to the stream.</param>
        static public void WriteVector3(this BitWriter writer, Vector3 vector3)
        {
            writer.WriteFloat(vector3.x);
            writer.WriteFloat(vector3.y);
            writer.WriteFloat(vector3.z);
        }

        /// <summary>
        /// Read a Vector3 from the stream.
        /// </summary>
        /// <returns>The Vector3 retrieved from the stream.</returns>
        static public Vector3 ReadVector3(this BitReader reader) => new Vector3(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat());
        
        #endregion Vector3

        #region Vector4

        /// <summary>
        /// Write a Vector4 to the stream.
        /// </summary>
        /// <param name="vector4">The Vector4 to write to the stream.</param>
        static public void WriteVector4(this BitWriter writer, Vector4 vector4)
        {
            writer.WriteFloat(vector4.x);
            writer.WriteFloat(vector4.y);
            writer.WriteFloat(vector4.z);
            writer.WriteFloat(vector4.w);
        }

        /// <summary>
        /// Read a Vector4 from the stream.
        /// </summary>
        /// <returns>The Vector4 to read from the stream.</returns>
        static public Vector4 ReadVector4(this BitReader reader) => new Vector4(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat());

        #endregion Vector4

        #region Vector2Int

        /// <summary>
        /// Write a Vector2Int to the stream.
        /// </summary>
        /// <param name="vector2Int">The Vector2Int to write to the stream.</param>
        static public void WriteVector2Int(this BitWriter writer, Vector2Int vector2Int)
        {
            writer.WriteInt(vector2Int.x);
            writer.WriteInt(vector2Int.y);
        }

        /// <summary>
        /// Read a Vector2Int from the stream.
        /// </summary>
        /// <returns>The Vector2Int retrieved from the stream.</returns>
        static public Vector2Int ReadVector2Int(this BitReader reader) => new Vector2Int(reader.ReadInt(), reader.ReadInt());

        #endregion Vector2Int

        #region Vector3Int

        /// <summary>
        /// Write a Vector3Int to the stream.
        /// </summary>
        /// <param name="vector3Int">The Vector3Int to write to the stream.</param>
        static public void WriteVector3Int(this BitWriter writer, Vector3Int vector3Int)
        {
            writer.WriteInt(vector3Int.x);
            writer.WriteInt(vector3Int.y);
            writer.WriteInt(vector3Int.z);
        }

        /// <summary>
        /// Read a Vector3Int from the stream.
        /// </summary>
        /// <returns>The Vector3Int retrieved from the stream.</returns>
        static public Vector3Int ReadVector3Int(this BitReader reader) => new Vector3Int(reader.ReadInt(), reader.ReadInt(), reader.ReadInt());

        #endregion Vector3Int

        #region Color

        /// <summary>
        /// Write a Color to the stream.
        /// </summary>
        /// <param name="color">The Color to write to the stream.</param>
        static public void WriteColor(this BitWriter writer, Color color) => writer.WriteColor32(color);

        /// <summary>
        /// Write a Color32 to the stream.
        /// </summary>
        /// <param name="color32">The Color32 to write to the stream.</param>
        static public void WriteColor32(this BitWriter writer, Color32 color32)
        {
            writer.WriteByte(color32.r);
            writer.WriteByte(color32.g);
            writer.WriteByte(color32.b);
            writer.WriteByte(color32.a);
        }

        /// <summary>
        /// Read a Color from the stream.
        /// </summary>
        /// <returns>The Color retrieved from the stream.</returns>
        static public Color ReadColor(this BitReader reader) => reader.ReadColor32();
        
        /// <summary>
        /// Read a Color32 from the stream.
        /// </summary>
        /// <returns>The Color32 retrieved from the stream.</returns>
        static public Color32 ReadColor32(this BitReader reader) => new Color32(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());

        #endregion Color

        #region Ray

        /// <summary>
        /// Write a Ray to the stream.
        /// </summary>
        /// <param name="ray">The Ray to write to the stream.</param>
        static public void WriteRay(this BitWriter writer, Ray ray)
        {
            writer.WriteVector3(ray.origin);
            writer.WriteVector3(ray.direction);
        }

        /// <summary>
        /// Read a Ray from the stream.
        /// </summary>
        /// <returns>The Ray retrieved from the stream.</returns>
        static public Ray ReadRay(this BitReader reader) => new Ray(reader.ReadVector3(), reader.ReadVector3());

        #endregion Ray

        #region Quaternion

        /// <summary>
        /// Write a Quaternion to the stream.
        /// </summary>
        /// <param name="rotation">The Quaternion to write to the stream.</param>
        static public void WriteRotation(this BitWriter writer, Quaternion rotation)
        {
            writer.WriteFloat(rotation.x);
            writer.WriteFloat(rotation.y);
            writer.WriteFloat(rotation.z);
            writer.WriteFloat(rotation.w);
        }

        /// <summary>
        /// Read a Quaternion from the stream.
        /// </summary>
        /// <returns>The Quaternion retrieved from the stream.</returns>
        static public Quaternion ReadRotation(this BitReader reader) => new Quaternion(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat());

        #endregion Quaternion

        #region Bounds

        /// <summary>
        /// Write a Bounds to the stream.
        /// </summary>
        /// <param name="bounds">The Bounds to write to the stream.</param>
        static public void WriteBounds(this BitWriter writer, Bounds bounds)
        {
            writer.WriteVector3(bounds.center);
            writer.WriteVector3(bounds.size);
        }

        /// <summary>
        /// Read a Bounds from the stream.
        /// </summary>
        /// <returns>The Bounds retrieved from the stream.</returns>
        static public Bounds ReadBounds(this BitReader reader) => new Bounds(reader.ReadVector3(), reader.ReadVector3());

        #endregion Bounds

        #region BoundsInt

        /// <summary>
        /// Write a BoundsInt to the stream.
        /// </summary>
        /// <param name="boundsInt">The BoundsInt to write to the stream.</param>
        static public void WriteBoundsInt(this BitWriter writer, BoundsInt boundsInt)
        {
            writer.WriteVector3Int(boundsInt.position);
            writer.WriteVector3Int(boundsInt.size);
        }

        /// <summary>
        /// Read a BoundsInt from the stream.
        /// </summary>
        static public BoundsInt ReadBoundsInt(this BitReader reader) => new BoundsInt(reader.ReadVector3Int(), reader.ReadVector3Int());

        #endregion BoundsInt
    }
}