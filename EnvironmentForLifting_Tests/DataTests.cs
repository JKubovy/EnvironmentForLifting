using System;
using EnvironmentForLifting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace EnvironmentForLifting_Tests
{
    [TestClass]
    public class DataTests
    {
        Type[] types = new Type[] {
            typeof(byte),
            typeof(sbyte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal) };
        public void DataReturnCorrectValuesHelper<T>()
        {
            var exceptedResults = new dynamic[] { 5, 42, 200 };
            var length = exceptedResults.Length;
            var signal = new T[length];
            for (int i = 0; i < length; i++)
            {
                signal[i] = (T)exceptedResults[i];
            }
            var data = new Data<T>(signal);
            Assert.AreEqual(length, data.Length);
            for (int index = 0; index < length; index++)
            {
                Assert.AreEqual((T)exceptedResults[index], data[index]);
            }
        }
        [TestMethod]
        public void Data_ReturnCorrectValues()
        {
            foreach (Type type in types)
            {
                MethodInfo method = typeof(DataTests).GetMethod("DataReturnCorrectValuesHelper").MakeGenericMethod(new Type[] { type });
                method.Invoke(this, null);
            }
        }
        public void DataForeachWorksHelper<T>()
        {
            var exceptedResults = new dynamic[] { 5, 42, 200 };
            var length = exceptedResults.Length;
            var signal = new T[length];
            for (int i = 0; i < length; i++)
            {
                signal[i] = (T)exceptedResults[i];
            }
            var data = new Data<T>(signal);
            Assert.AreEqual(length, data.Length);
            var index = 0;
            foreach (var item in data)
            {
                Assert.AreEqual((T)exceptedResults[index++], item);
            }
        }
        [TestMethod]
        public void Data_Foreach_Works()
        {
            foreach (Type type in types)
            {
                MethodInfo method = typeof(DataTests).GetMethod("DataForeachWorksHelper").MakeGenericMethod(new Type[] { type });
                method.Invoke(this, null);
            }
        }
        private void DataNegateWorksTestHelper<T>(Data<T> data, dynamic[] exceptedResults)
        {
            var result = -data;
            Assert.AreEqual(data.Length, result.Length);
            for (int i = 0; i < exceptedResults.Length; i++)
            {
                Assert.AreEqual((T)exceptedResults[i], result[i]);
            }
        }
        private void DataAddWorksTestHelper<T>(Data<T> data1, Data<T> data2, dynamic[] exceptedResults)
        {
            var result = data1 + data2;
            Assert.AreEqual(result.Length, data1.Length);
            for (int i = 0; i < exceptedResults.Length; i++)
            {
                Assert.AreEqual((T)exceptedResults[i], result[i]);
            }
        }
        private void DataSubtractWorksTestHelper<T>(Data<T> data1, Data<T> data2, dynamic[] exceptedResults)
        {
            var result = data1 - data2;
            Assert.AreEqual(result.Length, data1.Length);
            for (int i = 0; i < exceptedResults.Length; i++)
            {
                Assert.AreEqual((T)exceptedResults[i], result[i]);
            }
        }
        private void DataDivideWorksTestHelper<T>(Data<T> data, int number, dynamic[] exceptedResults)
        {
            var result = data / number;
            Assert.AreEqual(result.Length, data.Length);
            for (int i = 0; i < exceptedResults.Length; i++)
            {
                Assert.AreEqual((T)exceptedResults[i], result[i]);
            }
        }
        private void DataMultiplyWorksTestHelper<T>(Data<T> data, int number, dynamic[] exceptedResults)
        {
            var result = data * number;
            Assert.AreEqual(result.Length, data.Length);
            for (int i = 0; i < exceptedResults.Length; i++)
            {
                Assert.AreEqual((T)exceptedResults[i], result[i]);
            }
            result = number * data;
            Assert.AreEqual(result.Length, data.Length);
            for (int i = 0; i < exceptedResults.Length; i++)
            {
                Assert.AreEqual((T)exceptedResults[i], result[i]);
            }
        }
        public void DataOperationWorksTestHelper<T>()
        {
            var data1 = new Data<T>(new T[] { (T)((dynamic)2), (T)((dynamic)4) });
            var data2 = new Data<T>(new T[] { (T)((dynamic)1), (T)((dynamic)2) });
            DataNegateWorksTestHelper(data1, new dynamic[] { -2, -4 });
            DataAddWorksTestHelper(data1, data2, new dynamic[] { 3, 6 });
            DataSubtractWorksTestHelper(data1, data2, new dynamic[] { 1, 2 });
            DataDivideWorksTestHelper(data1, 2, new dynamic[] { 1, 2 });
            DataMultiplyWorksTestHelper(data2, 4, new dynamic[] { 4, 8 });
        }
        [TestMethod]
        public void Data_AllMathOperations_Works()
        {
            foreach (Type type in types)
            {
                MethodInfo method = typeof(DataTests).GetMethod("DataOperationWorksTestHelper").MakeGenericMethod(new Type[] { type });
                method.Invoke(this, null);
            }
        }
        [TestMethod]
        public void Data_Equels_Works()
        {
            var data1 = new Data<int>(new[] { 4, 4 });
            var data2 = new Data<int>(new[] { 4, 2 });
            var data3 = new Data<int>(new[] { 4, 4 });
            Assert.IsFalse(data1.Equals(data2));
            Assert.IsFalse(data2.Equals(data1));
            Assert.IsTrue(data1.Equals(data3));
            Assert.IsTrue(data3.Equals(data1));
        }
        public void DataZeroHelper<T>()
        {
            var signal = new T[] { default(T), default(T), default(T) };
            var length = signal.Length;
            var data = new ZeroData<T>(length);
            Assert.AreEqual(length, data.Length);
            for (int index = 0; index < length; index++)
            {
                Assert.AreEqual(signal[index], data[index]);
            }
        }
        [TestMethod]
        public void DataZero_Works()
        {
            foreach (Type type in types)
            {
                MethodInfo method = typeof(DataTests).GetMethod("DataZeroHelper").MakeGenericMethod(new Type[] { type });
                method.Invoke(this, null);
            }
        }
    }
}
