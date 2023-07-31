using System;

using UnityEngine;


namespace ADONEGames.CustomDebugLogger
{
    /// <summary>
    /// <see cref="Debug"/>で生成された<see cref="ILogHandler"/>をラップする<see cref="ILoggerEvent"/>
    /// </summary>
    public interface ILoggerEvent : ILogHandler, IDisposable { }
}
