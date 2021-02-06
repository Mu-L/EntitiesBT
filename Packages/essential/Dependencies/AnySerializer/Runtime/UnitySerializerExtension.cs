using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace AnySerializer
{
    public static class UnitySerializerExtension
    {
        public static bool IsUnitySerializableField([NotNull] this FieldInfo fieldInfo)
        {
            if (fieldInfo.IsStatic) return false;
            if (fieldInfo.IsInitOnly) return false;
            if (fieldInfo.IsNotSerialized) return false;
            if (fieldInfo.IsConst()) return false;
            if (!fieldInfo.IsPublic && fieldInfo.GetCustomAttribute<SerializeField>() == null) return false;
            if (!fieldInfo.FieldType.IsUnitySerializableType()) return false;
            return true;
        }

        public static bool IsUnitySerializableProperty([NotNull] this PropertyInfo propertyInfo)
        {
            return propertyInfo.CanRead && propertyInfo.CanWrite && propertyInfo.GetCustomAttribute<SerializeField>() != null;
        }

        public static bool IsConst([NotNull] this FieldInfo fieldInfo)
        {
            return fieldInfo.GetRequiredCustomModifiers().Any(type => type == typeof(IsConst));
        }

        public static bool IsUnitySerializableType([NotNull] this Type type)
        {
            if (type.IsUnitySimpleFieldType()) return true;
            if (type.IsArray) return true;
            if (type == typeof(List<>) && IsUnitySimpleFieldType(type.GenericTypeArguments[0])) return true;
            return false;
        }

        public static bool IsUnitySimpleFieldType([NotNull] this Type type)
        {
            if (type.IsPrimitive) return true;
            if (type.IsEnum) return true;
            if (type == typeof(Vector2)) return true;
            if (type == typeof(Vector3)) return true;
            if (type == typeof(Vector4)) return true;
            if (type == typeof(Rect)) return true;
            if (type == typeof(Quaternion)) return true;
            if (type == typeof(Matrix4x4)) return true;
            if (type == typeof(Color)) return true;
            if (type == typeof(Color32)) return true;
            if (type == typeof(LayerMask)) return true;
            if (type == typeof(AnimationCurve)) return true;
            if (type == typeof(Gradient)) return true;
            if (type == typeof(RectOffset)) return true;
            if (type == typeof(GUIStyle)) return true;
            if (type.IsValueType && type.GetCustomAttribute<SerializableAttribute>() != null) return true;
            if (type.IsClass && !type.IsAbstract && !type.IsGenericType && type.GetCustomAttribute<SerializableAttribute>() != null) return true;
            if (typeof(UnityEngine.Object).IsAssignableFrom(type)) return true;
            return false;
        }
    }
}