using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace EnvironmentForLifting
{
    /// <summary>
    /// Abstract class that provides common variables for node processing bitmap
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ImageProcessor<T> : Node<T>
    {
        protected Bitmap image;
        protected Size size;
        protected string name;
        protected byte[] rawData;
        protected int sizeofPixelFormat;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="image">Bitmap that will be read or write to</param>
        /// <param name="name">Name of bitmap in filesystem</param>
        /// <param name="predecessors">Arbitrary number of predecessor nodes</param>
        public ImageProcessor(Bitmap image, string name, params Node<T>[] predecessors) : base(predecessors)
        {
            this.image = image;
            this.size = image.Size;
            this.name = name;
            sizeofPixelFormat = Image.GetPixelFormatSize(image.PixelFormat) / 8;
            this.rawData = new byte[sizeofPixelFormat * size.Width * size.Height];
        }
        /// <summary>
        /// Set bitmap name
        /// </summary>
        /// <param name="name">Bitmap name</param>
        /// <returns>This node</returns>
        public ImageProcessor<T> SetImageName(string name)
        {
            this.name = name;
            return this;
        }
    }
    /// <summary>
    /// Bitmap reader
    /// Read pixels from left to right nad from top to bottom
    /// Data lenght is variable depend on pixel format
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public class BitmapReader<T> : ImageProcessor<T>
    {
        Data<T> zeroData;
        byte[][] splitRawData;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="image">Bitmap that will be read</param>
        public BitmapReader(Bitmap image) : base(image, "")
        {
            zeroData = new ZeroData<T>(sizeofPixelFormat);
            splitRawData = new byte[size.Width * size.Height][];
            for (int i = 0; i < size.Width * size.Height; i++)
            {
                splitRawData[i] = new byte[sizeofPixelFormat];
            }
            CopyImageData();
            outputStream.DisableDataStore();
        }
        /// <summary>
        /// Read all data from bitmap and store it to variable
        /// </summary>
        private void CopyImageData()
        {
            BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, size.Width, size.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            try
            {
                for (int i = 0; i < size.Width * size.Height; i++)
                {
                    Marshal.Copy(bitmapData.Scan0 + i * sizeofPixelFormat, splitRawData[i], 0, sizeofPixelFormat);
                }
            }
            finally
            {
                image.UnlockBits(bitmapData);
            }
        }
        public override Data<T> GetData(int dataNumber)
        {
            if (dataNumber < 0 || dataNumber >= size.Height * size.Width)
                return zeroData;
            return new Data<T>(splitRawData[dataNumber]);
        }
    }
    /// <summary>
    /// Final node that get and save data to bitmap
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public class BitmapWriter<T> : ImageProcessor<T>
    {
        byte[][] splitRawData;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="image">Bitmap in which will be data stored</param>
        /// <param name="name">Name of bitmap that will be saved</param>
        /// <param name="predecessor">One predecessor node</param>
        public BitmapWriter(Bitmap image, string name, Node<T> predecessor) : base(image, name, predecessor)
        {
            splitRawData = new byte[size.Width * size.Height][];
            for (int i = 0; i < size.Width * size.Height; i++)
            {
                splitRawData[i] = new byte[sizeofPixelFormat];
            }
            this.DataLength  = sizeofPixelFormat;
            Input[0].DisableDataStore();
        }
        /// <summary>
        /// Copy stored data to bitmap
        /// </summary>
        private void CopyImageData()
        {
            BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, size.Width, size.Height), ImageLockMode.WriteOnly, image.PixelFormat);
            try
            {
                for (int i = 0; i < size.Width * size.Height; i++)
                {
                    Marshal.Copy(splitRawData[i], 0, bitmapData.Scan0 + i * sizeofPixelFormat, sizeofPixelFormat);
                }
            }
            finally
            {
                image.UnlockBits(bitmapData);
            }
        }
        int progressWidth = 20;
        int cursorTop;
        /// <summary>
        /// Print progress
        /// How big part of data has been processed
        /// </summary>
        /// <param name="count"></param>
        /// <param name="total"></param>
        private void PrintProgress(int count, int total)
        {
            if (Math.Floor(count % (total / (float)progressWidth)) == 0 || count == total)
            {
                double progress = count / (double)total;
                int n = (int)(progress * progressWidth);
                //TODO: Edit to work with log4net
                //Console.SetCursorPosition(0, cursorTop);
                //Console.WriteLine("|{0}{1}{2}|", new String('-', n), n != progressWidth ? ">" : "-", new String(' ', progressWidth - n));
            }
        }
        /// <summary>
        /// Save bitmap with given name
        /// </summary>
        /// <param name="bitmap">Bitmap that need to be saved</param>
        /// <param name="name">Name of bitmap</param>
        private void SaveBitmap(ref Bitmap bitmap, string name)
        {
            try
            {
                bitmap.Save(name);
            }
            catch
            {
                Logger.Error($"Can't use {bitmap.PixelFormat} to save image.");
            }
        }
        public override void Process()
        {
            cursorTop = Console.CursorTop;
            int all = size.Width * size.Height;
            for (int y = 0; y < size.Height; y++)
            {
                for (int x = 0; x < size.Width; x++)
                {
                    var dataNumber = y * size.Width + x;
                    PrintProgress(dataNumber, all);
                    var data = Input[0].GetData(dataNumber);
                    if (data.IsEmpty)
                    {
                        Logger.Error("Empty data occure");
                        throw new Exception("Empty data");
                    }
                    var doubleData = Data<T>.ToDouble(data);
                    for (int i = 0; i < sizeofPixelFormat; i++)
                    {
                        double tmp;
                        if (i >= data.Length) tmp = doubleData[0];
                        else tmp = doubleData[i];
                        if (tmp > 255) tmp = 255;
                        if (tmp < 0) tmp = 0;
                        splitRawData[dataNumber][i] = (byte)tmp;
                    }
                    if (sizeofPixelFormat == 4) splitRawData[dataNumber][3] = 255;  // Alpha channel should be 255
                }
            }
            PrintProgress(all, all);
            CopyImageData();
            SaveBitmap(ref image, name);
        }
        public override void ProcessAsync()
        {
            Input[0].SetDropOldData(false);
            cursorTop = Console.CursorTop;
            int all = size.Width * size.Height;
            var tasks = new Task[all];
            for (int y = 0; y < size.Height; y++)
            {
                for (int x = 0; x < size.Width; x++)
                {
                    var dataNumber = y * size.Width + x;
                    PrintProgress(dataNumber, all);
                    tasks[dataNumber] = Task.Run(() =>
                    {
                        var data = Input[0].GetData(dataNumber);
                        if (data.IsEmpty)
                        {
                            Logger.Error("Empty data occure");
                            throw new Exception("Empty data");
                        }
                        var doubleData = Data<T>.ToDouble(data);
                        for (int i = 0; i < sizeofPixelFormat; i++)
                        {
                            double tmp;
                            if (i >= data.Length) tmp = doubleData[0];
                            else tmp = doubleData[i];
                            if (tmp > 255) tmp = 255;
                            if (tmp < 0) tmp = 0;
                            splitRawData[dataNumber][i] = (byte)tmp;
                        }
                        if (sizeofPixelFormat == 4) splitRawData[dataNumber][3] = 255;  // Alpha channel should be 255
                    });
                }
            }
            foreach (var task in tasks)
            {
                task.Wait();
            }
            PrintProgress(all, all);
            CopyImageData();
            SaveBitmap(ref image, name);
        }
        public override void SetParametersForThisNode(ParameterStorage parameterStorage)
        {
            name = parameterStorage.GetStringParameter("outputName");
        }
        public override void Reset()
        {
            splitRawData = new byte[size.Width * size.Height][];
            for (int i = 0; i < size.Width * size.Height; i++)
            {
                splitRawData[i] = new byte[sizeofPixelFormat];
            }
            base.Reset();
        }
    }
    /// <summary>
    /// Node that recalculate dataNumber to change direction of pixel read
    /// Change reading from left to right to top to bottom and vice versa
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public class ImageDiagonalFlipper<T> : Node<T>
    {
        Size inputSize;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="inputSize">Size of image that need to be flipped</param>
        /// <param name="predecessor">Predecessor node</param>
        public ImageDiagonalFlipper(Size inputSize, Node<T> predecessor) : base(predecessor)
        {
            this.inputSize = inputSize;
            Input[0].SetDropOldData(false);
            Input[0].DisableDataStore();
        }
        public override Data<T> GetData(int dataNumber)
        {
            int width = dataNumber / inputSize.Height;
            int height = dataNumber % inputSize.Height;
            int newDataNumber = (height * inputSize.Width) + width;
            return Input[0].GetData(newDataNumber);
        }
    }
    /// <summary>
    /// Node that merge data from MallatDecomposition to one output
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public class MallatMerger<T> : Node<T>
    {
        Size size;
        int levels;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="size">Size of whole image</param>
        /// <param name="levels">Number of decomposition levels</param>
        /// <param name="inputSegment">MallatDecomposition</param>
        public MallatMerger(Size size, int levels, Segment<T> inputSegment) : base(inputSegment.outputNodes)
        {
            this.size = size;
            this.levels = levels;
        }
        /// <summary>
        /// Calculate input index and dataNumber
        /// </summary>
        /// <param name="dataNumber">Wanted dataNumber</param>
        /// <returns>[Input index, dataNumber]</returns>
        private int[] GetIndexes(int dataNumber)
        {
            int height = dataNumber / size.Width;
            int width = dataNumber % size.Width;
            int index = 0;
            Size currentSize = size;
            currentSize.Width /= (int)(Math.Pow(2, levels));
            currentSize.Height /= (int)(Math.Pow(2, levels));
            if (width < currentSize.Width && height < currentSize.Height)
            {
                return new[] { index, (width % currentSize.Width) + (height % currentSize.Height) * currentSize.Width };
            }
            else
            {
                for (int i = 0; i < levels; i++)
                {
                    int addIndex = 0;
                    if (width >= currentSize.Width) addIndex += 2;
                    if (height >= currentSize.Height) addIndex += 1;
                    if (width < 2 * currentSize.Width && height < 2 * currentSize.Height)
                    {
                        return new[] { index + addIndex, (width % currentSize.Width) + (height % currentSize.Height) * currentSize.Width };
                    }
                    currentSize.Width *= 2;
                    currentSize.Height *= 2;
                    index += 3;
                }
                return new[] { 0, -1 }; // Zero data
            }
        }
        public override Data<T> GetData(int dataNumber)
        {
            var indexes = GetIndexes(dataNumber);
            return Input[indexes[0]].GetData(indexes[1]);
        }
    }
    /// <summary>
    /// Node that calcuclate entropy of data
    /// Commonly used to evaluate lifting segment
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public class MallatEntropyEvaluator<T> : Node<T>
    {
        Size size;
        int levels;
        int inputCount;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="size">Size of whole image</param>
        /// <param name="levels">Number of decomposition levels</param>
        /// <param name="inputSegment">MallatDecomposition</param>
        public MallatEntropyEvaluator(Size size, int levels, Segment<T> inputSegment) : base(inputSegment.outputNodes)
        {
            this.size = size;
            this.levels = levels;
            this.inputCount = (levels - 1) * 3 + 4;
        }
        /// <summary>
        /// Add data to data counter
        /// </summary>
        /// <param name="list">Data counter</param>
        /// <param name="data">Data to add</param>
        private void Update(List<Tuple<Data<int>, int>> list, Data<int> data)
        {
            Tuple<Data<int>, int> item = Tuple.Create(data, 1);
            bool found = false;
            for (int i = 0; i < list.Count; i++)
            {
                if (data.Equals(list[i].Item1))
                {
                    found = true;
                    list[i] = Tuple.Create(data, list[i].Item2+1);
                    break;
                }
            }
            if (!found)
            {
                list.Add(item);
            }
        }
        private double CalculateEntropyAsync(int inputIndex, double fraction, Size size)
        {
            List<Tuple<Data<int>, int>> counts = new List<Tuple<Data<int>, int>>();
            int all = size.Width * size.Height;
            var tasks = new Task<Data<T>>[all];
            for (int dataNumber = 0; dataNumber < all; dataNumber++)
            {
                int i = dataNumber;
                tasks[dataNumber] = Task.Run(() =>
                {
                    return Input[inputIndex].GetData(i);
                });
            }
            for (int dataNumber = 0; dataNumber < all; dataNumber++)
            {
                var data = Data<T>.ToInt(tasks[dataNumber].Result);
                Update(counts, data);
            }
            double result = 0;
            foreach (var item in counts)
            {
                double tmp = item.Item2 / (double)all;
                result -= (tmp * Math.Log(tmp, 2));
            }
            return result * fraction;
        }
         private double CalculateEntropySync(int inputIndex, double fraction, Size size)
        {
            List<Tuple<Data<int>, int>> counts = new List<Tuple<Data<int>, int>>();
            int all = size.Width * size.Height;
            for (int dataNumber = 0; dataNumber < all; dataNumber++)
            {
                var data = Data<T>.ToInt(Input[inputIndex].GetData(dataNumber));
                Update(counts, data);
            }
            double result = 0;
            foreach (var item in counts)
            {
                double tmp = item.Item2 / (double)all;
                result -= (tmp * Math.Log(tmp, 2));
            }
            return result * fraction;
        }
        public override void Process()
        {
            double result = 0;
            double fraction = 1 / 4d;
            int inputIndex = inputCount - 1;
            Size currentSize = new Size(size.Width, size.Height);
            for (int level = 0; level < levels; level++)
            { 
                for (int i = 0; i < 3; i++)
                {
                    result += CalculateEntropySync(inputIndex--, fraction, currentSize);
                }
                fraction /= 4;
                currentSize.Width /= 2;
                currentSize.Height /= 2;
            }
            result += CalculateEntropySync(inputIndex, fraction, currentSize);
            Logger.Info($"Entropy:\t{result}");
        }
        public override void ProcessAsync()
        {
            Input[0].SetDropOldData(false);
            double result = 0;
            double fraction = 1 / 4d;
            int inputIndex = inputCount - 1;
            Size currentSize = new Size(size.Width, size.Height);
            for (int level = 0; level < levels; level++)
            {
                for (int i = 0; i < 3; i++)
                {
                    result += CalculateEntropyAsync(inputIndex--, fraction, currentSize);
                }
                fraction /= 4;
                currentSize.Width /= 2;
                currentSize.Height /= 2;
            }
            result += CalculateEntropyAsync(inputIndex, fraction, currentSize);
            Logger.Info($"Entropy:\t{result}");
        }
    }
    /// <summary>
    /// Mallat decomposition of picture with given levels and lifting segment
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    /// <typeparam name="U">Type of lifting segment</typeparam>
    public class MallatDecomposition<T,U> : Segment<T> where U : Node<T>
    {
        private Instancer<U> instancer;
        /// <summary>
        /// Constructor
        /// Have to call Build(..) after constructor
        /// </summary>
        /// <param name="predecessor">Predecessor node</param>
        public MallatDecomposition(Node<T> predecessor) : base(1, predecessor)
        {
            this.instancer = new Instancer<U>();
        }
        /// <summary>
        /// Get size of result image
        /// If size cant be devided it has to be magnified
        /// </summary>
        /// <returns></returns>
        public Size GetRecalculatedSize() => adjustedSize;
        /// <summary>
        /// Build new level of decomposition
        /// </summary>
        /// <param name="currentSize">Curent size of image</param>
        /// <param name="predecesor">Input node</param>
        /// <returns>List of output nodes</returns>
        private Node<T>[] GenerateLevel(Size currentSize, Node<T> predecesor)
        {
            var wavelet11 = instancer.New(predecesor);
            currentSize.Width /= 2;
            var flipper11 = new ImageDiagonalFlipper<T>(currentSize, wavelet11[0]);
            var flipper12 = new ImageDiagonalFlipper<T>(currentSize, wavelet11[1]);
            var wavelet21 = instancer.New(flipper11);
            var wavelet22 = instancer.New(flipper12);
            currentSize.Height /= 2;
            var flipper21 = new ImageDiagonalFlipper<T>(currentSize, wavelet21[0]);
            var flipper22 = new ImageDiagonalFlipper<T>(currentSize, wavelet21[1]);
            var flipper23 = new ImageDiagonalFlipper<T>(currentSize, wavelet22[0]);
            var flipper24 = new ImageDiagonalFlipper<T>(currentSize, wavelet22[1]);
            return new[] { flipper21, flipper22, flipper23, flipper24 };
        }
        private int levels;
        Size adjustedSize;
        /// <summary>
        /// Build MallatDecomposition segment with given image size and level count
        /// </summary>
        /// <param name="imageSize"></param>
        /// <param name="levels"></param>
        /// <returns>This node</returns>
        public MallatDecomposition<T, U> Build(Size imageSize, int levels)
        {
            this.levels = levels;
            if (levels <= 0)
            {
                Logger.Error("Mallat Decompisiton can't have 0 or less levels");
                throw new ArgumentException("Mallat Decompisiton can't have 0 or less levels");
            }
            var outputStream = new Stack<Node<T>>();
            outputStream.Push(inputNodes[0]);
            adjustedSize = imageSize;
            adjustedSize.Width = (int)(Math.Ceiling(adjustedSize.Width / Math.Pow(2, levels)) * Math.Pow(2, levels));
            adjustedSize.Height = (int)(Math.Ceiling(adjustedSize.Height / Math.Pow(2, levels)) * Math.Pow(2, levels));
            Size tmpSize = adjustedSize;
            for (int level = 0; level < levels; level++)
            {
                Node<T> startNode = outputStream.Pop();
                Node<T>[] levelEndNodes = GenerateLevel(tmpSize, startNode);
                for (int i = levelEndNodes.Length - 1; i >= 0; i--)
                {
                    outputStream.Push(levelEndNodes[i]);
                }
                tmpSize.Width /= 2;
                tmpSize.Height /= 2;
            }
            outputNodes = outputStream.ToArray();
            return this;
        }
        public override void BuildSegment() { }
        /// <summary></summary>
        /// <returns>Name of MallatDecomposition segment</returns>
        public override string ToString()
        {
            var name = String.Format("Mallat.{0}.l{1}", typeof(U), levels);
            return name;
        }
    }
    /// <summary>
    /// Segment that calculate inverse operation that MallatDecomposition
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    /// <typeparam name="U">Lifting segment</typeparam>
    public class MallatDecompositionInverse<T, U> : Segment<T> where U : Node<T>
    {
        /// <summary>
        /// Node that recalculate index to get right data from one input
        /// </summary>
        /// <typeparam name="K">Type of data</typeparam>
        private class IndexRecalculator<K> : Node<K>
        {
            private Size areaSize;
            private Size imageSize;
            private int offset;
            // TODO dodelat komentar?
            public IndexRecalculator(Size areaSize, Size imageSize, int offset, Node<K> predecessor) : base(predecessor)
            {
                this.areaSize = areaSize;
                this.imageSize = imageSize;
                this.offset = offset;
            }

            public override Data<K> GetData(int dataNumber)
            {
                int x = dataNumber % areaSize.Width;
                int y = dataNumber / areaSize.Width;
                return Input[0].GetData(y * imageSize.Width + x + offset);
            }
        }
        private Instancer<U> instancer;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="predecessor">Predecessor node</param>
        public MallatDecompositionInverse(Node<T> predecessor) : base(1, predecessor)
        {
            this.instancer = new Instancer<U>();
        }
        /// <summary>
        /// Generate one level of inverse of MallatDecomposition
        /// </summary>
        /// <param name="currentSize">Size of area</param>
        /// <param name="predecessors">4 predecessors</param>
        /// <returns></returns>
        private Node<T> GenerateLevel(Size currentSize, params Node<T>[] predecessors)
        {
            var flipper11 = new ImageDiagonalFlipper<T>(currentSize, predecessors[0]);
            var flipper12 = new ImageDiagonalFlipper<T>(currentSize, predecessors[1]);
            var flipper13 = new ImageDiagonalFlipper<T>(currentSize, predecessors[2]);
            var flipper14 = new ImageDiagonalFlipper<T>(currentSize, predecessors[3]);
            var wavelet11 = instancer.New(flipper11, flipper12);
            var wavelet12 = instancer.New(flipper13, flipper14);
            currentSize.Width *= 2;
            var flipper21 = new ImageDiagonalFlipper<T>(currentSize, wavelet11);
            var flipper22 = new ImageDiagonalFlipper<T>(currentSize, wavelet12);
            var wavelet21 = instancer.New(flipper21, flipper22);
            return wavelet21;
        }
        /// <summary>
        /// Build MallatDecompositionInverse segment with given image size and level count
        /// </summary>
        /// <param name="imageSize"></param>
        /// <param name="levels"></param>
        /// <returns>This node</returns>
        public MallatDecompositionInverse<T,U> Build(Size imageSize, int levels)
        {
            if (levels <= 0)
            {
                Logger.Error("Mallat Decompisiton can't have 0 or less levels");
                throw new ArgumentException("Mallat Decompisiton can't have 0 or less levels");
            }
            Size adjustedSize = imageSize;
            adjustedSize.Width = (int)(Math.Ceiling(adjustedSize.Width / Math.Pow(2, levels)) * Math.Pow(2, levels));
            adjustedSize.Height = (int)(Math.Ceiling(adjustedSize.Height / Math.Pow(2, levels)) * Math.Pow(2, levels));
            int sections = 4 + ((levels - 1) * 3);
            var outputNodesStack = new Stack<Node<T>>();
            var recalculatorNodes = new Node<T>[sections];
            for (int i = 0; i < levels; i++)
            {
                adjustedSize.Width /= 2;
                adjustedSize.Height /= 2;
                outputNodesStack.Push(new IndexRecalculator<T>(adjustedSize, imageSize, (adjustedSize.Height * imageSize.Width + adjustedSize.Width), inputNodes[0]));
                outputNodesStack.Push(new IndexRecalculator<T>(adjustedSize, imageSize, (adjustedSize.Width), inputNodes[0]));
                outputNodesStack.Push(new IndexRecalculator<T>(adjustedSize, imageSize, (adjustedSize.Height * imageSize.Width), inputNodes[0]));
            }
            outputNodesStack.Push(new IndexRecalculator<T>(adjustedSize, imageSize, 0, inputNodes[0]));
            for (int i = 0; i < levels; i++)
            {
                outputNodesStack.Push(GenerateLevel(adjustedSize, outputNodesStack.Pop(), outputNodesStack.Pop(), outputNodesStack.Pop(), outputNodesStack.Pop()));
                adjustedSize.Width *= 2;
                adjustedSize.Height *= 2;
            }
            outputNodes[0] = outputNodesStack.Pop();
            return this;
        }
        public override void BuildSegment() { }
    }
}
