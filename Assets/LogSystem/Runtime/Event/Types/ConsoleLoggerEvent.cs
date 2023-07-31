using System;

using UnityEngine;

using Object = UnityEngine.Object;

namespace ADONEGames.CustomDebugLogger
{
    /// <summary>
    /// <see cref="Debug"/>で生成された<see cref="ILogHandler"/>をラップする<see cref="ILoggerEvent"/>
    /// </summary>
    public class ConsoleLoggerEvent : AbstractLoggerEvent
    {
        /// <inheritdoc />
        public ConsoleLoggerEvent( ILogHandler originalDebugLogHandler ) : base( originalDebugLogHandler ) { }

        /// <summary>
        /// 元の <see cref="Debug"/>で生成された<see cref="ILogHandler"/>を使用してログを出力する
        /// </summary>
        /// <inheritdoc />
        public override void LogFormat( LogType logType, Object context, string format, params object[] args )
        {
            OriginalDebugLogHandler.LogFormat( logType, context, format, args );
        }

        /// <summary>
        /// 元の <see cref="Debug"/>で生成された<see cref="ILogHandler"/>を使用して例外を出力する
        /// </summary>
        /// <inheritdoc />
        public override void LogException( Exception exception, Object context )
        {
            OriginalDebugLogHandler.LogException( exception, context );
        }
    }
}