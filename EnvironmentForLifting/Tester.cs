using System;
using System.Collections.Generic;

namespace EnvironmentForLifting
{
    /// <summary>
    /// Abstract class providing base of tester
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public abstract class Tester<T>
    {
        protected int progressWidth;
        protected Node<T>[] endNodes;
        protected bool showProgress;
        protected int cursorTop;
        /// <summary>
        /// Constructor that store last nodes of compute network
        /// </summary>
        /// <param name="endNodes">Last nodes of netowrk</param>
        public Tester(params Node<T>[] endNodes)
        {
            this.endNodes = endNodes;
            this.showProgress = false;
            this.progressWidth = 20;
        }
        /// <summary>
        /// Set if tester should show progress on console or not
        /// </summary>
        /// <param name="showProgress">Bool value if progress should be shown</param>
        protected void SetShowProgerssParameters(bool showProgress)
        {
            this.showProgress = showProgress;
            cursorTop = Console.CursorTop;
        }
        /// <summary>
        /// Method that print progress by given progress
        /// </summary>
        /// <param name="count">Number of processed parts</param>
        /// <param name="total">Number of all parts</param>
        protected void PrintProgress(int count, int total)
        {
            if (!showProgress) return;
            if (count % (total / progressWidth) == 0)
            {
                double progress = count / (double)total;
                int n = (int)(progress * progressWidth);
                //TODO: Edit to work with logger
                //Console.SetCursorPosition(0, cursorTop);
                //Console.WriteLine("|{0}{1}{2}|", new String('-', n), n!=progressWidth?">":"-", new String(' ', progressWidth - n));
            }
        }
        /// <summary>
        /// Start tester synchronously 
        /// </summary>
        public virtual void RunSync() { }
        /// <summary>
        /// Start tester synchronously 
        /// </summary>
        public virtual void RunAsync() { }
    }
    /// <summary>
    /// Tester where is possible to set diferent parameters before each start
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public class ParameterTester<T> : Tester<T>
    {
        List<ParameterStorage> parameterList;
        public ParameterTester(params Node<T>[] endNodes) : base(endNodes)
        {
            this.parameterList = new List<ParameterStorage>();
        }
        /// <summary>
        /// Set list of parameters
        /// </summary>
        /// <param name="parameterList">List of parameters</param>
        /// <returns>This ParameterTester</returns>
        public ParameterTester<T> SetParameterList(List<ParameterStorage> parameterList)
        {
            this.parameterList = parameterList;
            return this;
        }
        /// <summary>
        /// Add ParameterStorage to parameter list
        /// </summary>
        /// <param name="parameters">One ParameterStorage</param>
        /// <returns>This ParameterTester</returns>
        public ParameterTester<T> SetNextParameters(ParameterStorage parameters)
        {
            this.parameterList.Add(parameters);
            return this;
        }
        /// <summary>
        /// Check if list of parameters is not empty
        /// </summary>
        private void ParameterCheck()
        {
            if (parameterList == null)
            {
                Logger.Error("Can't start computing because the parameterList has not been set.");
                throw new ArgumentException("Can't start computing because the parameterList has not been set.");
            }
        }
        public override void RunSync()
        {
            ParameterCheck();
            foreach (var parameter in parameterList)
            {
                foreach (var endNodes in endNodes)
                {
                    endNodes.Parameters = parameter;
                }
                foreach (var endNodes in endNodes)
                {
                    endNodes.Process();
                }
                foreach (var endNodes in endNodes)
                {
                    endNodes.Reset();
                }
            }
        }
        public override void RunAsync()
        {
            ParameterCheck();
            foreach (var parameter in parameterList)
            {
                foreach (var endNodes in endNodes)
                {
                    endNodes.Parameters = parameter;
                }
                foreach (var endNodes in endNodes)
                {
                    endNodes.ProcessAsync();
                }
                foreach (var endNodes in endNodes)
                {
                    endNodes.Reset();
                }
            }
        }
        /// <summary>
        /// Set if progress should be shown
        /// </summary>
        /// <param name="showProgress">Bool value</param>
        /// <returns>This ParameterTester</returns>
        public ParameterTester<T> SetShowProgress(bool showProgress)
        {
            SetShowProgerssParameters(showProgress);
            return this;
        }
    }
    /// <summary>
    /// Tester that run same network multiple times
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public class RepeateTester<T> : Tester<T>
    {
        int repetitionCount;
        public RepeateTester(params Node<T>[] endNodes) : base(endNodes)
        {
            this.repetitionCount = 0;
        }
        /// <summary>
        /// Set how meny times of repetition should be run
        /// </summary>
        /// <param name="repetitionCount">Number of repetition</param>
        /// <returns>This RepeateTester</returns>
        public RepeateTester<T> SetRepetitionCount(int repetitionCount)
        {
            this.repetitionCount = repetitionCount;
            return this;
        }
        /// <summary>
        /// Check if number of repetition is not less then zero
        /// </summary>
        private void ParameterCheck()
        {
            if (repetitionCount < 0)
            {
                Logger.Error("Repetition can't be negativ number.");
                throw new ArgumentException("Repetition can't be negativ number.");
            }
        }
        /// <summary>
        /// Store cursor position
        /// </summary>
        private void PrepareCurosr()
        {
            cursorTop = Console.CursorTop;
        }
        public override void RunSync()
        {
            ParameterCheck();
            PrepareCurosr();
            for (int repetition = 0; repetition < repetitionCount; repetition++)
            {
                PrintProgress(repetition, repetitionCount);
                foreach (var endNode in endNodes)
                {
                    endNode.Process();
                }
                foreach (var endNode in endNodes)
                {
                    endNode.Reset();
                }
            }
            PrintProgress(repetitionCount, repetitionCount);
        }
        public override void RunAsync()
        {
            ParameterCheck();
            PrepareCurosr();
            for (int repetition = 0; repetition < repetitionCount; repetition++)
            {
                PrintProgress(repetition, repetitionCount);
                foreach (var endNode in endNodes)
                {
                    endNode.ProcessAsync();
                }
                foreach (var endNode in endNodes)
                {
                    endNode.Reset();
                }
            }
            PrintProgress(repetitionCount, repetitionCount);
        }
        /// <summary>
        /// Set if progress should be shown
        /// </summary>
        /// <param name="showProgress">Bool value</param>
        /// <returns>This ParameterTester</returns>
        public RepeateTester<T> SetShowProgerss(bool showProgress)
        {
            SetShowProgerssParameters(showProgress);
            return this;
        }
    }
    /// <summary>
    /// Tester that each run build diferent network with given mallat
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public class MallatTester<T> : Tester<T>
    {
        private List<GetMallatDelegate> mallats = new List<GetMallatDelegate>();
        private BuildNetworkDelegate buildNetwork;
        /// <summary>
        /// Constructor where is set how the network will be builded
        /// </summary>
        /// <param name="buildNetwork">Delegate that describing how the network will be builded</param>
        public MallatTester(BuildNetworkDelegate buildNetwork) : base()
        {
            this.buildNetwork = buildNetwork;
        }
        public delegate Node<T> GetMallatDelegate(Node<T> predecessor);
        public delegate Node<T>[] BuildNetworkDelegate(GetMallatDelegate mallat);
        /// <summary>
        /// Add new delegate that describes how to get mallat segment
        /// </summary>
        /// <param name="mallat">Delegate that describes how to get mallat segment</param>
        public void AddNewMallat(GetMallatDelegate mallat)
        {
            mallats.Add(mallat);
        }
        public override void RunSync()
        {
            foreach (var mallat in mallats)
            {
                var endNodes = buildNetwork(mallat);
                foreach (var endNode in endNodes)
                {
                    endNode.Process();
                }
                foreach (var endNode in endNodes)
                {
                    endNode.Reset();
                }
            }
        }
        public override void RunAsync()
        {
            foreach (var mallat in mallats)
            {
                var endNodes = buildNetwork(mallat);
                foreach (var endNode in endNodes)
                {
                    endNode.ProcessAsync();
                }
                foreach (var endNode in endNodes)
                {
                    endNode.Reset();
                }
            }
        }
    }
}
