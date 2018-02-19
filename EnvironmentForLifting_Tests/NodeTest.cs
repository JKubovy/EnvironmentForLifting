using System;
using EnvironmentForLifting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EnvironmentForLifting_Tests
{
    [TestClass]
    public class NodeTest
    {
        private class TestNode : Node<int>
        {
            public TestNode(params Node<int>[] predecessors) : base(predecessors) { }
            public override Data<int> GetData(int dataNumber)
            {
                return new Data<int>(new int[] { dataNumber });
            }
            private int resetCalls = 0;
            private int setDataLengthCalls = 0;
            private int setOldDataKeepCountCalls = 0;
            public int ResetCalls => resetCalls;
            public int SetDataLengthCalls => setDataLengthCalls;
            public int SetOldDataKeepCountCalls => setOldDataKeepCountCalls;
            public override void Reset()
            {
                resetCalls++;
                base.Reset();
            }
            public override void SetDataLengthForThisNode(int dataLength)
            {
                setDataLengthCalls++;
            }
            public override void SetOldDataKeepCountForThisNode(int oldDataKeepCount)
            {
                setOldDataKeepCountCalls++;
            }
            public DataStream<int> GetOutputDataStreamTest() => outputStream;
            public InputStreamsEnvelope<int>[] GetInputDataStreamsTest() => inputStreams;
        }
        [TestMethod]
        public void Node_OutputDataStream_IsInicialized()
        {
            var node = new TestNode();
            Assert.IsNotNull(node.GetOutputDataStreamTest());
            Assert.IsInstanceOfType(node.GetOutputDataStreamTest(), typeof(DataStream<int>));
        }
        [TestMethod]
        public void Node_InputDataStream_IsConnected()
        {
            var node1 = new TestNode();
            var node2 = new TestNode(node1);
            Assert.AreEqual(0, node1.GetInputDataStreamsTest().Length);
            Assert.AreEqual(1, node2.GetInputDataStreamsTest().Length);
            Assert.IsInstanceOfType(node2.GetInputDataStreamsTest()[0], typeof(InputStreamsEnvelope<int>));
        }
        [TestMethod]
        public void Node_Reset_CallPropaget()
        {
            var node1 = new TestNode();
            var node2 = new TestNode(node1);
            node2.Reset();
            Assert.AreEqual(1, node1.ResetCalls);
            Assert.AreEqual(1, node2.ResetCalls);
        }
        [TestMethod]
        public void Node_SetDataLength_CallPropaget()
        {
            var node1 = new TestNode();
            var node2 = new TestNode(node1);
            node2.DataLength = 1;
            Assert.AreEqual(1, node1.SetDataLengthCalls);
            Assert.AreEqual(1, node2.SetDataLengthCalls);
        }
        [TestMethod]
        public void Node_SetOldDataKeepCount_CallPropaget()
        {
            var node1 = new TestNode();
            var node2 = new TestNode(node1);
            node2.OldDataKeepCount = 0;
            Assert.AreEqual(1, node1.SetOldDataKeepCountCalls);
            Assert.AreEqual(1, node2.SetOldDataKeepCountCalls);
        }
    }
}
