using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace EnvironmentForLifting
{
    /// <summary>
    /// Node that write image to PFM formated file
    /// PFM = Portable FloatMap Image Format
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public class PFMWriter<T> : Node<T>
    {
        private Size size;
        private float[][] data;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="size">Size of image</param>
        /// <param name="predecessor">Predecessor node</param>
        public PFMWriter(Size size, Node<T> predecessor) : base(predecessor)
        {
            this.size = size;
            this.DataLength = 3;
            this.data = new float[size.Height * size.Width][];
        }
        public override void Process()
        {
            var dataArray = new Data<T>[size.Width * size.Height];
            for (int dataNumber = 0; dataNumber < size.Height * size.Width; dataNumber++)
            {
                dataArray[dataNumber] = Input[0].GetData(dataNumber);
            }
            for (int dataNumber = 0; dataNumber < size.Height * size.Width; dataNumber++)
            {
                var tmpData = Data<T>.ToFloat(dataArray[dataNumber]);
                data[dataNumber] = new float[3];
                data[dataNumber][0] = tmpData[0];
                data[dataNumber][1] = tmpData[1];
                data[dataNumber][2] = tmpData[2];
            }
        }
        /// <summary>
        /// Save image to certain file
        /// </summary>
        /// <param name="filename">Name of file</param>
        public void Save(String filename)
        {
            BinaryWriter file;
            file = new BinaryWriter(new FileStream(filename, FileMode.OpenOrCreate));
            WriteHeader(file);
            for (int y = size.Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < size.Width; x++)
                {
                    var tmpData = data[y * size.Width + x];
                    file.Write(BitConverter.GetBytes(tmpData[0]));
                    file.Write(BitConverter.GetBytes(tmpData[1]));
                    file.Write(BitConverter.GetBytes(tmpData[2]));
                }
            }
            file.Dispose();
        }
        public override void ProcessAsync()
        {
            Input[0].SetDropOldData(false);
            var dataArray = new Task<Data<T>>[size.Width * size.Height];
            for (int dataNumber = 0; dataNumber < size.Height * size.Width; dataNumber++)
            {
                int i = dataNumber;
                dataArray[dataNumber] = Task.Run(() =>
                {
                    return Input[0].GetData(dataNumber);
                });
            }
            for (int dataNumber = 0; dataNumber < size.Height * size.Width; dataNumber++)
            {
                var tmpData = Data<T>.ToFloat(dataArray[dataNumber].Result);
                data[dataNumber] = new float[3];
                data[dataNumber][0] = tmpData[0];
                data[dataNumber][1] = tmpData[1];
                data[dataNumber][2] = tmpData[2];
            }
        }
        /// <summary>
        /// Write header information to file
        /// </summary>
        /// <param name="file">File where header should be written</param>
        private void WriteHeader(BinaryWriter file)
        {
            Write("PF", file);
            file.Write((byte)0x0a);
            Write(String.Format("{0} {1}", size.Width, size.Height), file);
            file.Write((byte)0x0a);
            Write("-1.0", file);
            file.Write((byte)0x0a);
        }
        /// <summary>
        /// Write some text to file
        /// </summary>
        /// <param name="text">Text to be written</param>
        /// <param name="file">File where text should be written</param>
        private void Write(String text, BinaryWriter file)
        {
            var characters = text.ToCharArray();
            foreach (var character in characters)
            {
                file.Write(character);
            }
        }
    }
    /// <summary>
    /// Node that read image from PFM formated file
    /// PFM = Portable FloatMap Image Format
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PFMReader<T> : Node<T>
    {
        private BinaryReader file;
        private Size size;
        private float[][] data;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filename">Name of file which should be read</param>
        public PFMReader(String filename) : base()
        {
            this.file = new BinaryReader(new FileStream(filename, FileMode.Open));
            ReadData();
        }
        /// <summary>
        /// Read and store data content
        /// </summary>
        private void ReadData()
        {
            string format = GetChunk();
            if (format == "PF")
            {
                var resolution = GetChunk().Split(' ');
                size = new Size(int.Parse(resolution[0]), int.Parse(resolution[1]));
                var littleEndian = float.Parse(GetChunk(), CultureInfo.InvariantCulture) < 0;
                data = new float[size.Height * size.Width][];
                for (int y = size.Height - 1; y >= 0 ; y--)
                {
                    for (int x = 0; x < size.Width; x++)
                    {
                        int dataNumber = y * size.Width + x;
                        data[dataNumber] = new float[3];
                        data[dataNumber][0] = GetNextFloat(littleEndian);
                        data[dataNumber][1] = GetNextFloat(littleEndian);
                        data[dataNumber][2] = GetNextFloat(littleEndian);
                    }
                }
            }
        }
        private String GetChunk()
        {
            byte character;
            StringBuilder stringBuilder = new StringBuilder();
            while ((character = file.ReadByte()) != 0x0a)
            {
                stringBuilder.Append((char)character);
            }
            return stringBuilder.ToString();
        }
        private float GetNextFloat(bool littleEndian)
        {
            byte[] bytes = file.ReadBytes(4);
            if (!littleEndian) bytes.Reverse();
            return BitConverter.ToSingle(bytes, 0);
        }

        public override Data<T> GetData(int dataNumber)
        {
            if (dataNumber < 0 || dataNumber >= size.Width * size.Width) return new ZeroData<T>(3);
            return Data<T>.FloatToT(new Data<float>(data[dataNumber]));
        }
    }
}
