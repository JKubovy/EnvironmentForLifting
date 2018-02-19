using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace EnvironmentForLifting
{
    public class DataDouble : IEnumerable<double>
    {
        private double[] _signal;
        public DataDouble(double[] signal)
        {
            _signal = signal;
        }
        public double this[int key] => _signal[key];
        public int Length
        {
            get
            {
                return _signal.Length;
            }
        }
        public static DataDouble operator +(DataDouble data1, DataDouble data2)
        {
            var length = data1.Length;
            var signal = new double[length];
            for (int i = 0; i < length; i++)
            {
                signal[i] = data1[i] + data2[i];
            }
            return new DataDouble(signal);
        }
        public static DataDouble operator/(DataDouble data, int divisor)
        {
            var length = data.Length;
            var signal = new double[length];
            for (int i = 0; i < length; i++)
            {
                signal[i] = data[i] / divisor;
            }
            return new DataDouble(signal);
        }
        public IEnumerator<double> GetEnumerator()
        {
            return ((IEnumerable<double>)_signal).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<double>)_signal).GetEnumerator();
        }
    }
    public class DataDoubleDynamic : Data<double>
    {
        public DataDoubleDynamic(double[] signal) : base(signal) { }
        public static DataDoubleDynamic operator +(DataDoubleDynamic data1, DataDoubleDynamic data2)
        {
            var length = data1.Length;
            var signal = new double[length];
            for (int i = 0; i < length; i++)
            {
                signal[i] = (dynamic)data1[i] + (dynamic)data2[i];
            }
            return new DataDoubleDynamic(signal);
        }
        public static DataDoubleDynamic operator /(DataDoubleDynamic data, int divisor)
        {
            var length = data.Length;
            var signal = new double[length];
            for (int i = 0; i < length; i++)
            {
                signal[i] = (dynamic)data[i] / divisor;
            }
            return new DataDoubleDynamic(signal);
        }
    }
    public class Print<T> : Node<T>
    {
        TextWriter textWriter;
        public Print(Node<T> node) : base(node)
        {
            textWriter = Console.Out;
        }
        public Print<T> SetTextWriter(TextWriter textWriter)
        {
            this.textWriter = textWriter;
            return this;
        }
        public override void Process()
        {
            WorkBatch(new int[] { 0, 1, 1, 0, 2 });
        }
        public override void ProcessAsync()
        {
            WorkAsyncBatch(new int[] { 0, 1, 1, 0, 2 });
        }
        public void Work(int dataNumber)
        {
            foreach (var item in Input[0].GetData(dataNumber))
            {
                textWriter.Write("{0} ", item);
            }
            textWriter.WriteLine();
        }
        public void WorkBatch(int[] dataNumbers)
        {
            Data<T>[] results = new Data<T>[dataNumbers.Length];
            int index = 0;
            foreach (var dataNumber in dataNumbers)
            {
                results[index++] = Input[0].GetData(dataNumber);
            }
            foreach (var result in results)
            {
                foreach (var item in result)
                {
                    textWriter.Write("{0} ", item);
                }
                textWriter.WriteLine();
            }
        }
        public void WorkAsyncBatch(int[] dataNumbers)
        {
            Task<Data<T>>[] results = new Task<Data<T>>[dataNumbers.Length];
            int index = 0;
            foreach (var dataNumber in dataNumbers)
            {
                results[index++] = Task.Run(() =>
                {
                    return Input[0].GetData(dataNumber);
                });
            }
            foreach (var result in results)
            {
                foreach (var item in result.Result)
                {
                    textWriter.Write("{0} ", item);
                }
                textWriter.WriteLine();
            }
        }
    }
    public class Delayer<T> : Node<T>
    {
        private int miliseconds;
        public Delayer()
        {
            this.miliseconds = 0;
        }
        public Delayer<T> SetDelayTimeInSeconds(int seconds)
        {
            this.miliseconds = seconds * 1000;
            return this;
        }
        public Delayer<T> SetDelayTimeInMiliseconds(int miliseconds)
        {
            this.miliseconds = miliseconds;
            return this;
        }
        public override Data<T> GetData(int dataNumber)
        {
            System.Threading.Thread.Sleep(miliseconds);
            return new Data<T>(new T[] { default(T) });
        }
    }
}
