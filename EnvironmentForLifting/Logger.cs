namespace EnvironmentForLifting
{
    public  static class Logger
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("EnvironmentForLifting");
        public static void Debug(object message) { log.Debug(message); }
        public static void Debug(object message, System.Exception exception) { log.Debug(message, exception); }
        public static void Info(object message) { log.Info(message); }
        public static void Info(object message, System.Exception exception) { log.Info(message, exception); }
        public static void Warn(object message) { log.Warn(message); }
        public static void Warn(object message, System.Exception exception) { log.Warn(message, exception); }
        public static void Error(object message) { log.Error(message); }
        public static void Error(object message, System.Exception exception) { log.Error(message, exception); }
    }
}
