# CustomDebugLogger
![Unity](https://img.shields.io/badge/Framework-Unity-blue) ![LICENSE](https://img.shields.io/badge/License-MIT-magenta)  
![DebugLog](https://img.shields.io/badge/Unity-DebugLog-FFFFFF)

# 説明
Debug.Log/Debug.LogWarning/Debug.LogErrorなどが、
コンソールに表示される前に独自の処理を追加する。

# 使用方法  
```ADONEGames.CustomDebugLogger.AbstractLoggerEvent```を継承し作成した自身のクラスを  
```ADONEGames.CustomDebugLogger.LoggerSetup.Initialize( params LoggerEventFactory[] )```で指定する。  
## 例
```
    public class LoggerTest : MonoBehaviour
    {
        private void Start()
        {
            LoggerSetup.Initialize( handler => new ConsoleLoggerEvent( handler ), handler => new FileLoggerEvent( handler ) );
        }
    }
```

|ADONEGames.CustomDebugLogger|
|--------------------------- |
|AbstractLoggerEvent|

# まだ書き途中

### ライセンス
本ソフトウェアはMITライセンスで公開しています。  
ライセンスの範囲内で自由に使用可能です。  
使用する際は、以下の著作権表示とライセンス表示をお願いします。  

[LICENSE](https://github.com/ADONE-Games/CustomDebugLogger/blob/main/LICENSE)
