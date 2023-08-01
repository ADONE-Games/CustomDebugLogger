using UnityEngine;

namespace ADONEGames.CustomDebugLogger
{
    
    public static class LoggerSetup
    {
        private static ILoggerEvent LoggerEventInstance { get; set; }

        public static void Initialize( params LoggerEventFactory[] loggerEventFactories )
        {
            LoggerEventInstance ??= new LoggerEventManager( loggerEventFactories );
            
            Application.quitting += LoggerSetup.Dispose;
        }

        private static void Dispose()
        {
            LoggerEventInstance.Dispose();
        }
    }
}