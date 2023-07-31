using System;

using UnityEngine;

using Object = UnityEngine.Object;

namespace ADONEGames.CustomDebugLogger
{
    /// <summary>
    /// <see cref="Debug"/>で生成された<see cref="ILogHandler"/>をラップするための抽象クラス
    /// </summary>
    public abstract class AbstractLoggerEvent : ILoggerEvent
    {
        /// <summary>
        /// 元の <see cref="Debug"/>で生成された<see cref="ILogHandler"/>
        /// </summary>
        protected ILogHandler OriginalDebugLogHandler { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="originalDebugLogHandler">元の<see cref="Debug"/>で生成された<see cref="ILogHandler"/></param>
        protected AbstractLoggerEvent( ILogHandler originalDebugLogHandler )
        {
            OriginalDebugLogHandler = originalDebugLogHandler;
        }

        /// <inheritdoc/>
        public abstract void LogFormat( LogType logType, Object context, string format, params object[] args );

        /// <inheritdoc/>
        public abstract void LogException( Exception exception, Object context );

        /// <inheritdoc/>
        public virtual void Dispose()
        {
            OriginalDebugLogHandler = null;
        }
    }
}