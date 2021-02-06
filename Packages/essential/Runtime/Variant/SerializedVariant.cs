using System;
using EntitiesBT.Attributes;
using EntitiesBT.Core;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Variant
{
    [Serializable]
    public class SerializedReaderAndWriterVariant<T> : ISerializedVariantReaderAndWriter<T> where T : unmanaged
    {
        public bool IsLinked { get; private set; } = true;
        public IVariantReaderAndWriter<T> ReaderAndWriter { get; private set; }
        public IVariantReader<T> Reader { get; private set; }
        public IVariantWriter<T> Writer { get; private set; }
    }

    [Serializable]
    public class SerializedReaderVariant<T> where T : unmanaged
    {
        public IVariantReader<T> Variant;

        public unsafe void Allocate(
            ref BlobBuilder builder
          , ref BlobVariantReader<T> blobVariant
          , INodeDataBuilder self
          , ITreeNode<INodeDataBuilder>[] tree
        )
        {
            Variant?.Allocate(ref builder, ref blobVariant, self, tree);
        }
    }

    [Serializable]
    public class SerializedWriterVariant<T> where T : unmanaged
    {
        public IVariantWriter<T> Variant;

        public unsafe void Allocate(
            ref BlobBuilder builder
          , ref BlobVariantWriter<T> blobVariant
          , INodeDataBuilder self
          , ITreeNode<INodeDataBuilder>[] tree
        )
        {
            Variant?.Allocate(ref builder, ref blobVariant, self, tree);
        }
    }
}