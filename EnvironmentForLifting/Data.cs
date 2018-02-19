using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


namespace EnvironmentForLifting
{
    /// <summary>
    /// Envelope for signal
    /// Insatnces of Data is sending in computation network
    /// </summary>
    /// <typeparam name="T">Type od data</typeparam>
    public class Data<T> : IEnumerable<T>
    {
        private T[] _signal;
        protected bool isEmpty;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="signal">Signal in byte array</param>
        public Data(T[] signal)
        {
            _signal = signal;
            isEmpty = false;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="signal">Signal in T array</param>
        public Data(byte[] signal)
        {
            var length = signal.Length;
            _signal = new T[length];
            for (int i = 0; i < length; i++)
            {
                _signal[i] = toT(signal[i]);
            }
            isEmpty = false;
        }
        public bool IsEmpty => isEmpty;
        /// <summary>
        /// Signal indexer
        /// </summary>
        /// <param name="index">Signal index</param>
        /// <returns>Signal on given index</returns>
        public T this[int index] => _signal[index];
        public int Length => _signal.Length;
        /// <summary>
        /// Signal enumerator
        /// </summary>
        /// <returns>Signal enumerator</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_signal).GetEnumerator();
        }
        /// <summary>
        /// Signal enumerator
        /// </summary>
        /// <returns>Signal enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)_signal).GetEnumerator();
        }
        private static ParameterExpression paramT1 = Expression.Parameter(typeof(T));
        private static ParameterExpression paramT2 = Expression.Parameter(typeof(T));
        private static ParameterExpression paramInt = Expression.Parameter(typeof(int));
        private static ParameterExpression paramByte = Expression.Parameter(typeof(byte));
        private static ParameterExpression paramDouble = Expression.Parameter(typeof(double));
        private static Func<T, T, bool> equal = ImprovedExpression<T>.GetEqualFuncCompiled(paramT1, paramT2);
        private static Func<T, T> negate = ImprovedExpression<T>.GetNegateFuncCompiled(paramT1);
        private static Func<T, T, T> add = ImprovedExpression<T>.GetAddFuncCompiled(paramT1, paramT2);
        private static Func<T, T, T> subtract = ImprovedExpression<T>.GetSubtractFuncCompiled(paramT1, paramT2);
        private static Func<T, int, T> divide = ImprovedExpression<T>.GetDivideFuncCompiled(paramT1, paramInt);
        private static Func<T, int, T> multiplyInt = ImprovedExpression<T>.GetMultiplyIntFuncCompiled(paramT1, paramInt);
        private static Func<T, double, T> multiplyDouble = ImprovedExpression<T>.GetMultiplyDoubleFuncCompiled(paramT1, paramDouble);
        private static Func<T, byte> toByte = ImprovedExpression<T>.GetToByteFuncCompiled(paramT1);
        private static Func<T, int> toInt = ImprovedExpression<T>.GetToIntFuncCompiled(paramT1);
        private static Func<T, float> toFloat = ImprovedExpression<T>.GetToFloatFuncCompiled(paramT1);
        private static Func<T, double> toDouble = ImprovedExpression<T>.GetToDoubleFuncCompiled(paramT1);
        private static Func<byte, T> toT = ImprovedExpression<T>.GetByteToTFuncCompiled(paramByte);
        private static Func<float, T> floatToT = ImprovedExpression<T>.GetFloatToTFuncCompiled(Expression.Parameter(typeof(float)));
        private static Func<double, T> doubleToT = ImprovedExpression<T>.GetDoubleToTFuncCompiled(Expression.Parameter(typeof(double)));
        private static T Negate(T a) => negate(a);
        private static T Add(T a, T b) => add(a, b);
        private static T Subtract(T a, T b) => subtract(a, b);
        private static T Divide(T a, int divisor) => divide(a, divisor);
        private static T MultiplyInt(T a, int multiplicator) => multiplyInt(a, multiplicator);
        private static T MultiplyDouble(T a, double multiplicator) => multiplyDouble(a, multiplicator);
        /// <summary>
        /// Compare two signals
        /// </summary>
        /// <param name="obj">Second Data instance</param>
        /// <returns>Is two signal same</returns>
        public override bool Equals(object obj)
        {
            Data<T> data1 = this;
            Data<T> data2 = (Data<T>)obj;
            if (data1.Length != data2.Length) return false;
            for (int i = 0; i < data1.Length; i++)
            {
                if (!equal(data1[i], data2[i])) return false;
            }
            return true;
        }
        public override int GetHashCode() => base.GetHashCode();
        /// <summary>
        /// Negation of data
        /// </summary>
        /// <param name="data">Instance of Data</param>
        /// <returns>Negation of data</returns>
        public static Data<T> operator -(Data<T> data)
        {
            if (data.IsEmpty) return data;
            var length = data.Length;
            var signal = new T[length];
            for (int i = 0; i < length; i++)
            {
                signal[i] = Negate(data[i]);
            }
            return new Data<T>(signal);
        }
        /// <summary>
        /// Additional of two Data instances
        /// </summary>
        /// <param name="data1">First addend</param>
        /// <param name="data2">Second addend</param>
        /// <returns>Sum of two data</returns>
        public static Data<T> operator +(Data<T> data1, Data<T> data2)
        {
            if (data1.IsEmpty) return data1;
            if (data2.IsEmpty) return data2;
            var length = data1.Length;
            var signal = new T[length];
            for (int i = 0; i < length; i++)
            {
                signal[i] = Add(data1[i], data2[i]);
            }
            return new Data<T>(signal);
        }
        /// <summary>
        /// Subtruct of to data
        /// </summary>
        /// <param name="data1">Subtrahend</param>
        /// <param name="data2">Minuend</param>
        /// <returns>Computed data</returns>
        public static Data<T> operator -(Data<T> data1, Data<T> data2)
        {
            if (data1.IsEmpty) return data1;
            if (data2.IsEmpty) return data2;
            var length = data1.Length;
            var signal = new T[length];
            for (int i = 0; i < length; i++)
            {
                signal[i] = Subtract(data1[i], data2[i]);
            }
            return new Data<T>(signal);
        }
        /// <summary>
        /// Division of data by items in siganl
        /// </summary>
        /// <param name="data">Instance of Data</param>
        /// <param name="divisor">Divisor</param>
        /// <returns>Computed data</returns>
        public static Data<T> operator /(Data<T> data, int divisor)
        {
            if (data.IsEmpty) return data;
            var length = data.Length;
            var signal = new T[length];
            for (int i = 0; i < length; i++)
            {
                signal[i] = Divide(data[i], divisor);
            }
            return new Data<T>(signal);
        }
        /// <summary>
        /// Multiplication by items in signal
        /// </summary>
        /// <param name="multiplicator">Multiplicator</param>
        /// <param name="data">Instance of Data</param>
        /// <returns>Computed data</returns>
        public static Data<T> operator *(int multiplicator, Data<T> data) => data * multiplicator;
        /// <summary>
        /// Multiplication by items in signal
        /// </summary>
        /// <param name="multiplicator">Multiplicator</param>
        /// <param name="data">Instance of Data</param>
        /// <returns>Computed data</returns>
        public static Data<T> operator *(Data<T> data, int multiplicator)
        {
            if (data.IsEmpty) return data;
            var length = data.Length;
            var signal = new T[length];
            for (int i = 0; i < length; i++)
            {
                signal[i] = MultiplyInt(data[i], multiplicator);
            }
            return new Data<T>(signal);
        }
        /// <summary>
        /// Multiplication by items in signal
        /// </summary>
        /// <param name="multiplicator">Multiplicator</param>
        /// <param name="data">Instance of Data</param>
        /// <returns>Computed data</returns>
        public static Data<T> operator *(double multiplicator, Data<T> data) => data * multiplicator;
        /// <summary>
        /// Multiplication by items in signal
        /// </summary>
        /// <param name="multiplicator">Multiplicator</param>
        /// <param name="data">Instance of Data</param>
        /// <returns>Computed data</returns>
        public static Data<T> operator *(Data<T> data, double multiplicator)
        {
            if (data.IsEmpty) return data;
            var length = data.Length;
            var signal = new T[length];
            for (int i = 0; i < length; i++)
            {
                signal[i] = MultiplyDouble(data[i], multiplicator);
            }
            return new Data<T>(signal);
        }
        /// <summary>
        /// Change data type from T to byte
        /// </summary>
        /// <param name="data">Data T to change</param>
        /// <returns>Instance of Data byte</returns>
        public static Data<byte> ToByte(Data<T> data)
        {
            if (data.IsEmpty) return new EmptyData<byte>();
            var length = data.Length;
            var signal = new byte[length];
            for (int i = 0; i < length; i++)
            {
                signal[i] = toByte(data[i]);
            }
            return new Data<byte>(signal);
        }
        /// <summary>
        /// Change data type from T to int
        /// </summary>
        /// <param name="data">Data T to change</param>
        /// <returns>Instance of Data int</returns>
        public static Data<int> ToInt(Data<T> data)
        {
            if (data.IsEmpty) return new EmptyData<int>();
            var length = data.Length;
            var signal = new int[length];
            for (int i = 0; i < length; i++)
            {
                signal[i] = toInt(data[i]);
            }
            return new Data<int>(signal);
        }
        /// <summary>
        /// Change data type from T to float
        /// </summary>
        /// <param name="data">Data T to change</param>
        /// <returns>Instance of Data float</returns>
        public static Data<float> ToFloat(Data<T> data)
        {
            if (data.IsEmpty) return new EmptyData<float>();
            var length = data.Length;
            var signal = new float[length];
            for (int i = 0; i < length; i++)
            {
                signal[i] = toFloat(data[i]);
            }
            return new Data<float>(signal);
        }
        /// <summary>
        /// Change data type from T to double
        /// </summary>
        /// <param name="data">Data T to change</param>
        /// <returns>Instance of Data double</returns>
        public static Data<double> ToDouble(Data<T> data)
        {
            if (data.IsEmpty) return new EmptyData<double>();
            var length = data.Length;
            var signal = new double[length];
            for (int i = 0; i < length; i++)
            {
                signal[i] = toDouble(data[i]);
            }
            return new Data<double>(signal);
        }
        /// <summary>
        /// Change data type from float to T
        /// </summary>
        /// <param name="data">Data float to change</param>
        /// <returns>Instance of Data T</returns>
        public static Data<T> FloatToT(Data<float> data)
        {
            if (data.IsEmpty) return new EmptyData<T>();
            var length = data.Length;
            var signal = new T[length];
            for (int i = 0; i < length; i++)
            {
                signal[i] = floatToT(data[i]);
            }
            return new Data<T>(signal);
        }
        /// <summary>
        /// Change data type from double to T
        /// </summary>
        /// <param name="data">Data double to change</param>
        /// <returns>Instance of Data T</returns>
        public static Data<T> DoubleToT(Data<double> data)
        {
            if (data.IsEmpty) return new EmptyData<T>();
            var length = data.Length;
            var signal = new T[length];
            for (int i = 0; i < length; i++)
            {
                signal[i] = doubleToT(data[i]);
            }
            return new Data<T>(signal);
        }
        /// <summary>
        /// Compute floor (data + 1/2)
        /// </summary>
        /// <param name="data">Instance of Data</param>
        /// <returns>Computed data</returns>
        public static Data<double> FloorWithHalfAdded(Data<double> data)
        {
            if (data.IsEmpty) return data;
            var length = data.Length;
            var signal = new double[length];
            for (int i = 0; i < length; i++)
            {
                signal[i] = Math.Floor(data[i] + 1/2d);
            }
            return new Data<double>(signal);
        }
    }
    /// <summary>
    /// Data with zero signal
    /// </summary>
    /// <typeparam name="T">Type of signal</typeparam>
    public class ZeroData<T> : Data<T>
    {
        private ZeroData(T[] signal) : base(signal) { }
        /// <summary>
        /// Constructor creates ZeroData with length 1
        /// </summary>
        public ZeroData() : this(1) { }
        /// <summary>
        /// Constructor creates ZeroData with specific length
        /// </summary>
        /// <param name="signalLength">Signal length</param>
        public ZeroData(int signalLength) : base(Enumerable.Repeat(default(T), signalLength).ToArray()) { }
    }
    /// <summary>
    /// Empty data
    /// Used as non-valid Data instance
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public class EmptyData<T> : Data<T>
    {
        public EmptyData() : base((T[])null)
        {
            isEmpty = true;
        }
    }
}
