using UnityEngine;


namespace ADONEGames.CustomDebugLogger
{
    /// <summary>
    /// <see cref="AbstractLoggerEvent"/>インスタンスを生成するためのデリゲート
    /// </summary>
    public delegate AbstractLoggerEvent LoggerEventFactory( ILogHandler originalDebugLogHandler );
}
