using System;
using System.Threading.Tasks;

namespace EnvironmentForLifting
{
    /// <summary>
    /// Node for general computation. Useful for computation node where it is not necessary to remember state
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public class UniversalNode<T> : Node<T>
    {
        /// <summary>
        /// Delegate that calculate synchronously result based on given dataNumber.
        /// </summary>
        /// <param name="input">Array of inputs</param>
        /// <param name="dataNumber">Index of wanted data</param>
        /// <returns></returns>
        public delegate Data<T> GetDataDelegate(InputStreamsEnvelope<T>[] input, int dataNumber);
        private GetDataDelegate getData;
        /// <summary>
        /// Constructor that takes sync delegate
        /// </summary>
        /// <param name="getData">Delegate that calculate synchronously result based on given dataNumber.</param>
        /// <param name="predecessors">Predecessor nodes</param>
        public UniversalNode(GetDataDelegate getData, params Node<T>[] predecessors) : base(predecessors)
        {
            this.getData = getData;
        }
        public override Data<T> GetData(int dataNumber)
        {
            return getData(Input, dataNumber);
        }
    }
    /// <summary>
    /// Node that sum data from n inputs
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public class Add<T> : Node<T>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="predecessors">Arbitrary count of predecessor nodes</param>
        public Add(params Node<T>[] predecessors) : base(predecessors)
        {
            if (predecessors.Length == 0)
            {
                Logger.Error($"Node {typeof(Add<T>)} Can\'t add zero predecessors");
                throw new ArgumentException("Can\'t add zero predecessors");
            }
        }
        public override Data<T> GetData(int dataNumber)
        {
            Data<T> result = Input[0].GetData(dataNumber);
            for (int i = 1; i < Input.Length; i++)
            {
                result += Input[i].GetData(dataNumber);
            }
            return result;
        }
    }
    /// <summary>
    /// Substract node
    /// From data from first input subtruct data from second input
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public class Subtract<T> : Node<T>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="node1">Input node with subtrahend</param>
        /// <param name="node2">Input node with minuend</param>
        public Subtract(Node<T> node1, Node<T> node2) : base(node1, node2) { }
        public override Data<T> GetData(int dataNumber)
        {
            var data1 = Input[0].GetData(dataNumber);
            var data2 = Input[1].GetData(dataNumber);
            return data1 - data2;
        }
    }
    /// <summary>
    /// Merging multiple inputs to one output
    /// In fact it just recalculate dataNumber and ask from right input
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public class Merge<T> : Node<T>
    {
        int predecesorsCount;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="predecessors">Arbitrary number of predecessor nods</param>
        public Merge(params Node<T>[] predecessors) : base(predecessors)
        {
            this.predecesorsCount = predecessors.Length;
        }
        private void GetIndexAndDataNumber(int oldDataNumber, out int newDataNumber, out int index)
        {
            index = oldDataNumber % predecesorsCount;
            if (index < 0) index += predecesorsCount;
            if (oldDataNumber < 0) oldDataNumber -= predecesorsCount;
            newDataNumber = oldDataNumber / predecesorsCount;
        }
        public override Data<T> GetData(int dataNumber)
        {
            GetIndexAndDataNumber(dataNumber, out int newDataNumber, out int index);
            return Input[index].GetData(newDataNumber);
        }
    }
    /// <summary>
    /// Node that return setted data to every dataNumber
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public class Constant<T> : Node<T>
    {
        Data<T> constant;
        bool zeroData;
        /// <summary>
        /// Constructor that set constatnt to zero
        /// </summary>
        public Constant() : base()
        {
            this.constant = new ZeroData<T>();
            this.zeroData = true;
        }
        /// <summary>
        /// Constant setter
        /// </summary>
        /// <param name="signal">Constant signal as array of T</param>
        /// <returns>This node</returns>
        public Constant<T> SetConstant(T[] signal)
        {
            this.constant = new Data<T>(signal);
            this.zeroData = false;
            return this;
        }
        /// <summary>
        /// Constant setter
        /// </summary>
        /// <param name="constant">Constant data</param>
        /// <returns>This node</returns>
        public Constant<T> SetConstant(Data<T> constant)
        {
            this.constant = constant;
            this.zeroData = false;
            return this;
        }
        public override void SetDataLengthForThisNode(int dataLength)
        {
            if (zeroData) constant = new ZeroData<T>(dataLength);
        }
        public override Data<T> GetData(int dataNumber)
        {
            return constant;
        }
    }
}
