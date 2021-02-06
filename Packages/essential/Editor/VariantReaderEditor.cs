using EntitiesBT.Variant;
using UnityEditor;

namespace EntitiesBT.Editor
{
    [CustomEditor(typeof(IVariantReader<>), false)]
    public class VariantReaderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Slider(1, 0, 100);
            serializedObject.ApplyModifiedProperties();
        }
    }
}