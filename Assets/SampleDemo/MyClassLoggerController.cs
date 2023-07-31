using ADONEGames.CustomDebugLogger;

namespace Scenes
{
    public class MyClassLoggerController: AbstractLoggerController<MyClassLoggerController>
    {
        protected override string Prefix => "僕のクラス";
    }
}
