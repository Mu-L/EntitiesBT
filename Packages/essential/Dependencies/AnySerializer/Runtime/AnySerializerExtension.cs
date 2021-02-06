using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using UnityEngine.Assertions;

namespace AnySerializer
{
    public static class AnySerializerExtension
    {
        // TODO: serialize object recursively
        public static IEnumerable<AnySerializeFieldData> SerializeAny([NotNull] this IFormatter formatter, [NotNull] object obj)
        {
            var type = obj.GetType();
            return from info in type.GetAnySerializableFields()
                select new AnySerializeFieldData {Name = info.Name, Data = Serialize(formatter, info.GetValue(obj))}
            ;

            static byte[] Serialize(IFormatter formatter, object obj)
            {
                if (obj == null) return Array.Empty<byte>();
                using var stream = new MemoryStream();
                formatter.Serialize(stream, obj);
                return stream.ToArray();
            }
        }

        // TODO: deserialize object recursively
        public static void DeserializeAny([NotNull] this IFormatter formatter, [NotNull] object obj, IEnumerable<AnySerializeFieldData> data)
        {
            if (data == null) return;

            var type = obj.GetType();
            foreach (var d in data)
            {
                var value = Deserialize(formatter, d.Data);
                var field = type.GetField(d.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                Assert.IsNotNull(field);
                Assert.IsTrue(field.IsAnySerializableField());
                field.SetValue(obj, value);
            }

            static object Deserialize(IFormatter formatter, byte[] data)
            {
                if (!data.Any()) return null;
                using var stream = new MemoryStream(data);
                return formatter.Deserialize(stream);
            }
        }

        public static bool IsAnySerializableField([NotNull] this FieldInfo fieldInfo)
        {
            if (fieldInfo.IsStatic) return false;
            if (fieldInfo.IsInitOnly) return false;
            if (fieldInfo.IsNotSerialized) return false;
            if (fieldInfo.IsConst()) return false;
            if (fieldInfo.GetCustomAttribute<AnySerializeFieldAttribute>() == null) return false;
            return true;
        }

        public static IEnumerable<FieldInfo> GetAnySerializableFields([NotNull] this Type type)
        {
            return type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(fieldInfo => fieldInfo.IsAnySerializableField())
            ;
        }
    }
}