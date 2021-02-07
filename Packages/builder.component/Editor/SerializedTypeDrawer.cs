using System;
using System.Linq;
using EntitiesBT.Attributes.Editor;
using EntitiesBT.Components;
using EntitiesBT.Core;
using UnityEditor;
using UnityEngine;

namespace EntitiesBT.Editor
{
    [CustomPropertyDrawer(typeof(SerializedTypeAttribute))]
    public class SerializedTypeDrawer : PropertyDrawer
    {
        private string[] _options;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                if (_options == null)
                {
                    var baseType = ((SerializedTypeAttribute) attribute).BaseType;
                    var types = Core.Utilities.ALL_ASSEMBLIES.Value
                        .SelectMany(assembly => assembly.GetTypesWithoutException())
                    ;
                    if (baseType != null)
                        types = types.Where(type => baseType.IsInterface ? baseType.IsAssignableFrom(type) : type.IsSubclassOf(type));

                    _options = types.Select(type => type.AssemblyQualifiedName).ToArray();
                }
                property.PopupFunc()(position, label.text, _options);
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }
    }
}
