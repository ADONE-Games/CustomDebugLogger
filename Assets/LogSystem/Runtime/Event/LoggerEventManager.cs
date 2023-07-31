using System;

using UnityEngine;

using Object = UnityEngine.Object;

namespace ADONEGames.CustomDebugLogger
{
    /// <summary>
    /// <see cref="LoggerEventFactory"/>delegateで生成される<see cref="AbstractLoggerEvent"/>を継承した<see cref="ILoggerEvent"/>を管理するクラス
    /// </summary>
    internal sealed class LoggerEventManager : ILoggerEvent
    {
        /// <summary>
        /// 元の <see cref="Debug"/>で生成された<see cref="ILogHandler"/>
        /// </summary>
        private readonly ILogHandler _originalDebugLogHandler;

        /// <summary>
        /// <see cref="LoggerEventFactory"/>delegateで生成された<see cref="AbstractLoggerEvent"/>を継承した<see cref="ILoggerEvent"/>の配列
        /// </summary>
        private ILoggerEvent[] _loggerEvents;

        /// <summary>
        /// 各イベントのログ出力アクション
        /// </summary>
        private event LogFormatEvent logFormatEvent;

        /// <summary>
        /// 各イベントの例外出力アクション
        /// </summary>
        private event LogExceptionEvent logExceptionEvent;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="loggerEventFactories"><see cref="AbstractLoggerEvent"/>を生成するデリゲートオプション</param>
        public LoggerEventManager( params LoggerEventFactory[] loggerEventFactories )
        {
            _originalDebugLogHandler = Debug.unityLogger.logHandler;

            Debug.unityLogger.logHandler = this;

            _loggerEvents = new ILoggerEvent[loggerEventFactories.Length];

            for( int i = 0; i < loggerEventFactories.Length; i++ )
            {
                _loggerEvents[ i ] =  loggerEventFactories[ i ]( _originalDebugLogHandler );
                logFormatEvent     += _loggerEvents[ i ].LogFormat;
                logExceptionEvent  += _loggerEvents[ i ].LogException;
            }
        }

        /// <summary>
        /// 生成された<see cref="AbstractLoggerEvent"/>の<see cref="LogFormatEvent"/>を実行する
        /// </summary>
        /// <inheritdoc />
        public void LogFormat( LogType logType, Object context, string format, params object[] args ) => logFormatEvent?.Invoke( logType, context, format, args );

        /// <summary>
        /// 生成された<see cref="AbstractLoggerEvent"/>の<see cref="LogExceptionEvent"/>を実行する
        /// </summary>
        /// <inheritdoc />
        public void LogException( Exception exception, Object context ) => logExceptionEvent?.Invoke( exception, context );

        /// <inheritdoc />
        public void Dispose()
        {
            Debug.unityLogger.logHandler = _originalDebugLogHandler;

            foreach( var loggerEvent in _loggerEvents )
            {
                loggerEvent.Dispose();
            }

            _loggerEvents = null;
        }


        /// <summary>
        /// <see cref="LogFormatEvent"/>のデリゲート
        /// </summary>
        private delegate void LogFormatEvent( LogType logType, Object context, string format, params object[] args );


        /// <summary>
        /// <see cref="LogExceptionEvent"/>のデリゲート
        /// </summary>
        private delegate void LogExceptionEvent( Exception exception, Object context );
    }
}
