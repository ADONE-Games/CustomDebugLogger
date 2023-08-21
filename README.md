# CustomDebugLogger
![Unity](https://img.shields.io/badge/Framework-Unity-blue) ![LICENSE](https://img.shields.io/badge/License-MIT-magenta)  
![DebugLog](https://img.shields.io/badge/Unity-DebugLog-FFFFFF)

# Table of Contents
<hr>

- [Description](#description)
  - [Function structure](#function-structure)
- [Usage](#usage)
  - [Initialize](#initialize)
    - [Example 1](#example-1)
    - [Example 2](#example-2)
  - [Run](#run)
- [Provided Event Functionality](#provided-event-functionality)
  - [Registering Events](#registering-events)
- [Provided Controller Functionality](#provided-controller-functionality)
<hr>

- [License](#license)
<hr>

# Description
<hr>

Add custom processing before being displayed in the console using Debug.Log, Debug.LogWarning, Debug.LogError, etc.
<hr>

## Function structure
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
    Debug <-- ConsoleLoggerController : Send Debug.Log / Debug.LogWarning / Debug.LogError messages
```

<details>
<summary>The original structure of the Debug.Log series</summary>

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

# Usage
<hr>

Create your own class inheriting from ```ADONEGames.CustomDebugLogger.AbstractLoggerEvent```.  
Specify it using ```ADONEGames.CustomDebugLogger.LoggerSetup.Initialize( params LoggerEventFactory[] )```.
<hr>

## Initialize
<hr>

### Example 1
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

### Example 2
<hr>

```csharp
    public class LoggerTest
    {
        // Auto-start before the splash scene
        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSplashScreen )]
        private static void Initialize()
        {
            LoggerSetup.Initialize( handler => new ConsoleLoggerEvent( handler ), handler => new FileLoggerEvent( handler ) );
        }
    }
```
<hr>

## Run
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


# Provided Event Functionality
<hr>

|ADONEGames.CustomDebugLogger||
|-----|-----|
|AbstractLoggerEvent|Abstract class for additional processing|
<hr>

|ADONEGames.CustomDebugLogger|||
|-----|-----|-----|
|ConsoleLoggerEvent|Console display|Inherit from AbstractLoggerEvent|
|FileLoggerEvent|File output|Inherit from AbstractLoggerEvent|
<hr>

## Registering Events
<hr>

|ADONEGames.CustomDebugLogger|||
|----------------------------|-----------------------------------------|-|
| LoggerSetup                | -                                       |Initialization Class|
| -                          | Initialize(params LoggerEventFactory[]) |Registration of additional processing|

```csharp
LoggerSetup.Initialize( handler => new ConsoleLoggerEvent( handler ), handler => new FileLoggerEvent( handler ) );
```
<hr>

# Provided Controller Functionality
<hr>

|ADONEGames.CustomDebugLogger||
|-----|-----|
|AbstractLoggerController|Abstract class for the controller|
<hr>

|ADONEGames.CustomDebugLogger|||
|-----|-----|-----|
|ConsoleLoggerController|Console display|Inherit from AbstractLoggerController|
<hr>

```csharp
ConsoleLoggerController.Log( "Hello World!" );
ConsoleLoggerController.Log( "Hello World!", new Color32( 255, 255, 255, 255 ) );
ConsoleLoggerController.LogWarning( "Hello World!" );
ConsoleLoggerController.LogWarning( "Hello World!", new Color32( 255, 255, 255, 255 ) );
ConsoleLoggerController.LogError( "Hello World!" );
```
<hr>

### License
<hr>

This software is released under the MIT License.  
You are free to use it within the bounds of the license.  
When using it, please include the following copyright and license notices.

[LICENSE](https://github.com/ADONE-Games/CustomDebugLogger/blob/main/LICENSE)
