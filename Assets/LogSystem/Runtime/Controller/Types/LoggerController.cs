namespace ADONEGames.CustomDebugLogger
{
    /// <summary>
    /// Regular logger without prefix
    /// </summary>
    public class LoggerController : AbstractLoggerController<LoggerController>
    {
        protected override string Prefix => "";
    }
}