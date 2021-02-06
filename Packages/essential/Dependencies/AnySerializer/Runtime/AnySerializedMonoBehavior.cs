using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using EntitiesBT.Attributes;
using UnityEngine;

namespace AnySerializer
{
    public class AnySerializedMonoBehavior : MonoBehaviour, ISerializationCallbackReceiver
    {
        [SerializeReference, SerializeReferenceButton] private IFormatter _formatter = new BinaryFormatter();
        [SerializeField] private AnySerializeFieldData[] _anySerializedDataArray;

        public void OnBeforeSerialize()
        {
            _anySerializedDataArray = _formatter.SerializeAny(this).ToArray();
        }

        public void OnAfterDeserialize()
        {
            _formatter.DeserializeAny(this, _anySerializedDataArray);
        }
    }
}