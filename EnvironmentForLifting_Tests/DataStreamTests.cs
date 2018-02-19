using System;
using System.Threading.Tasks;
using EnvironmentForLifting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Concurrent;

namespace EnvironmentForLifting_Tests
{
    [TestClass]
    public class DataStreamTests
    {
        private class TestNode : Node<int>
        {
            public TestNode(params Node<int>[] predecessors) : base(predecessors) { }
            private int getDataCalls = 0;
            private int getDataAsyncCalls = 0;
            public int GetDataCalls => getDataCalls;
            public int GetDataAsyncCalls => getDataAsyncCalls;
            public override Data<int> GetData(int dataNumber)
            {
                getDataCalls++;
                return new Data<int>(new int[] { dataNumber });
            }
            public void SetOutputDataStream(DataStream<int> outputDataStream)
            {
                outputStream = outputDataStream;
            }
            public InputStreamsEnvelope<int>[] InputDataStream => inputStreams;        }
        private class TestDataStream : DataStream<int>
        {
            public TestDataStream(INode<int> belongsTo) : base(belongsTo) { }
            public ConcurrentDictionary<INode<int>, int> NodesMaxReadedDataNumber => nodesMaxReadedDataNumber;
        }
        [TestMethod]
        public void DataStream_CallsGetData()
        {
            var node1 = new TestNode();
            var node2 = new TestNode(node1);
            var data = node2.InputDataStream[0].GetData(2);
            Assert.AreEqual(1, node1.GetDataCalls);
            Assert.AreEqual(2, data[0]);
        }
        [TestMethod]
        public void DataStream_SavingData_Works()
        {
            var node1 = new TestNode();
            var node2 = new TestNode(node1);
            Data<int> data = node2.InputDataStream[0].GetData(2);
            for (int i = 0; i < 5; i++)
            {
                data = node2.InputDataStream[0].GetData(2);
            }
            Assert.AreEqual(1, node1.GetDataCalls);
            Assert.AreEqual(2, data[0]);
        }
        [TestMethod]
        public void DataStream_DropDataCount0_Data()
        {
            var node1 = new TestNode();
            var node2 = new TestNode(node1);
            var dataStream = node2.InputDataStream[0];
            node2.OldDataKeepCount = 0;
            Data<int> data;
            for (int i = 0; i < 5; i++)
            {
                data = dataStream.GetData(i);
                Assert.AreEqual(i, data[0]);
            }
        }
        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void DataStream_DropDataCount0_GetSameDataTwice_Exception()
        {
            var node1 = new TestNode();
            var node2 = new TestNode(node1);
            var dataStream = node2.InputDataStream[0];
            node2.OldDataKeepCount = 0;
            Data<int> data = dataStream.GetData(0);
            data = dataStream.GetData(0);
        }
        [TestMethod]
        public void DataStream_DropDataAsync_Works()
        {
        }
    }
}
