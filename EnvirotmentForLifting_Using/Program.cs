using System;
using System.Drawing;
using EnvironmentForLifting;

namespace EnvirotmentForLifting_Using
{
    class Program
    {
        static void Main(string[] args)
        {
            //Experiment1OperationPerformance();
            //Experiment2TypePerformence();
            //Experiment4DataInit();
            //Experiment5Taskv2Performence();
            //Experiment6Entropy();
            Experiment7();
        }
        static void Experiment1OperationPerformance()
        {
            var count = (int)Math.Pow(10, 9);
            Logger.Info("Performance Type Demo");
            Logger.Info($"Count:\t{count}");
            Logger.Info("MOPS = Million Operation Per Second");
            var nonGeneric = PerformaceTestDouble(count);
            var generic = PerformenceTypeTest<double>(count);
            var genericDynamic = PerformaceTestDoubleDynamic(count);
        }
        static void Experiment2TypePerformence()
        {
            var count = (int)Math.Pow(10, 9);
            Logger.Info("Performance Type Demo");
            Logger.Info($"Count:\t{count}");
            Logger.Info("MOPS = Million Operation Per Second");
            PerformenceTypeTest<byte>(count);
            PerformenceTypeTest<sbyte>(count);
            PerformenceTypeTest<short>(count);
            PerformenceTypeTest<ushort>(count);
            PerformenceTypeTest<int>(count);
            PerformenceTypeTest<uint>(count);
            PerformenceTypeTest<long>(count);
            PerformenceTypeTest<ulong>(count);
            PerformenceTypeTest<float>(count);
            PerformenceTypeTest<double>(count);
            PerformenceTypeTest<decimal>(count);
        }
        static void Experiment4DataInit()
        {
            int count = (int)Math.Pow(10, 9);
            Logger.Info($"Data count: {count}");
            Logger.Info("new Data<double>:");
            var sw = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < count; i++)
            {
                var data = new Data<double>(new[] { 5d });
            }
            Logger.Info($"Time:\t{sw.Elapsed}");
            Logger.Info("Task.FromResult(...):");
            sw.Restart();
            for (int i = 0; i < count; i++)
            {
                var data = System.Threading.Tasks.Task.FromResult(new Data<double>(new[] { 5d }));
            }
            Logger.Info($"Time:\t{sw.Elapsed}");
            Logger.Info("new Task<Data<double>>(...):");
            sw.Restart();
            for (int i = 0; i < count; i++)
            {
                var data = new System.Threading.Tasks.Task<Data<double>>(() => new Data<double>(new[] { 5d }));
            }
            Logger.Info($"Time:\t{sw.Elapsed}");
        }
        static void Experiment5Taskv2Performence()
        {
            int level = 2;
            string name = "lenna";
            Logger.Info(name);
            string inputName = $"../../../test_images/{name}.png";
            Bitmap input = new Bitmap(inputName);
            Bitmap output = new Bitmap(input.Width, input.Height, input.PixelFormat);

            var imageReader = new BitmapReader<int>(input);
            var mallat = new MallatDecomposition<int, NineSevenBiortogonalInteger<int>>(imageReader).Build(input.Size, level);
            Logger.Info(mallat.ToString());
            var calculator = new MallatEntropyEvaluator<int>(input.Size, level, mallat);
            var mallatCollector = new MallatMerger<int>(input.Size, level, mallat);
            var imageWriter = new BitmapWriter<int>(new Bitmap(input.Width, input.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb), "", mallatCollector);
            string outputName = $"{inputName}_{mallat.ToString()}.png";
            imageWriter.SetImageName(outputName);
            calculator.DataLength = 3;

            var sw = System.Diagnostics.Stopwatch.StartNew();
            Logger.Info("Sync:");
            imageWriter.Process();
            Logger.Info($"Time:\t{sw.Elapsed}");
            sw.Restart();
            Logger.Info("Async v2:");
            imageWriter.ProcessAsync();
            Logger.Info($"Time:\t{sw.Elapsed}");
        }
        static void Experiment6Entropy()
        {
            for (int level = 1; level <= 2; level++)
            {
                foreach (var name in new[] { "barbara", "boat", "fingerprint", "flinstones", "house", "lena", "peppers256" })
                {
                    ExperimentEntropy(name, level);
                }
            }
        }


        static long PerformenceTypeTest<T>(int count)
        {
            var data1 = new Data<T>(new T[] { default(T) });
            var data2 = new Data<T>(new T[] { default(T) });
            Logger.Info($"Type:\t{typeof(T)}");
            Data<T> result;
            var sw = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < count; i++)
            {
                result = data1 + data2;
                //result = data1 / 42;
            }
            long ops = (long)(count / sw.Elapsed.TotalSeconds);
            Logger.Info($"Speed:\t{ops / 1000000} MOPS");
            return ops;
        }
        static long PerformaceTestDouble(int count)
        {
            var data1 = new DataDouble(new double[] { default(double) });
            var data2 = new DataDouble(new double[] { default(double) });
            Logger.Info("Type:\t NonGeneric Double");
            DataDouble result;
            var sw = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < count; i++)
            {
                result = data1 + data2;
                //result = data1 / 42;
            }
            long ops = (long)(count / sw.Elapsed.TotalSeconds);
            Logger.Info($"Speed:\t{ops / 1000000} MOPS");
            return ops;
        }
        static long PerformaceTestDoubleDynamic(int count)
        {
            var data1 = new DataDoubleDynamic(new double[] { default(double) });
            var data2 = new DataDoubleDynamic(new double[] { default(double) });
            Logger.Info("Type:\t Generic Double Dynamic");
            DataDoubleDynamic result;
            var sw = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < count; i++)
            {
                result = data1 + data2;
                //result = data1 / 42;
            }
            long ops = (long)(count / sw.Elapsed.TotalSeconds);
            Logger.Info($"Speed:\t{ops / 1000000} MOPS");
            return ops;
        }
        static void TesterRepeateDemo()
        {
            var delayer = new Delayer<int>().SetDelayTimeInMiliseconds(50);
            var printer = new Print<int>(delayer);
            printer.OldDataKeepCount = 2;
            printer.SetTextWriter(System.IO.TextWriter.Null);

            var tester = new RepeateTester<int>(printer).SetShowProgerss(true).SetRepetitionCount(21);
            tester.RunSync();
            tester.RunAsync();
        }
        static void TesterParameterDemo()
        {
            string inputName = "../../../test_images/lenna.png";
            Bitmap input = new Bitmap(inputName);
            var imageReader = new BitmapReader<float>(input);
            var mallat =  new MallatDecomposition<float, Haar<float>>(imageReader).Build(input.Size, 1);
            var imageWriter = new BitmapWriter<float>(new Bitmap(input.Size.Width, input.Size.Height), "", mallat);
            var tester = new ParameterTester<float>(imageWriter)
                .SetNextParameters(new ParameterStorage().SetParameter("outputName", "../lenna_1.png"))
                .SetNextParameters(new ParameterStorage().SetParameter("outputName", "../lenna_2.png"))
                .SetNextParameters(new ParameterStorage().SetParameter("outputName", "../lenna_3.png"));
            tester.RunSync();
        }
        static void ExperimentEntropy(string name, int level)
        {
            string folder = "../../../test_images";
            string inputName = $"{folder}/{name}.png";
            Logger.Info(inputName);
            Bitmap input = new Bitmap(inputName);
            Bitmap output = new Bitmap(input.Width, input.Height, input.PixelFormat);

            var tester = new MallatTester<int>((GetMallat) =>
            {
                var imageReader = new BitmapReader<int>(input);
                var mallat = GetMallat(imageReader);
                Logger.Info(mallat.ToString());
                var calculator = new MallatEntropyEvaluator<int>(input.Size, level, (Segment<int>)mallat);
                var mallatCollector = new MallatMerger<int>(input.Size, level, (Segment<int>)mallat);
                var imageWriter = new BitmapWriter<int>(new Bitmap(input.Width, input.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb), "", mallatCollector);
                string outputName = $"{folder}/{level}/{name}_{mallat.ToString()}.png";
                imageWriter.SetImageName(outputName);
                calculator.DataLength = 3;
                return new Node<int>[] { imageWriter, calculator };
            });

            tester.AddNewMallat((predecessor) => { return new MallatDecomposition<int, TwoTwoInterpolatingInteger<int>>(predecessor).Build(input.Size, level); });
            tester.AddNewMallat((predecessor) => { return new MallatDecomposition<int, FourTwoInterpolatingInteger<int>>(predecessor).Build(input.Size, level); });
            tester.AddNewMallat((predecessor) => { return new MallatDecomposition<int, TwoFourInterpolatingInteger<int>>(predecessor).Build(input.Size, level); });
            tester.AddNewMallat((predecessor) => { return new MallatDecomposition<int, FourFourInterpolatingInteger<int>>(predecessor).Build(input.Size, level); });
            tester.AddNewMallat((predecessor) => { return new MallatDecomposition<int, TwoPlusTwoTwoInteger<int>>(predecessor).Build(input.Size, level); });
            tester.AddNewMallat((predecessor) => { return new MallatDecomposition<int, DaubechiesWaveletD4Integer<int>>(predecessor).Build(input.Size, level); });
            tester.AddNewMallat((predecessor) => { return new MallatDecomposition<int, NineSevenBiortogonalInteger<int>>(predecessor).Build(input.Size, level); });
            tester.AddNewMallat((predecessor) => { return new MallatDecomposition<int, BaseWavelet<int>>(predecessor).Build(input.Size, level); });

            var sw = System.Diagnostics.Stopwatch.StartNew();
            tester.RunAsync();
            Logger.Info($"Time:\t{sw.Elapsed}");
        }
        static void Experiment7()
        {
            string name = "barbara";
            string folder = "../../../test_images";
            string inputName = $"{folder}/{name}.png";
            Logger.Info(inputName);
            Bitmap input = new Bitmap(inputName);
            Bitmap output1 = new Bitmap(input.Width/2, input.Height, input.PixelFormat);
            Bitmap output2 = new Bitmap(input.Width/2, input.Height, input.PixelFormat);

            var imageReader = new BitmapReader<int>(input);
            var d4 = new DaubechiesWaveletD4Integer<int>(imageReader);
            var imageWriter1 = new BitmapWriter<int>(output1, $"{folder}/Experiment7_barbara_0.png", d4[0]);
            var imageWriter2 = new BitmapWriter<int>(output1, $"{folder}/Experiment7_barbara_1.png", d4[1]);

            imageWriter1.ProcessAsync();
            imageWriter2.ProcessAsync();
        }

    }
}
