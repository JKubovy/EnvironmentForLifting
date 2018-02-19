using System;
using System.Threading.Tasks;

namespace EnvironmentForLifting
{
    /// <summary>
    /// Base class of all nodes.
    /// Provieds implementation of all necessary methods and functions and also provides empty methods whitch should be override 
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public abstract class Node<T> : INode<T>
    {
        /// <summary>
        /// Output DataStream
        /// </summary>
        internal DataStream<T> outputStream;
        /// <summary>
        /// Input envelope of DataStream
        /// </summary>
        internal InputStreamsEnvelope<T>[] inputStreams;
        private Node()
        {
            outputStream = new DataStream<T>(this);
            inputStreams = new InputStreamsEnvelope<T>[0];
        }
        /// <summary>
        /// Constructor that connect predecessor nodes
        /// </summary>
        /// <param name="predecessors">Predecessor nodes</param>
        public Node(params Node<T>[] predecessors) : this()
        {
            int length = predecessors.Length;
            inputStreams = new InputStreamsEnvelope<T>[length];
            for (int i = 0; i < predecessors.Length; i++)
            {
                var predecessorNode = predecessors[i];
                if (predecessorNode is Segment<T>) predecessorNode = predecessorNode[0];
                predecessorNode = predecessorNode[0];
                var outputStream = predecessorNode.OutputStream;
                inputStreams[i] = new InputStreamsEnvelope<T>(this, outputStream);
            }
        }
        /// <summary>
        /// Get node's output
        /// </summary>
        /// <param name="index">Index of output</param>
        /// <returns>Output node</returns>
        public virtual Node<T> this[int index]
        {
            get
            {
                if (index != 0) throw new IndexOutOfRangeException();
                return this;
            }
        }
        /// <summary>
        /// Output DataStream of this node
        /// </summary>
        internal DataStream<T> OutputStream => outputStream;
        /// <summary>
        /// Array of input DataStream
        /// </summary>
        public InputStreamsEnvelope<T>[] Input => inputStreams;
        /// <summary>
        /// Synchronous function to get instance of Data with given index
        /// </summary>
        /// <param name="dataIndex">Index of wanted data</param>
        /// <returns>Instance of Data with given index</returns>
        public virtual Data<T> GetData(int dataIndex)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Method define whole network synchornous computation
        /// Used on last node in network
        /// </summary>
        public virtual void Process()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Method define whole network asynchornous computation
        /// Used on last node in network
        /// </summary>
        public virtual void ProcessAsync()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Calls Reset() on all InputStreams
        /// </summary>
        public virtual void Reset()
        {
            if (inputStreams == null) return;
            foreach (var inputStream in inputStreams)
            {
                inputStream.Reset();
            }
        }
        /// <summary>
        /// Calls SetDataLengthForThisNode(value) and call SetDataLength(value) on all InputNodes
        /// </summary>
        public int DataLength
        {
            set
            {
                this.SetDataLengthForThisNode(value);
                foreach (var inputStream in inputStreams)
                {
                    inputStream.SetDataLength(value);
                }
            }
        }
        /// <summary>
        /// Method used to set behavior when length of data signal is changed
        /// </summary>
        /// <param name="dataLength"></param>
        public virtual void SetDataLengthForThisNode(int dataLength) { }
        /// <summary>
        /// Calls SetOldDataKeepCountForThisNode(value) and call SetOldDataKeepCount(value) on all InputNodes
        /// </summary>
        public int OldDataKeepCount
        {
            set
            {
                this.SetOldDataKeepCountForThisNode(value);
                foreach (var inputStream in inputStreams)
                {
                    inputStream.SetOldDataKeepCount(value);
                }
            }
        }
        /// <summary>
        /// Method used to set behavior when count of old data keep is changed
        /// </summary>
        /// <param name="oldDataKeepCount"></param>
        public virtual void SetOldDataKeepCountForThisNode(int oldDataKeepCount) { }
        /// <summary>
        /// Calls SetDropOldDataForThisNode(value) and call SetDropOldData(value) on all InputNodes
        /// </summary>
        public bool DropOldData
        {
            set
            {
                this.SetDropOldDataForThisNode(value);
                foreach (var inputStream in inputStreams)
                {
                    inputStream.SetDropOldData(value);
                }
            }
        }
        /// <summary>
        /// Method used to set behavior when varible DropOldData is changed 
        /// </summary>
        /// <param name="value"></param>
        public virtual void SetDropOldDataForThisNode(bool value) { }
        /// <summary>
        /// Calls SetParametersForThisNode(value) and call SetParameters(value) on all InputNodes
        /// </summary>
        public ParameterStorage Parameters
        {
            set
            {
                this.SetParametersForThisNode(value);
                foreach (var inputStream in inputStreams)
                {
                    inputStream.SetParameters(value);
                }
            }
        }
        /// <summary>
        /// Method used to set behavior when parameters are set
        /// </summary>
        /// <param name="parameterStorage"></param>
        public virtual void SetParametersForThisNode(ParameterStorage parameterStorage) { }
    }
}
