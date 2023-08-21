# CustomDebugLogger
![Unity](https://img.shields.io/badge/Framework-Unity-blue) ![LICENSE](https://img.shields.io/badge/License-MIT-magenta)  
![DebugLog](https://img.shields.io/badge/Unity-DebugLog-FFFFFF)

# 目次
- [説明](#説明)
  - [機能構造](#機能構造)
- [使用方法](#使用方法)
  - [初期化](#初期化)
    - [例1](#例1)
    - [例2](#例2)
  - [実行](#実行)
- [イベント機能](#イベント機能)
  - [イベントの登録](#イベントの登録)
- [コントローラー機能](#コントローラー機能)
<hr>

- [ライセンス](#ライセンス)
<hr>

# 説明
<hr>

Debug.Log/Debug.LogWarning/Debug.LogErrorなどでコンソールに表示される前に独自の処理を追加する。
<hr>

## 機能構造
```mermaid
classDiagram
    direction TB

    namespace UnityEngine {
        class ILogHandler {
            <<interface>>
            + LogFormat(LogType, UnityEngine.Object, string, params object[]): ~void~
            + LogException(Exception, Object): ~void~
        }
        class ILogger {
            <<interface>>
            + logHandler : ~ILogHandler~
        }
        class Debug {
            + Log(string): ~void~
            + Log(string, UnityEngine.Object): ~void~
            + LogWarning(string): ~void~
            + LogWarning(string, UnityEngine.Object): ~void~
            + LogError(string): ~void~
            + LogError(string, UnityEngine.Object): ~void~
        }
        class Logger{
            + logHandler[ LoggerEventManager ] : ILogHandler
        }
    }
    
    namespace System {
        class IDisposable {
            <<interface>>
            + Dispose(): ~void~
        }
    }

    namespace ADONEGames_CustomDebugLogger_internal {
        class ILoggerEvent {
            <<interface>>
        }
        class LoggerEventManager {
            - OriginalDebugLogHandler[ DebugLogHandler ] : ~ILogHandler~
            + LogForamt(LogType, UnityEngine.Object, string, params object[]): ~void~
            + LogException(Exception, UnityEngine.Object): ~void~
        }
    }

    namespace ADONEGames_CustomDebugLogger_public {
        class LoggerSetup {
            + Initialize(params LoggerEventFactory[]): ~AbstractLoggerEvent~
        }
        class AbstractLoggerEvent {
            <<abstract>>
            # OriginalDebugLogHandler : ~ILogHandler~
            + LogForamt(LogType, UnityEngine.Object, string, params object[])*: abstract ~void~
            + LogException(Exception, UnityEngine.Object)*: abstract ~void~
            + Dispose(): virtual ~void~
        }

        class ConsoleLoggerEvent {
            + LogForamt(LogType, UnityEngine.Object, string, params object[]): ~void~
            + LogException(Exception, UnityEngine.Object): ~void~
        }
        class FileLoggerEvent {
            + LogForamt(LogType, UnityEngine.Object, string, params object[]): ~void~
            + LogException(Exception, UnityEngine.Object): ~void~
        }

        class AbstractLoggerController{
            <<abstract>>
            # Prefix : abstract ~string~ 
            + Log(string)$ : ~void~
            + Log(string, UnityEngine.Color32)$ : ~void~
            + LogWarning(string)$ : ~void~
            + LogWarning(string, UnityEngine.Color32)$ : ~void~
            + LogError(string)$ : ~void~
        }
        class ConsoleLoggerController{
            # Prefix : ~string~
        }
    }
    
    namespace view {
        class DebugLogHandler [ "DebugLogHandler : < ILogHandler>" ]
        class console
    }
    
    namespace io {
        class FileOutput
    }

    ILogHandler <|-- Logger
    ILogHandler <|-- ILogger
    ILogger <|-- Logger
    Debug --> Logger

    ILogHandler <|-- ILoggerEvent
    IDisposable <|-- ILoggerEvent
    ILoggerEvent <|-- AbstractLoggerEvent
    ILoggerEvent <|-- LoggerEventManager
    Logger --> LoggerEventManager

    LoggerEventManager *-- LoggerSetup
    LoggerSetup <-- AbstractLoggerEvent
    AbstractLoggerEvent <|.. ConsoleLoggerEvent
    AbstractLoggerEvent <|.. FileLoggerEvent
    
    FileLoggerEvent --> FileOutput
    ConsoleLoggerEvent --> DebugLogHandler
    DebugLogHandler --> console
    AbstractLoggerEvent"use" -- DebugLogHandler
    

    ConsoleLoggerController --|> AbstractLoggerController 
    Debug <-- ConsoleLoggerController : Debug.Log / Debug.LogWarning / Debug.LogErrorメッセージを送信
```

<details>
<summary>元のDebug.Log系の構造</summary>

```mermaid
classDiagram
    direction TB

    namespace UnityEngine {
        class ILogHandler {
            <<interface>>
            + LogFormat(LogType, UnityEngine.Object, string, params object[]): ~void~
            + LogException(Exception, UnityEngine.Object): ~void~
        }
        class ILogger {
            <<interface>>
            + logHandler : ~ILogHandler~
        }
        class Debug {
            + Log(string): ~void~
            + Log(string, UnityEngine.Object): ~void~
            + LogWarning(string): ~void~
            + LogWarning(string, UnityEngine.Object): ~void~
            + LogError(string): ~void~
            + LogError(string, UnityEngine.Object): ~void~
        }
        class Logger{
            + logHandler[DebugLogHandler] : ~ILogHandler~
        }
    }
    
   
    namespace view {
        class DebugLogHandler [ "DebugLogHandler : ~ILogHandler~" ]
        class console
    }

    ILogHandler <|-- Logger
    ILogHandler <|-- ILogger
    ILogger <|-- Logger
    Debug --> Logger

    Logger --> DebugLogHandler
    DebugLogHandler --> console
```

</details>
<hr>

# 使用方法
<hr>

```ADONEGames.CustomDebugLogger.AbstractLoggerEvent```を継承した自身のクラス作成  
```ADONEGames.CustomDebugLogger.LoggerSetup.Initialize( params LoggerEventFactory[] )```で指定する。
<hr>

## 初期化
<hr>

### 例1
<hr>

```csharp
    public class LoggerTest : MonoBehaviour
    {
        private void Start()
        {
            LoggerSetup.Initialize( handler => new ConsoleLoggerEvent( handler ), handler => new FileLoggerEvent( handler ) );
        }
    }
```
<hr>

### 例2
<hr>

```csharp
    public class LoggerTest
    {
        // スプラッシュシーン前に自動起動
        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSplashScreen )]
        private static void Initialize()
        {
            LoggerSetup.Initialize( handler => new ConsoleLoggerEvent( handler ), handler => new FileLoggerEvent( handler ) );
        }
    }
```
<hr>

## 実行
<hr>

```csharp
    public class ButtonTrigger : MonoBehaviour
    {
        public void OnClick_NormalDebugLog()
        {
            Debug.Log( "Button Clicked!" );
        }
        public void OnClick_CustomDebugLog()
        {
            MyClassLoggerController.Log( "ボタン　クリック！", Color.magenta );
        }
    }
```
<hr>


# イベント機能
<hr>

|ADONEGames.CustomDebugLogger||
|-----|-----|
|AbstractLoggerEvent|追加処理の抽象クラス|
<hr>

|ADONEGames.CustomDebugLogger|||
|-----|-----|-----|
|ConsoleLoggerEvent|コンソール表示|AbstractLoggerEventを継承|
|FileLoggerEvent|ファイル書き出し|AbstractLoggerEventを継承|
<hr>

## イベントの登録
<hr>

|ADONEGames.CustomDebugLogger|||
|----------------------------|-----------------------------------------|-|
| LoggerSetup                | -                                       |初期化クラス|
| -                          | Initialize(params LoggerEventFactory[]) |追加処理の登録|

```csharp
LoggerSetup.Initialize( handler => new ConsoleLoggerEvent( handler ), handler => new FileLoggerEvent( handler ) );
```
<hr>

# コントローラー機能
<hr>

|ADONEGames.CustomDebugLogger||
|-----|-----|
|AbstractLoggerController|コントローラーの抽象クラス|
<hr>

|ADONEGames.CustomDebugLogger|||
|-----|-----|-----|
|ConsoleLoggerController|コンソール表示|AbstractLoggerControllerを継承|

```csharp
ConsoleLoggerController.Log( "Hello World!" );
ConsoleLoggerController.Log( "Hello World!", new Color32( 255, 255, 255, 255 ) );
ConsoleLoggerController.LogWarning( "Hello World!" );
ConsoleLoggerController.LogWarning( "Hello World!", new Color32( 255, 255, 255, 255 ) );
ConsoleLoggerController.LogError( "Hello World!" );
```
<hr>


### ライセンス
<hr>

本ソフトウェアはMITライセンスで公開しています。  
ライセンスの範囲内で自由に使用可能です。  
使用する際は、以下の著作権表示とライセンス表示をお願いします。  
<hr>

[LICENSE](https://github.com/ADONE-Games/CustomDebugLogger/blob/main/LICENSE)
<hr>