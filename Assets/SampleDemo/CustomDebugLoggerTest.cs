using ADONEGames.CustomDebugLogger;

using UnityEngine;

namespace Scenes
{
    public static class CustomDebugLoggerTest
    {
        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSplashScreen )]
        private static void Initialize()
        {
            LoggerSetup.Initialize( handler => new ConsoleLoggerEvent( handler ), handler => new FileLoggerEvent( handler ) );

            Application.quitting += LoggerSetup.Dispose;
        }
    }
}
