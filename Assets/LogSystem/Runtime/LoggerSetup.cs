using UnityEngine;

namespace ADONEGames.CustomDebugLogger
{
    public static class LoggerSetup
    {
        private static ILoggerEvent LoggerEventInstance { get; set; }

        /// <summary>
        /// LoggerEventの登録を行う
        /// </summary>
        /// <param name="loggerEventFactories">登録するLoggerEvent</param>
        public static void Initialize( params LoggerEventFactory[] loggerEventFactories )
        {
            LoggerEventInstance ??= new LoggerEventManager( loggerEventFactories );

            // Initialize()が呼ばれたら、Dispose()を登録する
            // 念の為、解除してから登録する
            // --------------------------
            // Register Dispose() when Initialize() is called.
            // For good measure, unregister before registering.
            Application.quitting -= Dispose;
            Application.quitting += Dispose;
        }

        private static void Dispose()
        {
            LoggerEventInstance.Dispose();
        }
    }
}
