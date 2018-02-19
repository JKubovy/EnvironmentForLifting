using System;
using System.Threading.Tasks;

namespace EnvironmentForLifting
{
    /// <summary>
    /// Node splitting data in n outputs
    /// Inside just recalculate dataNumber and output
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public class Split<T> : Segment<T>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="outputCount">Number of output</param>
        /// <param name="predecessor">Predecessor node</param>
        public Split(int outputCount, Node<T> predecessor) : base(outputCount, predecessor) { }
        public override void BuildSegment()
        {
            var outputCount = outputNodes.Length;
            for (int i = 0; i < outputCount; i++)
            {
                var offset = i;
                outputNodes[i] = new UniversalNode<T>(
                    (InputStreamsEnvelope<T>[] input, int dataNumber) =>
                    {
                        return input[0].GetData(dataNumber * outputCount + offset);
                    }, inputNodes[0]);
            }
        }
    }
    /// <summary>
    /// Haar lifting segment
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public class Haar<T> : Segment<T>
    {
        /// <summary>
        /// Update step in lifting
        /// </summary>
        /// <typeparam name="K">Type of data</typeparam>
        private class Update<K> : Node<K>
        {
            public Update(Node<K> predecessor) : base(predecessor) { }
            public override Data<K> GetData(int dataNumber)
            {
                return Input[0].GetData(dataNumber) / 2;
            }
        }
        public Haar(Node<T> predecessor) : base(2, predecessor) { }
        public override void BuildSegment()
        {
            var splitter = new Split<T>(2, inputNodes[0]);
            var substract = new Subtract<T>(splitter[1], splitter[0]);
            var Update = new Update<T>(substract);
            var add = new Add<T>(splitter[0], Update);
            outputNodes[0] = add;
            outputNodes[1] = substract;
        }
    }
    /// <summary>
    /// Inverse Haar lifting segment
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public class HaarInverse<T> : Segment<T>
    {
        /// <summary>
        /// Update step in lifting
        /// </summary>
        /// <typeparam name="K">Type of data</typeparam>
        private class Update<K> : Node<K>
        {
            public Update(Node<K> predecessor) : base(predecessor) { }
            public override Data<K> GetData(int dataNumber)
            {
                return Input[0].GetData(dataNumber) / 2;
            }
        }
        public HaarInverse(Node<T> predecessor1, Node<T> predecessor2) : base(1, predecessor1, predecessor2) { }
        public override void BuildSegment()
        {
            var Update = new Update<T>(inputNodes[1]);
            var substract = new Subtract<T>(inputNodes[0], Update);
            var add = new Add<T>(inputNodes[1], substract);
            var merge = new Merge<T>(substract, add);
            outputNodes[0] = merge;
        }
    }
    /// <summary>
    /// Basic lifting segment
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public class BaseWavelet<T> : Segment<T>
    {
        public BaseWavelet(Node<T> predecessor) : base(2, predecessor) { }
        public override void BuildSegment()
        {
            var d = new UniversalNode<T>(
                (input, dataNumber) =>{
                    var data1 = input[0].GetData(2 * dataNumber + 1);
                    var data2 = input[0].GetData(2 * dataNumber - 2);
                    var data3 = input[0].GetData(2 * dataNumber);
                    return data1 - ((data2 + data3) / 2);
                }, inputNodes[0]);
            var s = new UniversalNode<T>(
                (input, dataNumber) => {
                    var data1 = input[0].GetData(2 * dataNumber);
                    var data2 = input[1].GetData(dataNumber - 1);
                    var data3 = input[1].GetData(dataNumber);
                    return data1 + ((data2 + data3) / 4);
                }, inputNodes[0], d);
            outputNodes[0] = s;
            outputNodes[1] = d;
        }
    }
    /// <summary>
    /// Inverse Basic lifting segment
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public class BaseWaveletInverse<T> : Segment<T>
    {
        public BaseWaveletInverse(Node<T> predecessor1, Node<T> predecessor2) : base(1, predecessor1, predecessor2) { }
        public override void BuildSegment()
        {
            var U = new UniversalNode<T>(
                (input, dataNumber) =>
                {
                    var data1 = input[0].GetData(dataNumber - 1);
                    var data2 = input[0].GetData(dataNumber);
                    return (data1 + data2) / 4;
                }, inputNodes[1]);
            var substract = new Subtract<T>(inputNodes[0], U);
            var P = new UniversalNode<T>(
                (input, dataNumber) =>
                {
                    var data1 = input[0].GetData(dataNumber - 1);
                    var data2 = input[0].GetData(dataNumber);
                    return (data1 + data2) / 2;
                }, substract);
            var add = new Add<T>(P, inputNodes[1]);
            var merge = new Merge<T>(substract, add);
            outputNodes[0] = merge;
        }
    }
    /// <summary>
    /// Daubechies D4 lifting segment
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public class DaubechiesWaveletD4<T> : Segment<T>
    {
        public DaubechiesWaveletD4(Node<T> predecessor) : base(2, predecessor) { }
        public override void BuildSegment()
        {
            var d1 = new UniversalNode<T>(
                (input, dataNumber) =>
                {
                    var data1 = input[0].GetData(2 * dataNumber + 1);
                    var data2 = input[0].GetData(2 * dataNumber);
                    return data1 - Math.Sqrt(3) * data2;
                }, inputNodes[0]);
            var s1 = new UniversalNode<T>(
                (input, dataNumber) =>
                {
                    var data1 = input[0].GetData(2 * dataNumber);
                    var data2 = input[1].GetData(dataNumber);
                    var data3 = input[1].GetData(dataNumber + 1);
                    return data1 + (Math.Sqrt(3) / 4) * data2 + ((Math.Sqrt(3) - 2) / 4) * data3;
                }, inputNodes[0], d1);
            var d2 = new UniversalNode<T>(
                (input, dataNumber) =>
                {
                    var data1 = input[0].GetData(dataNumber);
                    var data2 = input[1].GetData(dataNumber - 1);
                    return data1 + data2;
                }, d1, s1);
            var s = new UniversalNode<T>(
                (input, dataNumber) =>
                {
                    var data = input[0].GetData(dataNumber);
                    return ((Math.Sqrt(3) + 1) / (Math.Sqrt(2))) * data;
                }, s1);
            var d = new UniversalNode<T>(
                (input, dataNumber) =>
                {
                    var data = input[0].GetData(dataNumber);
                    return ((Math.Sqrt(3) - 1) / (Math.Sqrt(2))) * data;
                }, d2);
            outputNodes[0] = s;
            outputNodes[1] = d;
        }
    }
    /// <summary>
    /// Inverse Daubechies D4 lifting segment
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public class DaubechiesWaveletD4Inverse<T> : Segment<T>
    {
        public DaubechiesWaveletD4Inverse(Node<T> predecessor1, Node<T> predecessor2) : base(1, predecessor1, predecessor2) { }

        public override void BuildSegment()
        {
            var d2 = new UniversalNode<T>(
                (input, dataNumber) =>
                {
                    var data = input[0].GetData(dataNumber);
                    return ((Math.Sqrt(3) + 1) / Math.Sqrt(2)) * data;
                }, inputNodes[1]);
            var s1 = new UniversalNode<T>(
                (input, dataNumber) =>
                {
                    var data = input[0].GetData(dataNumber);
                    return ((Math.Sqrt(3) - 1) / Math.Sqrt(2)) * data;
                }, inputNodes[0]);
            var d1 = new UniversalNode<T>(
                (input, dataNumber) =>
                {
                    var data1 = input[0].GetData(dataNumber);
                    var data2 = input[1].GetData(dataNumber - 1);
                    return data1 - data2;
                }, d2, s1);
            var xEven = new UniversalNode<T>(
                (input, dataNumber) =>
                {
                    var data1 = input[0].GetData(dataNumber);
                    var data2 = input[1].GetData(dataNumber);
                    var data3 = input[1].GetData(dataNumber + 1);
                    return data1 - Math.Sqrt(3) / 4 * data2 - (Math.Sqrt(3) - 2) / 4 * data3;
                }, s1, d1);
            var xOdd = new UniversalNode<T>(
                (input, dataNumber) =>
                {
                    var data1 = input[0].GetData(dataNumber);
                    var data2 = input[1].GetData(dataNumber);
                    return data1 + Math.Sqrt(3) * data2;
                }, d1, xEven);
            var merger = new Merge<T>(xEven, xOdd);
            outputNodes[0] = merger;
        }
    }
    /// <summary>
    /// Daubechies D4 integer to integer lifting segment
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public class DaubechiesWaveletD4Integer<T> : Segment<T>
    {
        public DaubechiesWaveletD4Integer(Node<T> predecessor) : base(2, predecessor) { }

        private double K = ((Math.Sqrt(3) + 1) / Math.Sqrt(2)) / Math.Sqrt(2);
        public override void BuildSegment()
        {
            var d1 = new UniversalNode<T>(
                (input, dataNumber) => {
                    var data1 = Data<T>.ToDouble(input[0].GetData(2 * dataNumber + 1));
                    var data2 = Data<T>.ToDouble(input[0].GetData(2 * dataNumber));
                    var result = data1 - Data<double>.FloorWithHalfAdded(Math.Sqrt(3) * data2);
                    return Data<T>.DoubleToT(result);
                }, inputNodes[0]);
            var sM = new UniversalNode<T>(
                (input, dataNumber) => {
                    var data1 = Data<T>.ToDouble(input[0].GetData(2 * dataNumber));
                    var data2 = Data<T>.ToDouble(input[1].GetData(dataNumber));
                    var data3 = Data<T>.ToDouble(input[1].GetData(dataNumber - 1));
                    var result = data1 + Data<double>.FloorWithHalfAdded((Math.Sqrt(3)/4) * data2 + ((Math.Sqrt(3) - 2)/4)*data3);
                    return Data<T>.DoubleToT(result);
                }, inputNodes[0], d1);
            var dM = new UniversalNode<T>(
                (input, dataNumber) => {
                    var data1 = input[0].GetData(dataNumber);
                    var data2 = input[1].GetData(dataNumber + 1);
                    return data1 + data2;
                }, d1, sM);
            var s = new UniversalNode<T>(
                (input, dataNumber) =>
                {
                    var data = Data<T>.ToDouble(input[0].GetData(dataNumber));
                    var result = Data<double>.FloorWithHalfAdded(K * data);
                    return Data<T>.DoubleToT(result);
                }, sM);
            var d = new UniversalNode<T>(
                (input, dataNumber) =>
                {
                    var data = Data<T>.ToDouble(input[0].GetData(dataNumber));
                    var result = Data<double>.FloorWithHalfAdded((1 / K) * data);
                    return Data<T>.DoubleToT(result);
                }, dM);
            outputNodes[0] = s;
            outputNodes[1] = d;
        }
    }
    /// <summary>
    /// Inverse Daubechies D4 integer to integer lifting segment
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DaubechiesWaveletD4IntegerInverse<T> : Segment<T>
    {
        public DaubechiesWaveletD4IntegerInverse(Node<T> predecessor1, Node<T> predecessor2) : base(1, predecessor1, predecessor2) { }

        private double K = ((Math.Sqrt(3) + 1) / Math.Sqrt(2)) / Math.Sqrt(2);
        public override void BuildSegment()
        {
            var sM = new UniversalNode<T>(
                (input, dataNumber) =>
                {
                    var data = Data<T>.ToDouble(input[0].GetData(dataNumber));
                    var result = Data<double>.FloorWithHalfAdded((1 / K) * data);
                    return Data<T>.DoubleToT(result);
                }, inputNodes[0]);
            var dM = new UniversalNode<T>(
                (input, dataNumber) =>
                {
                    var data = Data<T>.ToDouble(input[0].GetData(dataNumber));
                    var result = Data<double>.FloorWithHalfAdded(K * data);
                    return Data<T>.DoubleToT(result);
                }, inputNodes[1]);
            var d1 = new UniversalNode<T>(
                (input, dataNumber) => {
                    var data1 = input[0].GetData(dataNumber);
                    var data2 = input[1].GetData(dataNumber + 1);
                    return data1 - data2;
                }, dM, sM);
            var s = new UniversalNode<T>(
                (input, dataNumber) => {
                    var data1 = Data<T>.ToDouble(input[0].GetData(dataNumber));
                    var data2 = Data<T>.ToDouble(input[1].GetData(dataNumber));
                    var data3 = Data<T>.ToDouble(input[1].GetData(dataNumber - 1));
                    var result = data1 - Data<double>.FloorWithHalfAdded(Math.Sqrt(3) / 4 * data2 + (Math.Sqrt(3) - 2) / 4 * data3);
                    return Data<T>.DoubleToT(result);
                }, sM, d1);
            var d = new UniversalNode<T>(
                (input, dataNumber) => {
                    var data1 = Data<T>.ToDouble(input[0].GetData(dataNumber));
                    var data2 = Data<T>.ToDouble(input[1].GetData(dataNumber));
                    var result = data1 + Data<double>.FloorWithHalfAdded(Math.Sqrt(3) * data2);
                    return Data<T>.DoubleToT(result);
                }, d1, s);
            var merger = new Merge<T>(s, d);
            outputNodes[0] = merger;
        }
    }
    /// <summary>
    /// (2,2) Interpolating integer to integer lifting segment
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public class TwoTwoInterpolatingInteger<T> : Segment<T>
    {
        public TwoTwoInterpolatingInteger(Node<T> predecessor) : base(2, predecessor) { }
        public override void BuildSegment()
        {
            var d = new UniversalNode<T>(
                (input, dataNumber) =>
                {
                    var data1 = Data<T>.ToDouble(input[0].GetData(2 * dataNumber + 1));
                    var data2 = Data<T>.ToDouble(input[0].GetData(2 * dataNumber));
                    var data3 = Data<T>.ToDouble(input[0].GetData(2 * dataNumber + 2));
                    var result = data1 - Data<double>.FloorWithHalfAdded((1/2d) * (data2 + data3));
                    return Data<T>.DoubleToT(result);
                }, inputNodes[0]);
            var s = new UniversalNode<T>(
                (input, dataNumber) =>
                {
                    var data1 = Data<T>.ToDouble(input[0].GetData(2 * dataNumber));
                    var data2 = Data<T>.ToDouble(input[1].GetData(dataNumber - 1));
                    var data3 = Data<T>.ToDouble(input[1].GetData(dataNumber));
                    var result = data1 + Data<double>.FloorWithHalfAdded((1/4d) * (data2 + data3));
                    return Data<T>.DoubleToT(result);
                }, inputNodes[0], d);
            outputNodes[0] = s;
            outputNodes[1] = d;
        }
    }
    /// <summary>
    /// (4,2) Interpolating integer to integer lifting segment
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public class FourTwoInterpolatingInteger<T> : Segment<T>
    {
        public FourTwoInterpolatingInteger(Node<T> predecessor) : base(2, predecessor) { }
        public override void BuildSegment()
        {
            var d = new UniversalNode<T>(
                (input, dataNumber) =>
                {
                    var data1 = Data<T>.ToDouble(input[0].GetData(2 * dataNumber + 1));
                    var data2 = Data<T>.ToDouble(input[0].GetData(2 * dataNumber));
                    var data3 = Data<T>.ToDouble(input[0].GetData(2 * dataNumber + 2));
                    var data4 = Data<T>.ToDouble(input[0].GetData(2 * dataNumber - 2));
                    var data5 = Data<T>.ToDouble(input[0].GetData(2 * dataNumber + 4));
                    var result = data1 - Data<double>.FloorWithHalfAdded((9 / 16d) * (data2 + data3) - (1/16d)*(data4+data5));
                    return Data<T>.DoubleToT(result);
                }, inputNodes[0]);
            var s = new UniversalNode<T>(
                (input, dataNumber) =>
                {
                    var data1 = Data<T>.ToDouble(input[0].GetData(2 * dataNumber));
                    var data2 = Data<T>.ToDouble(input[1].GetData(dataNumber - 1));
                    var data3 = Data<T>.ToDouble(input[1].GetData(dataNumber));
                    var result = data1 + Data<double>.FloorWithHalfAdded((1 / 4d) * (data2 + data3));
                    return Data<T>.DoubleToT(result);
                }, inputNodes[0], d);
            outputNodes[0] = s;
            outputNodes[1] = d;
        }
    }
    /// <summary>
    /// (2,4) Interpolating integer to integer lifting segment
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public class TwoFourInterpolatingInteger<T> : Segment<T>
    {
        public TwoFourInterpolatingInteger(Node<T> predecessor) : base(2, predecessor) { }
        public override void BuildSegment()
        {
            var d = new UniversalNode<T>(
                (input, dataNumber) =>
                {
                    var data1 = Data<T>.ToDouble(input[0].GetData(2 * dataNumber + 1));
                    var data2 = Data<T>.ToDouble(input[0].GetData(2 * dataNumber));
                    var data3 = Data<T>.ToDouble(input[0].GetData(2 * dataNumber + 2));
                    var result = data1 - Data<double>.FloorWithHalfAdded((1 / 2d) * (data2 + data3));
                    return Data<T>.DoubleToT(result);
                }, inputNodes[0]);
            var s = new UniversalNode<T>(
                (input, dataNumber) =>
                {
                    var data1 = Data<T>.ToDouble(input[0].GetData(2 * dataNumber));
                    var data2 = Data<T>.ToDouble(input[1].GetData(dataNumber - 1));
                    var data3 = Data<T>.ToDouble(input[1].GetData(dataNumber));
                    var data4 = Data<T>.ToDouble(input[1].GetData(dataNumber - 2));
                    var data5 = Data<T>.ToDouble(input[1].GetData(dataNumber + 1));
                    var result = data1 + Data<double>.FloorWithHalfAdded((19 / 64d) * (data2 + data3) - (3 / 64d) * (data4 + data5));
                    return Data<T>.DoubleToT(result);
                }, inputNodes[0], d);
            outputNodes[0] = s;
            outputNodes[1] = d;
        }
    }
    /// <summary>
    /// (4,4) Interpolating integer to integer lifting segment
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public class FourFourInterpolatingInteger<T> : Segment<T>
    {
        public FourFourInterpolatingInteger(Node<T> predecessor) : base(2, predecessor) { }
        public override void BuildSegment()
        {
            var d = new UniversalNode<T>(
                (input, dataNumber) =>
                {
                    var data1 = Data<T>.ToDouble(input[0].GetData(2 * dataNumber + 1));
                    var data2 = Data<T>.ToDouble(input[0].GetData(2 * dataNumber));
                    var data3 = Data<T>.ToDouble(input[0].GetData(2 * dataNumber + 2));
                    var data4 = Data<T>.ToDouble(input[0].GetData(2 * dataNumber - 2));
                    var data5 = Data<T>.ToDouble(input[0].GetData(2 * dataNumber + 4));
                    var result = data1 - Data<double>.FloorWithHalfAdded((9 / 16d) * (data2 + data3) - (1 / 16d) * (data4 + data5));
                    return Data<T>.DoubleToT(result);
                }, inputNodes[0]);
            var s = new UniversalNode<T>(
                (input, dataNumber) =>
                {
                    var data1 = Data<T>.ToDouble(input[0].GetData(2 * dataNumber));
                    var data2 = Data<T>.ToDouble(input[1].GetData(dataNumber - 1));
                    var data3 = Data<T>.ToDouble(input[1].GetData(dataNumber));
                    var data4 = Data<T>.ToDouble(input[1].GetData(dataNumber - 2));
                    var data5 = Data<T>.ToDouble(input[1].GetData(dataNumber + 1));
                    var result = data1 + Data<double>.FloorWithHalfAdded((9 / 32) * (data2 + data3) - (1 / 32) * (data4 + data5));
                    return Data<T>.DoubleToT(result);
                }, inputNodes[0], d);
            outputNodes[0] = s;
            outputNodes[1] = d;
        }
    }
    /// <summary>
    /// (2+2,2) integer to integer with four vanishing moments lifting segment
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public class TwoPlusTwoTwoInteger<T> : Segment<T>
    {
        double alpha, beta, gama;
        public TwoPlusTwoTwoInteger(Node<T> predecessor) : base(2, predecessor)
        {
            alpha = 1 / 4d;
            beta = -1 / 4d;
            gama = 1;
        }

        public override void BuildSegment()
        {
            var d1 = new UniversalNode<T>(
                (input, dataNumber) =>
                {
                    var data1 = Data<T>.ToDouble(input[0].GetData(2 * dataNumber + 1));
                    var data2 = Data<T>.ToDouble(input[0].GetData(2 * dataNumber));
                    var data3 = Data<T>.ToDouble(input[0].GetData(2 * dataNumber + 2));
                    var result = data1 - Data<double>.FloorWithHalfAdded((1 / 2) * (data2 + data3));
                    return Data<T>.DoubleToT(result);
                }, inputNodes[0]);
            var s = new UniversalNode<T>(
                (input, dataNumber) =>
                {
                    var data1 = Data<T>.ToDouble(input[0].GetData(2 * dataNumber + 1));
                    var data2 = Data<T>.ToDouble(input[1].GetData(dataNumber - 1));
                    var data3 = Data<T>.ToDouble(input[1].GetData(dataNumber));
                    var result = data1 + Data<double>.FloorWithHalfAdded((1 / 4) * (data2 + data3));
                    return Data<T>.DoubleToT(result);
                }, inputNodes[0], d1);
            var d = new UniversalNode<T>(
                (input, dataNumber) =>
                {
                    var data1 = Data<T>.ToDouble(input[0].GetData(dataNumber));
                    var data2 = Data<T>.ToDouble(input[1].GetData(dataNumber - 1));
                    var data3 = Data<T>.ToDouble(input[1].GetData(dataNumber));
                    var data4 = Data<T>.ToDouble(input[1].GetData(dataNumber + 1));
                    var data5 = Data<T>.ToDouble(input[1].GetData(dataNumber + 2));
                    var data6 = Data<T>.ToDouble(input[0].GetData(dataNumber + 1));
                    var result = data1 - Data<double>.FloorWithHalfAdded(
                        (alpha * ((-1 / 2d) * data2 + data3 - (1 / 2d) * data4)) +
                        (beta * ((-1 / 2d) * data3 + data4 - (1 / 2d) * data5)) + 
                        (gama * data6));
                    return Data<T>.DoubleToT(result);
                }, d1, s);
            outputNodes[0] = s;
            outputNodes[1] = d;
        }
        private void CheckParameters()
        {
            if (8*beta + 2*gama != 1 ||
                4*alpha + 4*beta + gama != 1)
            {
                Logger.Error("Parameters have to pass conditions");
                throw new ArgumentException("Parameters have to pass conditions");
            }
        }
        public override void SetParametersForThisNode(ParameterStorage parameterStorage)
        {
            alpha = parameterStorage.GetDoubleParameter("alpha");
            beta = parameterStorage.GetDoubleParameter("beta");
            gama = parameterStorage.GetDoubleParameter("gama");

        }
    }
    /// <summary>
    /// (9-7) Symmetric Biorthogonal lifting segment
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public class NineSevenBiortogonalInteger<T> : Segment<T>
    {
        double alpha, beta, gama, delta, K;
        public NineSevenBiortogonalInteger(Node<T> predecessor) : base(2, predecessor)
        {
            alpha = -1.586134342;
            beta = -0.05298011854;
            gama = 0.8829110762;
            delta = 0.4435068522;
            K = 1.149604398 / Math.Sqrt(2);
        }
        public override void BuildSegment()
        {
            var d1 = new UniversalNode<T>(
                (input, dataNumber) =>
                {
                    var data1 = Data<T>.ToDouble(input[0].GetData(2 * dataNumber + 1));
                    var data2 = Data<T>.ToDouble(input[0].GetData(2 * dataNumber));
                    var data3 = Data<T>.ToDouble(input[0].GetData(2 * dataNumber + 2));
                    var result = data1 + Data<double>.FloorWithHalfAdded(alpha * (data2 + data3));
                    return Data<T>.DoubleToT(result);
                }, inputNodes[0]);
            var s1 = new UniversalNode<T>(
                (input, dataNumber) =>
                {
                    var data1 = Data<T>.ToDouble(input[0].GetData(2 * dataNumber));
                    var data2 = Data<T>.ToDouble(input[1].GetData(dataNumber));
                    var data3 = Data<T>.ToDouble(input[1].GetData(dataNumber - 1));
                    var result = data1 + Data<double>.FloorWithHalfAdded(beta * (data2 + data3));
                    return Data<T>.DoubleToT(result);
                }, inputNodes[0], d1);
            var dm = new UniversalNode<T>(
                (input, dataNumber) =>
                {
                    var data1 = Data<T>.ToDouble(input[0].GetData(dataNumber));
                    var data2 = Data<T>.ToDouble(input[1].GetData(dataNumber));
                    var data3 = Data<T>.ToDouble(input[1].GetData(dataNumber + 1));
                    var result = data1 + Data<double>.FloorWithHalfAdded(gama * (data2 + data3));
                    return Data<T>.DoubleToT(result);
                }, d1, s1);
            var sm = new UniversalNode<T>(
                (input, dataNumber) =>
                {
                    var data1 = Data<T>.ToDouble(input[0].GetData(dataNumber));
                    var data2 = Data<T>.ToDouble(input[1].GetData(dataNumber));
                    var data3 = Data<T>.ToDouble(input[1].GetData(dataNumber - 1));
                    var result = data1 + Data<double>.FloorWithHalfAdded(delta * (data2 + data3));
                    return Data<T>.DoubleToT(result);
                }, s1, dm);
            var d = new UniversalNode<T>(
                (input, dataNumber) =>
                {
                    var data1 = Data<T>.ToDouble(input[0].GetData(dataNumber));
                    var result = Data<double>.FloorWithHalfAdded((1 / K) * data1);
                    return Data<T>.DoubleToT(result);
                }, dm);
            var s = new UniversalNode<T>(
                (input, dataNumber) =>
                {
                    var data1 = Data<T>.ToDouble(input[0].GetData(dataNumber));
                    var result = Data<double>.FloorWithHalfAdded(K * data1);
                    return Data<T>.DoubleToT(result);
                }, sm);
            outputNodes[0] = s;
            outputNodes[1] = d;
        }
    }
}