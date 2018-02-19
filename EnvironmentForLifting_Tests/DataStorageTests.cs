using System;
using EnvironmentForLifting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EnvironmentForLifting_Tests
{
    [TestClass]
    public class DataStorageTests
    {
        [TestMethod]
        public void DataStorage_TryGetFromNonExistingIndex_Default()
        {
            IDataStorage<double> storage = new DataStorageList<double>();
            var result = storage[0];
            Assert.AreEqual(result, default(double));
        }
        [TestMethod]
        public void DataStorage_TryGetFromExistingIndex_Data()
        {
            IDataStorage<double> storage = new DataStorageList<double>();
            var data = 5.0;
            storage[0] = data;
            Assert.AreEqual(data, storage[0]);
        }
        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void DataStorage_GetDroppedData_IndexOutOfRangeException()
        {
            IDataStorage<double> storage = new DataStorageList<double>();
            var data = 5.0;
            storage[0] = data;
            Assert.AreEqual(data, storage[0]);
            storage.DropDataTo(0);
            var errorData = storage[0];
        }
        [TestMethod]
        public void DataStorage_AddDataToNonContinuouseSeqention_Works()
        {
            IDataStorage<int> storage = new DataStorageList<int>();
            storage[5] = 5;
            storage[2] = 2;
            storage[20] = 20;
            Assert.AreEqual(2, storage[2]);
            Assert.AreEqual(5, storage[5]);
            Assert.AreEqual(20, storage[20]);
        }
        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void DataStorage_GetDataFromNegativIndex_IndexOutOfRangeException()
        {
            IDataStorage<double> storage = new DataStorageList<double>();
            var data = storage[-5];
        }
    }
}
