using System;
using UnityEngine;

namespace EntitiesBT.Variant
{
    [Serializable]
    public class SerializedReaderAndWriterVariant<T> : ISerializedVariantReaderAndWriter<T> where T : unmanaged
    {
        [field: SerializeField]
        public bool IsLinked { get; private set; } = true;
        public IVariantReaderAndWriter<T> ReaderAndWriter { get; private set; }
        public IVariantReader<T> Reader { get; private set; }
        public IVariantWriter<T> Writer { get; private set; }
    }
}