using System;
using System.Collections.Generic;

namespace EnvironmentForLifting
{
    /// <summary>
    /// Implementation of DataStorage with List
    /// Used to store results in DataStream
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    class DataStorageList<T> : IDataStorage<T>
    {
        private List<T> data;
        private int offset;
        public DataStorageList()
        {
            data = new List<T>();
            offset = -1;
        }
        /// <summary>
        /// DataStorage indexer
        /// </summary>
        /// <param name="index">Index of data</param>
        /// <returns>Instance of data on given index</returns>
        public T this[int index]
        {
            get
            {
                if (index <= offset)
                {
                    Logger.Error($"Trying to get {index} but can ask only for {(offset + 1)} and higher");
                    throw new IndexOutOfRangeException($"Trying to get {index} but can ask only for {(offset + 1)} and higher");
                }
                if (data.Count - 1 >= index)
                    return data[index];
                else
                    return default(T);
            }
            set
            {
                lock (data) // Had to be add to proper work async calculation
                {
                    if (data.Count < index)
                    {
                        while (data.Count != index)
                        {
                            data.Add(default(T));
                        }
                        data.Add(value);
                    }
                    else if (data.Count == index)
                    {
                        data.Add(value);
                    }
                    else
                    {
                        data[index] = value;
                    }
                }
            }
        }
        /// <summary>
        /// Clear DataStorage
        /// </summary>
        public void Clear()
        {
            data.Clear();
            offset = -1;
        }
        /// <summary>
        /// Clear DataStorage to certain index
        /// </summary>
        /// <param name="index">Index of data</param>
        public void DropDataTo(int index)
        {
            for (int i = offset + 1; i < index; i++)
            {
                data[i] = default(T);
            }
            offset = index;
        }
    }
}
