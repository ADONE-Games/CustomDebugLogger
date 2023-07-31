using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using UnityEngine;

using Object = UnityEngine.Object;

namespace ADONEGames.CustomDebugLogger
{
    /// <summary>
    /// ファイルにログを出力する<see cref="ILoggerEvent"/>
    /// </summary>
    public class FileLoggerEvent : AbstractLoggerEvent
    {
        /// <summary>
        /// ログの種類とその名前の対応表
        /// </summary>
        private static readonly Dictionary<LogType, string> LogTypeNames = new() {
                                                                                     { LogType.Error, "Error" },
                                                                                     { LogType.Assert, "Assert" },
                                                                                     { LogType.Warning, "Warning" },
                                                                                     { LogType.Log, "Log" },
                                                                                     { LogType.Exception, "Exception" }
                                                                                 };

        /// <summary>
        /// 書き込み用のStringBuilderの初期容量
        /// </summary>
        private const int WriteStringCapacitySize = 1024; // 1KB


        #if UNITY_EDITOR
        /// <summary>
        /// ログファイルの保存先ディレクトリのパス
        /// </summary>
        private readonly string _logDirectoryPath = $"{Application.dataPath}/../Logs";

        #else
        /// <summary>
        /// ログファイルの保存先ディレクトリのパス
        /// </summary>
        private readonly string _logDirectoryPath = $"{Application.persistentDataPath}/Logs";

        #endif

        /// <summary>
        /// 書き込み用のキュー
        /// </summary>
        private ConcurrentQueue<string> _writerQueue = new();

        /// <summary>
        /// 書き込み用のStringBuilder
        /// </summary>
        [ThreadStatic] private static StringBuilder _writerString;

        /// <summary>
        /// 書き込み用のStringBuilderのインスタンスを取得する
        /// </summary>
        private static StringBuilder WriterString
        {
            get
            {
                _writerString ??= new StringBuilder( WriteStringCapacitySize );

                _writerString.Capacity = _writerString.Capacity > WriteStringCapacitySize ? WriteStringCapacitySize : _writerString.Capacity;
                _writerString.Length   = 0;

                return _writerString;
            }
        }

        /// <summary>
        /// 書き込みタスクのCancellationTokenSource
        /// </summary>
        private CancellationTokenSource _writerTaskCancellationTokenSource = new();

        /// <summary>
        /// 書き込み処理のサブスレットタスク
        /// </summary>
        private Task _writerTask;

        /// <inheritdoc /> 
        public FileLoggerEvent( ILogHandler originalDebugLogHandler ) : base( originalDebugLogHandler )
        {
            _writerTask = Task.Run( () => WriterTask( _writerTaskCancellationTokenSource.Token ) );

            _writerTask.ContinueWith( task => {
                                          if( task.IsCompletedSuccessfully || task.IsCanceled )
                                              return;

                                          OriginalDebugLogHandler.LogFormat( LogType.Error, null, "{0}", $"[FileLoggerEvent] WriterTask is failed. {task.Exception}" );
                                      } );
        }

        /// <summary>
        /// ログメッセージを書き込みキューに追加する
        /// </summary>
        /// <inheritdoc />
        public override void LogFormat( LogType logType, Object context, string format, params object[] args )
        {
            if( _writerTask.IsCompleted )
                return;

            StringBuilder writerString = WriterString;
            writerString.Clear();
            writerString.AppendFormat( "[{0}]:[{1}]:", LogTypeNames[ logType ], DateTime.Now.ToString( "yyyy/MM/dd HH:mm:ss 'UTC'zz" ) );
            writerString.AppendFormat( format,         args );
            writerString.AppendLine();

            _writerQueue.Enqueue( writerString.ToString() );

            writerString.Clear();
            writerString.Capacity = writerString.Capacity > WriteStringCapacitySize ? WriteStringCapacitySize : writerString.Capacity;
            writerString.Length   = 0;
        }

        /// <summary>
        /// 例外メッセージを書き込みキューに追加する
        /// </summary>
        /// <inheritdoc />
        public override void LogException( Exception exception, Object context )
        {
            if( _writerTask.IsCompleted )
                return;

            StringBuilder writerString = WriterString;
            writerString.Clear();
            writerString.AppendFormat( "[{0}]:[{1}]:", LogTypeNames[ LogType.Exception ], DateTime.Now.ToString( "yyyy/MM/dd HH:mm:ss 'UTC'zz" ) );
            writerString.Append( exception.Message );
            writerString.Append( exception.StackTrace );
            writerString.AppendLine();

            _writerQueue.Enqueue( writerString.ToString() );

            writerString.Clear();
            writerString.Capacity = writerString.Capacity > WriteStringCapacitySize ? WriteStringCapacitySize : writerString.Capacity;
            writerString.Length   = 0;
        }

        /// <summary>
        /// テキストに書き込むタスク
        /// 例外が発生する可能性があるので、try-catchで囲むこと。
        /// もしくはtry-catchに相当する処理をすること。
        /// </summary>
        /// <param name="cancellationToken">終了判定させる<see cref="CancellationToken"/></param>
        private void WriterTask( CancellationToken cancellationToken )
        {
            LogFileInitialize();

            string timeStamp   = DateTime.Now.ToString( "yyyyMMddHHmmss" );
            string logFilePath = Path.Combine( _logDirectoryPath, $"{timeStamp}.log" );


            using FileStream fileStream = File.Create( logFilePath );

            try
            {
                while( !cancellationToken.IsCancellationRequested )
                {
                    if( _writerQueue.TryDequeue( out var message ) )
                    {
                        byte[] messageBytes = Encoding.UTF8.GetBytes( message );
                        fileStream.Write( messageBytes, 0, messageBytes.Length );
                    }
                    else
                    {
                        fileStream.Flush();
                        Thread.Sleep( 1 );
                    }
                }
            }
            catch( Exception )
            {
                fileStream.Flush();
                throw;
            }
        }

        /// <summary>
        /// ログファイルを初期化する
        /// </summary>
        private void LogFileInitialize()
        {
            if( !Directory.Exists( _logDirectoryPath ) )
            {
                Directory.CreateDirectory( _logDirectoryPath );
            }

            string[] logFiles = Directory.GetFiles( _logDirectoryPath, "*.log" );

            foreach( var logFile in logFiles )
            {
                File.Delete( logFile );
            }
        }

        /// <summary>
        /// Dispose this instance.
        /// </summary>
        public override void Dispose()
        {
            _writerTaskCancellationTokenSource.Cancel();
            _writerTaskCancellationTokenSource.Dispose();
            _writerTaskCancellationTokenSource = null;

            try
            {
                _writerTask.Wait();
            }
            catch( Exception )
            {
                // ignored
            }
            finally
            {
                _writerTask.Dispose();
                _writerTask = null;
            }


            _writerQueue.Clear();
            _writerQueue = null;

            base.Dispose();
        }
    }
}
