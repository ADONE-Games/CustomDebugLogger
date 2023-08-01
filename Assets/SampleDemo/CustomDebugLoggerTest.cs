using System;

using ADONEGames.CustomDebugLogger;

using UnityEngine;

namespace Scenes
{
    public class CustomDebugLoggerTest : MonoBehaviour
    {
        private void Start()
        {
            LoggerSetup.Initialize( handler => new ConsoleLoggerEvent( handler ), handler => new FileLoggerEvent( handler ) );
        }

        // MonoBehaviourを使わない場合は、こんな方法もある
        // --------------------------
        // Here's another method you can use if you're not using MonoBehaviour.
        // [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSplashScreen )]
        // private static void Initialize()
        // {
        //     LoggerSetup.Initialize( handler => new ConsoleLoggerEvent( handler ), handler => new FileLoggerEvent( handler ) );
        // }
    }
}
