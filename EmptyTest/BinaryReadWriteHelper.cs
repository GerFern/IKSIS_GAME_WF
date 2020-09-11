using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EmptyTest.BinaryWriterExtensions
{
    public static class BinaryReadWriteHelper
    {
        public static TEnum ReadEnum<TEnum>(this BinaryReader reader) where TEnum : Enum
        {
            Type baseType = typeof(TEnum).GetEnumUnderlyingType();
            return (TEnum)reader.GetType().GetMethod("Read" + baseType.Name).Invoke(reader, null);
        }

        public static void WriteEnum<TEnum>(this BinaryWriter writer, TEnum @enum) where TEnum : Enum
        {
            writer.GetType().GetMethod("Write", new Type[] { @enum.GetType().GetEnumUnderlyingType() }).Invoke(writer, new object[] { @enum });
        }
    }
}
