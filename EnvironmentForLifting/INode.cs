using System;
using System.Threading.Tasks;

namespace EnvironmentForLifting
{
    /// <summary>
    /// Node interface
    /// Every node have to use this interface
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public interface INode<T>
    {
        /// <summary>
        /// Synchronous function to get instance of Data with given index
        /// </summary>
        /// <param name="dataIndex">Index of wanted data</param>
        /// <returns>Instance of Data with given index</returns>
        Data<T> GetData(int dataNumber);
        /// <summary>
        /// Method define whole network synchornous computation
        /// Used on last node in network
        /// </summary>
        void Process();
        /// <summary>
        /// Method define whole network asynchornous computation
        /// Used on last node in network
        /// </summary>
        void ProcessAsync();
        /// <summary>
        /// Reset network and clear all DataStorage
        /// </summary>
        void Reset();
        /// <summary>
        /// Set length of signal in data in whole network
        /// </summary>
        int DataLength { set; }
        /// <summary>
        /// Set how many old data should be stored in DataStorage in whole network
        /// </summary>
        int OldDataKeepCount { set; }
        /// <summary>
        /// Set if results should be stored in DataStorage in whole network
        /// </summary>
        bool DropOldData { set; }
        /// <summary>
        /// Set parameters to node
        /// </summary>
        ParameterStorage Parameters { set; }
        /// <summary>
        /// Get node's output
        /// </summary>
        /// <param name="index">Index of output</param>
        /// <returns>Output node</returns>
        Node<T> this[int index] { get; }
    }
}
