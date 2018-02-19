using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentForLifting
{
    /// <summary>
    /// Storage of parameters for network
    /// Can store many types of parameters
    /// </summary>
    public class ParameterStorage
    {
        private Dictionary<string, string> stringParameters;
        private Dictionary<string, byte> byteParameters;
        private Dictionary<string, sbyte> sbyteParameters;
        private Dictionary<string, short> shortParameters;
        private Dictionary<string, ushort> ushortParameters;
        private Dictionary<string, int> intParameters;
        private Dictionary<string, uint> uintParameters;
        private Dictionary<string, float> floatParameters;
        private Dictionary<string, double> doubleParameters;
        private Dictionary<string, decimal> decimalParameters;

        /// <summary>
        /// Get stored string parameter
        /// </summary>
        /// <param name="key">Parameter's key</param>
        /// <returns>Stored value</returns>
        public string GetStringParameter(string key) => stringParameters[key];
        /// <summary>
        /// Get stored byte parameter
        /// </summary>
        /// <param name="key">Parameter's key</param>
        /// <returns>Stored value</returns>
        public byte GetByteParameter(string key) => byteParameters[key];
        /// <summary>
        /// Get stored sbyte parameter
        /// </summary>
        /// <param name="key">Parameter's key</param>
        /// <returns>Stored value</returns>
        public sbyte GetSByteParameter(string key) => sbyteParameters[key];
        /// <summary>
        /// Get stored short parameter
        /// </summary>
        /// <param name="key">Parameter's key</param>
        /// <returns>Stored value</returns>
        public short GetShortParameter(string key) => shortParameters[key];
        /// <summary>
        /// Get stored ushort parameter
        /// </summary>
        /// <param name="key">Parameter's key</param>
        /// <returns>Stored value</returns>
        public ushort GetUShortParameter(string key) => ushortParameters[key];
        /// <summary>
        /// Get stored int parameter
        /// </summary>
        /// <param name="key">Parameter's key</param>
        /// <returns>Stored value</returns>
        public int GetIntParameter(string key) => intParameters[key];
        /// <summary>
        /// Get stored uint parameter
        /// </summary>
        /// <param name="key">Parameter's key</param>
        /// <returns>Stored value</returns>
        public uint GetUIntParameter(string key) => uintParameters[key];
        /// <summary>
        /// Get stored float parameter
        /// </summary>
        /// <param name="key">Parameter's key</param>
        /// <returns>Stored value</returns>
        public float GetFloatParameter(string key) => floatParameters[key];
        /// <summary>
        /// Get stored double parameter
        /// </summary>
        /// <param name="key">Parameter's key</param>
        /// <returns>Stored value</returns>
        public double GetDoubleParameter(string key) => doubleParameters[key];
        /// <summary>
        /// Get stored decimal parameter
        /// </summary>
        /// <param name="key">Parameter's key</param>
        /// <returns>Stored value</returns>
        public decimal GetDecimalParameter(string key) => decimalParameters[key];

        /// <summary>
        /// Store string parameter
        /// </summary>
        /// <param name="key">Parameter's key</param>
        /// <param name="value">Parameter's value</param>
        /// <returns>This ParameterStorage</returns>
        public ParameterStorage SetParameter(string key, string value)
        {
            if (stringParameters == null) stringParameters = new Dictionary<string, string>();
            stringParameters[key] = value;
            return this;
        }
        /// <summary>
        /// Store byte parameter
        /// </summary>
        /// <param name="key">Parameter's key</param>
        /// <param name="value">Parameter's value</param>
        /// <returns>This ParameterStorage</returns>
        public ParameterStorage SetParameter(string key, byte value)
        {
            if (byteParameters == null) byteParameters = new Dictionary<string, byte>();
            byteParameters[key] = value;
            return this;
        }
        /// <summary>
        /// Store sbyte parameter
        /// </summary>
        /// <param name="key">Parameter's key</param>
        /// <param name="value">Parameter's value</param>
        /// <returns>This ParameterStorage</returns>
        public ParameterStorage SetParameter(string key, sbyte value)
        {
            if (sbyteParameters == null) sbyteParameters = new Dictionary<string, sbyte>();
            sbyteParameters[key] = value;
            return this;
        }
        /// <summary>
        /// Store short parameter
        /// </summary>
        /// <param name="key">Parameter's key</param>
        /// <param name="value">Parameter's value</param>
        /// <returns>This ParameterStorage</returns>
        public ParameterStorage SetParameter(string key, short value)
        {
            if (shortParameters == null) shortParameters = new Dictionary<string, short>();
            shortParameters[key] = value;
            return this;
        }
        /// <summary>
        /// Store ushort parameter
        /// </summary>
        /// <param name="key">Parameter's key</param>
        /// <param name="value">Parameter's value</param>
        /// <returns>This ParameterStorage</returns>
        public ParameterStorage SetParameter(string key, ushort value)
        {
            if (ushortParameters == null) ushortParameters = new Dictionary<string, ushort>();
            ushortParameters[key] = value;
            return this;
        }
        /// <summary>
        /// Store int parameter
        /// </summary>
        /// <param name="key">Parameter's key</param>
        /// <param name="value">Parameter's value</param>
        /// <returns>This ParameterStorage</returns>
        public ParameterStorage SetParameter(string key, int value)
        {
            if (intParameters == null) intParameters = new Dictionary<string, int>();
            intParameters[key] = value;
            return this;
        }
        /// <summary>
        /// Store uint parameter
        /// </summary>
        /// <param name="key">Parameter's key</param>
        /// <param name="value">Parameter's value</param>
        /// <returns>This ParameterStorage</returns>
        public ParameterStorage SetParameter(string key, uint value)
        {
            if (uintParameters == null) uintParameters = new Dictionary<string, uint>();
            uintParameters[key] = value;
            return this;
        }
        /// <summary>
        /// Store float parameter
        /// </summary>
        /// <param name="key">Parameter's key</param>
        /// <param name="value">Parameter's value</param>
        /// <returns>This ParameterStorage</returns>
        public ParameterStorage SetParameter(string key, float value)
        {
            if (floatParameters == null) floatParameters = new Dictionary<string, float>();
            floatParameters[key] = value;
            return this;
        }
        /// <summary>
        /// Store double parameter
        /// </summary>
        /// <param name="key">Parameter's key</param>
        /// <param name="value">Parameter's value</param>
        /// <returns>This ParameterStorage</returns>
        public ParameterStorage SetParameter(string key, double value)
        {
            if (doubleParameters == null) doubleParameters = new Dictionary<string, double>();
            doubleParameters[key] = value;
            return this;
        }
        /// <summary>
        /// Store decimal parameter
        /// </summary>
        /// <param name="key">Parameter's key</param>
        /// <param name="value">Parameter's value</param>
        /// <returns>This ParameterStorage</returns>
        public ParameterStorage SetParameter(string key, decimal value)
        {
            if (decimalParameters == null) decimalParameters = new Dictionary<string, decimal>();
            decimalParameters[key] = value;
            return this;
        }
    }
}
