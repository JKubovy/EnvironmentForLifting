using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace EnvironmentForLifting
{
    /// <summary>
    /// Class that is placed between nodes to store results and enable to multiple nodes to ask for data from one node
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    internal class DataStream<T>
    {
        private IDataStorage<Data<T>> dataBuffer;
        private INode<T> belongsTo;
        protected ConcurrentDictionary<INode<T>, int> nodesMaxReadedDataNumber;
        private bool isEnabledDropData;
        private bool isEnabledDataStore;
        private int dataLength;
        private int oldDataKeepCount;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="belongsTo">Instance of Node after whome is conected</param>
        public DataStream(INode<T> belongsTo)
        {
            dataBuffer = new DataStorageList<Data<T>>();
            this.belongsTo = belongsTo;
            nodesMaxReadedDataNumber = new ConcurrentDictionary<INode<T>, int>();
            dataLength = 1;
            oldDataKeepCount = 1;
            isEnabledDropData = true;
            isEnabledDataStore = true;
        }
        /// <summary>
        /// Get some data synchronously from previose node 
        /// </summary>
        /// <param name="node">Node that is asking for data</param>
        /// <param name="dataIndex">Index of data</param>
        /// <returns>Instance of data with given index</returns>
        public Data<T> GetData(INode<T> node, int dataIndex)
        {
            UpdateLastReadedDataNumber(node, dataIndex);
            if (dataIndex < 0)
            {
                return belongsTo.GetData(dataIndex);
            }
            if (!isEnabledDataStore) return belongsTo.GetData(dataIndex);
            Data<T> result = dataBuffer[dataIndex];
            if (result == default(Data<T>)) // Data has not been received yet
            {
                result = belongsTo.GetData(dataIndex);
                dataBuffer[dataIndex] = result;

            }
            DropOldData();
            return result;
        }
        /// <summary>
        /// Register new data receiver to this DataStream
        /// </summary>
        /// <param name="node">Node that want to be registered</param>
        internal void AddDataReceiver(INode<T> node)
        {
            nodesMaxReadedDataNumber.TryAdd(node, -1);
        }
        /// <summary>
        /// Clear DataStream and reset counters
        /// </summary>
        public void Reset()
        {
            dataBuffer.Clear();
            foreach (var key in nodesMaxReadedDataNumber.Keys.ToArray())
            {
                nodesMaxReadedDataNumber[key] = -1;
            }
            belongsTo.Reset();
        }
        private void UpdateLastReadedDataNumber(INode<T> node, int dataNumber)
        {
            int oldValue, newValue;
            do
            {
                oldValue = nodesMaxReadedDataNumber[node];
                newValue = Math.Max(oldValue, dataNumber);
            } while (!nodesMaxReadedDataNumber.TryUpdate(node, newValue, oldValue));
        }
        internal void SetDataLength(int dataLength)
        {
            if (dataLength <= 0)
            {
                Logger.Error("Data's length can't be smaller then 1");
                throw new ArgumentOutOfRangeException();   // Data's length can't be smaller then 1
            }
            this.dataLength = dataLength;
            belongsTo.DataLength = dataLength;
        }
        internal void SetOldDataKeepCount(int oldDataKeepCount)
        {
            if (oldDataKeepCount < 0)
            {
                Logger.Error("Old data count can't be smaller then 0");
                throw new ArgumentOutOfRangeException();   // Old data count can't be smaller then 0
            }
            this.oldDataKeepCount = oldDataKeepCount;
            belongsTo.OldDataKeepCount = oldDataKeepCount;
        }
        private void DropOldData()
        {
            if (!isEnabledDropData) return;
            int min = int.MaxValue;
            foreach (var item in nodesMaxReadedDataNumber.Values)
            {
                min = Math.Min(min, item);
            }
            dataBuffer.DropDataTo(min - oldDataKeepCount);
        }
        private void DropOldDataAsync()
        {
            if (!isEnabledDropData) return;
            int min = int.MaxValue;
            foreach (var item in nodesMaxReadedDataNumber.Values)
            {
                min = Math.Min(min, item);
            }
        }
        internal void SetDropOldData(bool value)
        {
            isEnabledDropData = value;
            belongsTo.DropOldData = value;
        }
        internal void SetParameters(ParameterStorage parameterStorage)
        {
            belongsTo.Parameters = parameterStorage;
        }

        internal void EnableDropOldData() => isEnabledDropData = true;
        internal void DisableDataStore() => isEnabledDataStore = false;
        internal void EnableDataStore() => isEnabledDataStore = true;
    }
    /// <summary>
    /// Envelope of InputStream
    /// Used to make easier to node to ask some data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InputStreamsEnvelope<T>
    {
        private INode<T> node;
        internal DataStream<T> dataStream;
        internal InputStreamsEnvelope(INode<T> node, DataStream<T> dataStream)
        {
            this.node = node;
            this.dataStream = dataStream;
            this.dataStream.AddDataReceiver(node);
        }
        public Data<T> GetData(int dataNumber)
        {
            return dataStream.GetData(node, dataNumber);
        }
        public void Reset()
        {
            dataStream.Reset();
        }
        public void SetOldDataKeepCount(int oldDataKeepCount)
        {
            dataStream.SetOldDataKeepCount(oldDataKeepCount);
        }
        public void SetDropOldData(bool value)
        {
            dataStream.SetDropOldData(value);
        }
        public void DisableDataStore()
        {
            dataStream.DisableDataStore();
        }
        public void SetDataLength(int dataLength)
        {
            dataStream.SetDataLength(dataLength);
        }
        internal void SetParameters(ParameterStorage parameterStorage)
        {
            dataStream.SetParameters(parameterStorage);
        }
    }
}
