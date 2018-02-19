using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentForLifting
{
    /// <summary>
    /// Interface to DataStorage
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    interface IDataStorage<T>
    {
        /// <summary>
        /// DataStorage indexer
        /// </summary>
        /// <param name="index">Index of wanted data</param>
        /// <returns>Instance of data with given index</returns>
        T this[int index]
        {
            get;
            set;
        }
        /// <summary>
        /// Clear results stored in DataStorage
        /// </summary>
        void Clear();
        /// <summary>
        /// Clear results to certain data index stored in DataStorage
        /// </summary>
        /// <param name="dataIndex"></param>
        void DropDataTo(int dataIndex);
    }
}
