using System;

namespace Elanetic.Tools.Serialization
{
    public static class SerializeSystemTypes
    {
        #region Version

        /// <summary>
        /// Write a Version to the stream.
        /// </summary>
        /// <param name="version">The Version to write to the stream.</param>
        static public void WriteVersion(this BitWriter writer, Version version)
        {
            writer.WriteInt(version.Major);
            writer.WriteInt(version.Minor);
            writer.WriteInt(version.Build);
            writer.WriteInt(version.MajorRevision);
            writer.WriteInt(version.MinorRevision);
        }

        /// <summary>
        /// Read a Version from the stream.
        /// </summary>
        /// <returns>The Version retrieved from the stream.</returns>
        static public Version ReadVersion(this BitReader reader)
        {
            int major = reader.ReadInt();
            int minor = reader.ReadInt();
            int build = reader.ReadInt();
            int majorRevision = reader.ReadInt();
            int minorRevision = reader.ReadInt();

            //These if statements make it so that Version's unused properties(potentially Build, MajorRevision or MinorRevision)
            //are correctly -1 to match the written Version since passing anything less than zero into Version's constructor causes an exception
            if(build >= 0)
            {
                if(majorRevision >= 0)
                {
                    if(minorRevision >= 0)
                    {
                        return new Version(major, minor, build, (majorRevision << 16) + minorRevision);
                    }
                    return new Version(major, minor, build, majorRevision << 16);
                }
                return new Version(major, minor, build);
            }

            return new Version(major, minor);
        }

        #endregion Version

        #region DateTime

        /// <summary>
        /// Write a DateTime to the stream.
        /// </summary>
        /// <param name="dateTime">The DateTime to write to the stream.</param>
        static public void WriteDateTime(this BitWriter writer, DateTime dateTime)
        {
            writer.WriteLong(dateTime.Ticks);
            writer.WriteInt((int)dateTime.Kind);
        }

        /// <summary>
        /// Read a DateTime from the stream.
        /// </summary>
        /// <returns>The DateTime retrieved from the stream.</returns>
        static public DateTime ReadDateTime(this BitReader reader)
        {
            return new DateTime(reader.ReadLong(), (DateTimeKind)reader.ReadInt());
        }

        #endregion DateTime

        #region Type

        /// <summary>
        /// Write a System.Type to the stream.
        /// </summary>
        /// <param name="type">The Type to write to the stream.</param>
        static public void WriteType(this BitWriter writer, Type type)
        {
            writer.WriteString(type.AssemblyQualifiedName);
        }

        /// <summary>
        /// Read a System.Type from stream.
        /// </summary>
        /// <returns>The Type retrieved from the stream.</returns>
        static public Type ReadType(this BitReader reader)
        {
            return Type.GetType(reader.ReadString());
        }

        #endregion
    }
}