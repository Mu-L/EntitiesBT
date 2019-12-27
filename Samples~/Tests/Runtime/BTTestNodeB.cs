using EntitiesBT.Core;
using EntitiesBT.Editor;
using UnityEngine;

namespace EntitiesBT.Test
{
    public static class NodeB
    {
        public static int Id = 101;

        static NodeB()
        {
            VirtualMachine.Register(Id, Reset, Tick);
        }
        
        public struct Data : INodeData
        {
            public int B;
            public int BB;
        }

        static void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
            Debug.Log($"[B]: reset {index}");
        }

        static NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            var data = blob.GetNodeData<Data>(index);
            var state = data.B == data.BB ? NodeState.Success : NodeState.Failure;
            Debug.Log($"[B]: tick {index} {state}");
            return state;
        }
    }
    
    public class BTTestNodeB : BTNode<NodeB.Data>
    {
        public int B;
        public int BB;

        public override int NodeId => NodeB.Id;

        public override unsafe void Build(void* dataPtr)
        {
            var ptr = (NodeB.Data*) dataPtr;
            ptr->B = B;
            ptr->BB = BB;
        }
    }
}
