using System;
using EntitiesBT.Core;
using Unity.Entities;
using UnityEngine.Scripting;
using static EntitiesBT.Core.Utilities;

namespace EntitiesBT.Variant
{
    public interface IVariant
    {
        // return: pointer of meta data
        unsafe void* Allocate(
            ref BlobBuilder builder
          , ref BlobVariant blobVariant
          , INodeDataBuilder self
          , ITreeNode<INodeDataBuilder>[] tree
        );
    }

    public interface IVariantReader<T> : IVariant where T : unmanaged {}
    public interface IVariantWriter<T> : IVariant where T : unmanaged {}
    public interface IVariantReaderAndWriter<T> : IVariant where T : unmanaged {}

    public static class DefaultVariant
    {
        public class Any : IVariant
        {
            public unsafe void* Allocate(ref BlobBuilder builder, ref BlobVariant blobVariant, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
            {
                blobVariant.VariantId = GuidHashCode(GUID);
                return builder.Allocate(ref blobVariant, 0);
            }
        }

        public class Writer<T> : Any, IVariantWriter<T> where T : unmanaged {}
        public class Reader<T> : Any, IVariantReader<T> where T : unmanaged {}
        public class ReaderAndWriter<T> : Any, IVariantReaderAndWriter<T> where T : unmanaged {}

        public const string GUID = "DA59FBA5-A829-45D4-A14D-FB2977F6A5AB";

        [Preserve, ReaderMethod(GUID)]
        private static T Read<T, TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where T : unmanaged
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            return default;
        }

        [Preserve, WriterMethod(GUID)]
        private static void Write<T, TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb, T value)
            where T : unmanaged
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
        }
    }
}
