using System;

namespace EnvironmentForLifting
{
    /// <summary>
    /// Base class of all segments.
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public abstract class Segment<T> : Node<T>
    {
        /// <summary>
        /// Input nodes of segment
        /// Has to be used insted of Input property
        /// </summary>
        public Node<T>[] inputNodes;
        /// <summary>
        /// Output nodes of segment
        /// Has to be set before using
        /// </summary>
        public Node<T>[] outputNodes;
        /// <summary>
        /// Constructor that store input nodes and declare array of output nodes
        /// Calls BuildSegment() before end
        /// </summary>
        /// <param name="outputCount">Number of outputs</param>
        /// <param name="predecessors">Arbitrary count of predecessor nodes</param>
        public Segment(int outputCount, params Node<T>[] predecessors) : base(predecessors)
        {
            inputNodes = predecessors;
            outputNodes = new Node<T>[outputCount];
            BuildSegment();
        }
        /// <summary>
        /// Method that build segment inside nodes and connections
        /// </summary>
        public abstract void BuildSegment();
        /// <summary>
        /// Get segment's output node
        /// </summary>
        /// <param name="index">Index of output node</param>
        /// <returns>Output node with given index</returns>
        public override Node<T> this[int index]
        {
            get
            {
                if (index < 0 || index >= outputNodes.Length) throw new IndexOutOfRangeException();
                return outputNodes[index];
            }
        }
        
    }
}
