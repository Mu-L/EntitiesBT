using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EntitiesBT.Core;
using EntitiesBT.Editor;
using NUnit.Framework;
using UnityEngine;

namespace EntitiesBT.Test
{
    public class BehaviorTreeTestBase
    {
        private readonly Dictionary<string, Func<string, BTNode>> _nodeCreators = new Dictionary<string, Func<string, BTNode>>
        {
            { "seq", Create<BTSequence> }
          , { "sel", Create<BTSelector> }
          , { "par", Create<BTParallel> }
          , { "yes", CreateTestNode(NodeState.Success) }
          , { "no", CreateTestNode(NodeState.Failure) }
          , { "run", CreateTestNode(NodeState.Running) }
          , { "a", CreateA }
          , { "b", CreateB }
        };

        protected BehaviorNodeFactory Factory;
        
        [SetUp]
        public void Setup()
        {
            Factory = new BehaviorNodeFactory();
            Factory.RegisterCommonNodes(() => TimeSpan.Zero);
            Factory.Register<TestNode>(() => new TestNode());
            Factory.Register<NodeA>(() => new NodeA());
            Factory.Register<NodeB>(() => new NodeB());
        }

        protected VirtualMachine CreateVM(string tree)
        {
            var root = CreateBTNode(tree).GetComponent<BTNode>();
            var blobRef = root.ToBlob(Factory);
            return new VirtualMachine(new NodeBlobRef(blobRef), Factory);
        }
    
        private static BTNode CreateA(string @params)
        {
            var nodeA = Create<BTTestNodeA>();
            nodeA.A = int.Parse(@params);
            return nodeA;
        }
        
        private static BTNode CreateB(string @params)
        {
            var nodeB = Create<BTTestNodeB>();
            var paramArray = @params.Split(',');
            nodeB.B = int.Parse(paramArray[0].Trim());
            nodeB.BB = int.Parse(paramArray[1].Trim());
            return nodeB;
        }

        private static Func<string, BTNode> CreateTestNode(NodeState state)
        {
            return @params =>
            {
                var testNodeState = Create<BTTestNodeState>();
                testNodeState.State = state;
                return testNodeState;
            };
        }

        private static T Create<T>(string @params = "") where T : BTNode
        {
            var obj = new GameObject(typeof(T).Name);
            var comp = obj.AddComponent<T>();
            return comp;
        }

        // sample 1: "!seq>yes|no|run|a:10";
        // sample 2: @"
        // seq
        //   yes
        //   no
        //   run
        //   a:10
        //   b:1,2
        // ";
        protected GameObject CreateBTNode(string branch)
        {
            if (branch.First() == '!') return ParseSingleLine(branch.Substring(1));

            using (var reader = new StringReader(branch))
                return ParseMultiLines(reader);

            GameObject ParseMultiLines(StringReader reader)
            {
                throw new NotImplementedException();
                // var splits = branch.Split('>');
                // Assert.AreEqual(splits.Length, 2);
                // var parent = Create(splits[0].Trim());
                // foreach (var nodeString in splits[1].Split('|'))
                // {
                //     var child = Create(nodeString.Trim());
                //     child.transform.SetParent(parent.transform, false);
                // }
                // return parent;
            }

            GameObject ParseSingleLine(string branchString)
            {
                var splits = branchString.Split('>');
                Assert.AreEqual(splits.Length, 2);
                var parent = Create(splits[0].Trim());
                foreach (var nodeString in splits[1].Split('|'))
                {
                    var child = Create(nodeString.Trim());
                    child.transform.SetParent(parent.transform, false);
                }
                return parent;
            }

            GameObject Create(string nodeString)
            {
                var nameParamsArray = nodeString.Split(':');
                var name = nameParamsArray[0].Trim();
                var @params = nameParamsArray.Length >= 2 ? nameParamsArray[1].Trim() : "";
                return _nodeCreators[name](@params).gameObject;
            }
        }
    }
}