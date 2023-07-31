using System;
using System.Runtime.CompilerServices;
using System.Text;

using UnityEngine;

namespace ADONEGames.CustomDebugLogger
{
    /// <summary>
    /// Logger's abstract base class
    /// </summary>
    /// <typeparam name="T">Logger type</typeparam>
    public abstract class AbstractLoggerController<T> where T : AbstractLoggerController<T>, new()
    {
        /// <summary>
        /// Type Constraint Instance
        /// </summary>
        private static T _instance;


        /// <summary>
        /// <inheritdoc cref="_instance"/>
        /// create as needed
        /// </summary>
        private static T Instance => _instance ??= new T();

        /// <summary>
        /// Log format buffer
        /// </summary>
        private StringBuilder _stringBuffer;

        /// <summary>
        /// <inheritdoc cref="_stringBuffer"/>
        /// create as needed
        /// </summary>
        private StringBuilder StringBuffer => _stringBuffer ??= new StringBuilder( capacity: 128 );

        /// <summary>
        /// Log Prefix
        /// </summary>
        protected abstract string Prefix { get; }


        /// <summary>
        /// normal log message
        /// </summary>
        /// <param name="log">Message displayed</param>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void Log( string log ) => Log( log, Color.white );

        /// <summary>
        /// <inheritdoc cref="Log(string)"/>
        /// </summary>
        /// <param name="log">Message displayed</param>
        /// <param name="logColor">Displayed message color</param>
        public static void Log( string log, Color32 logColor )
        {
            var buffer = Instance.StringBuffer.Clear();

            SetupLog( buffer, log, logColor );

            Debug.Log( buffer.ToString() );

            buffer.Clear();
            buffer.Capacity = 128;
        }

        /// <summary>
        /// warning log message
        /// </summary>
        /// <param name="log">Message displayed</param>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void LogWarning( string log ) => LogWarning( log, Color.yellow );

        /// <summary>
        /// <inheritdoc cref="LogWarning(string)"/>
        /// </summary>
        /// <param name="log">Message displayed</param>
        /// <param name="logColor">Displayed message color</param>
        public static void LogWarning( string log, Color32 logColor )
        {
            var buffer = Instance.StringBuffer.Clear();

            SetupLog( buffer, log, logColor );

            Debug.LogWarning( buffer.ToString() );

            buffer.Clear();
            buffer.Capacity = 128;
        }


        /// <summary>
        /// Error log Message
        /// </summary>
        /// <param name="log">Message displayed</param>
        public static void LogError( string log )
        {
            var buffer = Instance.StringBuffer.Clear();

            SetupLog( buffer, log, Color.red );

            Debug.LogError( buffer.ToString() );

            buffer.Clear();
            buffer.Capacity = 128;
        }

        /// <summary>
        /// Log formatting
        /// </summary>
        /// <param name="buffer">format buffer</param>
        /// <param name="log">Message displayed</param>
        /// <param name="logColor">Displayed message color</param>
        private static void SetupLog( StringBuilder buffer, string log, Color32 logColor )
        {
            AppendPrefix( buffer );

            buffer.Append( log );

            #if UNITY_EDITOR
            AppendColorTag( buffer, logColor );
            #endif
        }

        /// <summary>
        /// Add prefix to format buffer
        /// </summary>
        /// <param name="buffer">format buffer</param>
        private static void AppendPrefix( StringBuilder buffer )
        {
            if( string.IsNullOrEmpty( Instance.Prefix ) ) return;

            buffer.Append( '【' ).Append( Instance.Prefix ).Append( '】' );
        }

        #if UNITY_EDITOR
        /// <summary>
        /// Add color hex tag to format buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="color"></param>
        private static void AppendColorTag( StringBuilder buffer, Color32 color )
        {
            Span<char> colorHex = stackalloc char[6];
            color.r.TryFormat( colorHex[ 0.. ], out _, "X2" );
            color.g.TryFormat( colorHex[ 2.. ], out _, "X2" );
            color.b.TryFormat( colorHex[ 4.. ], out _, "X2" );

            buffer.Insert( 0, ">" ).Insert( 0, colorHex ).Insert( 0, "<color=#" );

            buffer.Append( "</color>" );
        }
        #endif
    }
}
