using System;

namespace EnvironmentForLifting
{
    /// <summary>
    /// Class that call constructor of given type with specific parameters
    /// </summary>
    /// <typeparam name="T">Type need to be constructed</typeparam>
    public class Instancer<T>
    {
        /// <summary>
        /// Call constructor with parameter
        /// </summary>
        /// <param name="parameters">Constucotr parameters</param>
        /// <returns>Instance of type</returns>
        public T New(params object[] parameters)
        {
            return (T)Activator.CreateInstance(typeof(T), parameters);
        }
    }
}
