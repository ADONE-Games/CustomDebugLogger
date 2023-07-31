namespace ADONEGames.CustomDebugLogger
{
    
    public static class LoggerSetup
    {
        private static ILoggerEvent LoggerEventInstance { get; set; }

        public static void Initialize( params LoggerEventFactory[] loggerEventFactories )
        {
            LoggerEventInstance ??= new LoggerEventManager( loggerEventFactories );
        }

        public static void Dispose()
        {
            LoggerEventInstance.Dispose();
        }
    }
}