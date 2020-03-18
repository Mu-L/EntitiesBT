# EntitiesBT

[![openupm](https://img.shields.io/npm/v/entities-bt?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/entities-bt/)

> **Table of contents**
>   * [Why another Behavior Tree framework?](#why-another-behavior-tree-framework)
>   * [Features](#features)
>   * [Disadvantages](#disadvantages)
>   * [HowTo](#howto)
>     - [Install](#install)
>     - [Usage](#usage)
>       - [Create behavior tree](#create-behavior-tree)
>       - [Attach behavior tree onto Entity](#attach-behavior-tree-onto-entity)
>       - [Serialization](#serialization)
>       - [Thread control](#thread-control)
>       - [Variable Property](#variable-property)
>     - [Debug](#debug)
>     - [Custom behavior node](#custom-behavior-node)
>       - [Action](#action)
>       - [Decorator](#decorator)
>       - [Composite](#composite)
>       - [Advanced: customize debug view](#advanced-customize-debug-view)
>       - [Advanced: access other node data](#advanced-access-other-node-data)
>       - [Advanced: behavior tree component](#advanced-behavior-tree-component)
>       - [Advanced: virtual node builder](#advanced-virtual-node-builder)
>   * [Data Structure](#data-structure)

Behavior Tree framework based on and used for Unity Entities (DOTS)

## Why another Behavior Tree framework?
While developing my new game by using Unity Entities, I found that the existing BT frameworks are not support Entities out of box and also lack of compatibility with plugins like [odin](https://odininspector.com/), so I decide to write my own.

## Features
- Actions are easy to read/write data from/to entity.
- Use Component of Unity directly instead of own editor window to maximize compatibility of other plugins.
- Data-oriented design, save all nodes data into a continuous data blob ([NodeBlob.cs](Runtime/Entities/NodeBlob.cs))
- Node has no internal states.
- Separate runtime nodes and editor nodes.
- Easy to extend.
- Also compatible with Unity GameObject without any entity.
- Able to serialize behavior tree into binary file.
- Flexible thread control: force on main thread, force on job thread, controlled by behavior tree.
- Runtime debug window to show the states of nodes.

## Disadvantages
- Incompatible with burst (Won't support this in the foreseen future)
- Lack of action nodes. (Will add some actions as extension if I personally need them)
- Not easy to modify tree structure at runtime.

## HowTo
### Installation
Requirement: Unity >= 2020.1 and entities package >= 0.4.0-preview.10

Install the package either by

[UMP](https://docs.unity3d.com/Manual/upm-ui-giturl.html): `https://github.com/quabug/EntitiesBT.git` 

or 

[OpenUMP](https://openupm.com/docs/getting-started.html#installing-an-upm-package): `openupm add entities-bt`

### Usage
#### Create behavior tree
<img width="600" alt="create" src="https://user-images.githubusercontent.com/683655/72404172-5b4f5c00-378f-11ea-94a1-bb8aa5eb2608.gif" />

#### Attach behavior tree onto _Entity_
<img width="600" alt="attach" src="https://user-images.githubusercontent.com/683655/72404398-27c10180-3790-11ea-82e3-0a973369ab0f.gif" />

#### Serialization
<img width="600" alt="save-to-file" src="https://user-images.githubusercontent.com/683655/72407209-b7b77900-3799-11ea-9de3-0703b1936f63.gif" />

#### Thread control
<img width="400" alt="thread-control" src="https://user-images.githubusercontent.com/683655/72407274-ee8d8f00-3799-11ea-9847-76ad6fdc5a37.png" />

- Force Run on Main Thread: running on main thread only, will not use job to tick behavior tree. Safe to call `UnityEngine` method.
- Force Run on Job: running on job threads only, will not use main thread to tick behavior tree. Not safe to call `UnityEngine` method.
- Controlled by Behavior Tree: Running on job threads by default, but will switch to main thread once meet decorator of [`RunOnMainThread`](Runtime/Nodes/RunOnMainThreadNode.cs)
<img width="300" alt="" src="https://user-images.githubusercontent.com/683655/72407836-cdc63900-379b-11ea-8979-605e725ab0f7.png" />

#### Variable Property
Fetch data from different sources.
- [`CustomVariableProperty`](Runtime/Variable/CustomVariableProperty.cs): regular variable, custom value will save into `NodeData`.
<img width="600" alt="" src="https://user-images.githubusercontent.com/683655/76950074-7764aa80-6944-11ea-9ea7-0697ad5a6da5.gif">

- [`ComponentVariableProperty`](Runtime/Components/ComponentVariableProperty.cs): fetch data from `Component` on `Entity`
<img width="600" alt="" src="https://user-images.githubusercontent.com/683655/76950083-79c70480-6944-11ea-9b42-558bc2429f74.gif">

  - _Component Value Name_: which value should be access from component
  - _Access Mode_: will add chosen query type for behavior tree ([Entity Query](https://docs.unity3d.com/Packages/com.unity.entities@0.1/manual/ecs_entity_query.html))
    - _ReadOnly_: add `ComponentType.ReadOnly` of _Component_ into queries of behavior tree
    - _ReadWrite_: add `ComponentType.ReadWrite` of _Component_ into queries of behavior tree
    - _Optional_: nothing will be added into queries of behavior tree
- [`NodeVariableProperty`](Runtime/Components/NodeVariableProperty.cs): fetch data from blob of another node
<img width="600" alt="" src="https://user-images.githubusercontent.com/683655/76950091-7cc1f500-6944-11ea-994b-5307f08169a2.gif">

  - _Node Object_: another node should be access by this variable, must be in the same behavior tree as the node of variable property.
  - _Value Field Name_: the name of data field in another node.
  - _Access Runtime Data_:
    - false: will copy data to local blob node while building, value change of _Node Object_ won't effect variable once build.
    - true: will access data field of _Node Object_ at runtime, something like reference value of _Node Object_.
- [`ScriptableObjectVariableProperty`](Runtime/Components/ScriptableObjectVariableProperty.cs): fetch data from field of `ScriptableObject`.
<img width="600" alt="" src="https://user-images.githubusercontent.com/683655/76950097-7df32200-6944-11ea-8902-650987d58827.gif">

  - _Scriptable Object_: target SO.
  - _Scriptable Object Value_: target field.
``` c#
    public class BTVariableNode : BTNode<VariableNode>
    {
        [SerializeReference, SerializeReferenceButton] // neccessary for editor
        public Int32Property IntVariable; // an `int` variable property

        protected override void Build(ref VariableNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            // save `Int32Property` as `BlobVariable<int>` of `VariableNode`
            IntVariable.Allocate(ref builder, ref data.Variable, this, tree);
        }
    }
    
    [BehaviorNode("867BFC14-4293-4D4E-B3F0-280AD4BAA403")]
    public struct VariableNode : INodeData
    {
        public BlobVariable<int> IntBlobVariable;

        public static IEnumerable<ComponentType> AccessTypes(int index, INodeBlob blob)
        {
            ref var data = ref blob.GetNodeDefaultData<VariableNode>(index);
            return data.IntBlobVariable.ComponentAccessList; // will return right access type of node, see `NodeVariableProperty` below.
        }

        public static NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            ref var data = ref blob.GetNodeData<VariableNode>(index);
            var intVariable = data.IntBlobVariable.GetData(index, blob, blackboard); // get variable value
            ref var intVariable = ref data.IntBlobVariable.GetDataRef(index, blob, blackboard); // get variable ref value
            return NodeState.Success;
        }
    }
```
##### Generate specific types of `VariableProperty<T>`
Because generic type of `VariableProperty<T>` cannot be serialized in Unity, since `[SerializeReference]` is not allowed for a generic type.
A specific type of `VariableProperty<T>` must be declared before use.
- First create a _Scriptable Object_ of _VariableGeneratorSetting_
<img width="800" alt="Snipaste_2020-03-18_18-57-30" src="https://user-images.githubusercontent.com/683655/76953861-6159e880-694a-11ea-8fbc-33a83b181ebf.png">

- Fill which _Types_ you want to use as variable property.
- Fill _Filename_, _Namespace_, etc.
- Create script from this setting and save it in _Assets_
<img width="600" alt="Snipaste_2020-03-18_18-57-36" src="https://user-images.githubusercontent.com/683655/76953872-63bc4280-694a-11ea-8f03-73af3fa2fec2.png">

- And now you are free to use specific type properties, like `float2Property` etc.


### Debug
<img width="600" alt="debug" src="https://user-images.githubusercontent.com/683655/72407368-517f2600-379a-11ea-8aa9-c72754abce9f.gif" />

### Custom behavior node

#### Action
``` c#
// most important part of node, actual logic on runtime.
[Serializable] // for debug view only
[BehaviorNode("F5C2EE7E-690A-4B5C-9489-FB362C949192")] // must add this attribute to indicate a class is a `BehaviorNode`
public struct EntityMoveNode : INodeData
{
    public float3 Velocity; // node data saved in `INodeBlob`
    
    // declare access of each component data.
    public static readonly ComponentType[] Types = {
        ComponentType.ReadWrite<Translation>()
      , ComponentType.ReadOnly<BehaviorTreeTickDeltaTime>()
    };

    // access and modify node data
    public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
    {
        ref var data = ref blob.GetNodeData<EntityMoveNode>(index);
        ref var translation = ref bb.GetDataRef<Translation>(); // get blackboard data by ref (read/write)
        var deltaTime = bb.GetData<BehaviorTreeTickDeltaTime>(); // get blackboard data by value (readonly)
        translation.Value += data.Velocity * deltaTime.Value;
        return NodeState.Running;
    }
}

// builder and editor part of node
public class EntityMove : BTNode<EntityMoveNode>
{
    public Vector3 Velocity;

    protected override void Build(ref EntityMoveNode data, BlobBuilder _, ITreeNode<INodeDataBuilder>[] __)
    {
        // set `NodeData` here
        data.Velocity = Velocity;
    }
}

// debug view (optional)
public class EntityMoveDebugView : BTDebugView<EntityMoveNode> {}
```

#### Decorator
``` c#
// runtime behavior
[Serializable] // for debug view only
[BehaviorNode("A13666BD-48E3-414A-BD13-5C696F2EA87E", BehaviorNodeType.Decorate/*decorator must explicit declared*/)]
public struct RepeatForeverNode : INodeData
{
    public NodeState BreakStates;
    
    public static NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
    {
        // short-cut to tick first only children
        var childState = blob.TickChildren(index, blackboard).FirstOrDefault();
        if (childState == 0) // 0 means no child was ticked
                             // tick a already completed `Sequence` or `Selector` will return 0
        {
            blob.ResetChildren(index, blackboard);
            childState = blob.TickChildren(index, blackboard).FirstOrDefault();
        }
        ref var data = ref blob.GetNodeData<RepeatForeverNode>(index);
        if (data.BreakStates.HasFlag(childState)) return childState;
        
        return NodeState.Running;
    }
}

// builder and editor
public class BTRepeat : BTNode<RepeatForeverNode>
{
    public NodeState BreakStates;
    
    public override void Build(ref RepeatForeverNode data, BlobBuilder _, ITreeNode<INodeDataBuilder>[] __)
    {
        data.BreakStates = BreakStates;
    }
}

// debug view (optional)
public class BTDebugRepeatForever : BTDebugView<RepeatForeverNode> {}
```

#### Composite
``` c#
// runtime behavior
[StructLayout(LayoutKind.Explicit)] // sizeof(SelectorNode) == 0
[BehaviorNode("BD4C1D8F-BA8E-4D74-9039-7D1E6010B058", BehaviorNodeType.Composite/*composite must explicit declared*/)]
public struct SelectorNode : INodeData
{
    public static NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
    {
        // tick children and break if child state is running or success.
        return blob.TickChildren(index, blackboard, breakCheck: state => state.IsRunningOrSuccess()).LastOrDefault();
    }
}

// builder and editor
public class BTSelector : BTNode<SelectorNode> {}

// avoid debug view since there's nothing need to be debug for `Selector`
```

#### Advanced: customize debug view
- Behavior Node example: [PrioritySelectorNode.cs](Runtime/Nodes/PrioritySelectorNode.cs)
- Debug View example: [BTDebugPrioritySelector.cs](Runtime/Debug/BTDebugPrioritySelector.cs)

#### Advanced: access other node data
`NodeBlob` store all internal data of behavior tree, and it can be access from any node.
To access specific node data, just store its index and access by `INodeData.GetNodeData<T>(index)`.
- Behavior Node example: [ModifyPriorityNode.cs](Runtime/Nodes/ModifyPriorityNode.cs)
- Editor/Builder example: [BTModifyPriority.cs](Runtime/Components/BTModifyPriority.cs)

#### Advanced: behavior tree component
``` c#
[BehaviorTreeComponent] // mark a component data as `BehaviorTreeComponent`
public struct BehaviorTreeTickDeltaTime : IComponentData
{
    public float Value;
}

[UpdateBefore(typeof(VirtualMachineSystem))]
public class BehaviorTreeDeltaTimeSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref BehaviorTreeTickDeltaTime deltaTime) => deltaTime.Value = Time.DeltaTime);
    }
}
```
The components of behavior will add into `Entity` automatically on the stage of convert `GameObject` to `Entity`, if `AutoAddBehaviorTreeComponents` is enabled.
<img width="600" alt="" src="https://user-images.githubusercontent.com/683655/72411453-d7549e80-37a5-11ea-925a-b3949180dd16.png" />

#### Advanced: virtual node builder
A single builder node is able to product multiple behavior nodes while building.
``` C#
public class BTSequence : BTNode<SequenceNode>
{
    [Tooltip("Enable this will re-evaluate node state from first child until running node instead of skip to running node directly.")]
    [SerializeField] private bool _recursiveResetStatesBeforeTick;

    public override INodeDataBuilder Self => _recursiveResetStatesBeforeTick
        // add `RecursiveResetStateNode` as parent of `this` node
        ? new BTVirtualDecorator<RecursiveResetStateNode>(this)
        : base.Self
    ;
}
```

## Data Structure
``` c#
public struct NodeBlob
{
    // default data (serializable data)
    public BlobArray<int> Types; // type id of behavior node, generated from `Guid` of `BehaviorNodeAttribute`
    public BlobArray<int> EndIndices; // range of node branch must be in [nodeIndex, nodeEndIndex)
    public BlobArray<int> Offsets; // data offset of `DefaultDataBlob` of this node
    public BlobArray<byte> DefaultDataBlob; // nodes data
    
    // runtime only data (only exist on runtime)
    public BlobArray<NodeState> States; // nodes states
    // initialize from `DefaultDataBlob`
    public BlobArray<byte> RuntimeDataBlob; // same as `DefaultNodeData` but only available at runtime and will reset to `DefaultNodeData` once reset.
}
```
<img width="600" alt="data-structure" src="https://user-images.githubusercontent.com/683655/72414832-1edf2880-37ae-11ea-8ef1-146e99d30727.png" />
