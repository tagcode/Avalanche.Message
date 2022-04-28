<b>Avalanche.Message.Logging</b> contains 
[ILogger](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.ilogger) extensions for messaging,
[[git]](https://github.com/tagcode/Avalanche.Message/Avalanche.Message.Logging/), 
[[www]](https://avalanche.fi/Avalanche.Core/Avalanche.Message/docs/logging/index.html), 
[[licensing]](https://avalanche.fi/Avalanche.Core/license/index.html).

<b>.LogMessage(this logger, message)</b> writes message to logger.

```csharp
// Create message
IMessage message = SystemMessages.ArgumentNull.Generic.New("argumentName");
// Log message
logger.LogMessage(message);
```

<b>.LogTo(this message, logger)</b> writes message to logger.

```csharp
// Create message
IMessage message = SystemMessages.ArgumentNull.Generic.New("argumentName");
// Log message
message.LogTo(logger);
```

Note that <b>.SetSeverity(this message, level)</b> assigns overriding log level to the message.

```csharp
SystemMessages.ArgumentNull.Generic.New("argumentName").SetSeverity(LogLevel.Critical).LogTo(logger);
```

<b>.LogTo(this message, logger, loglevel)</b> and <b>.LogMessage(this logger, message, loglevel)</b> overload log level parameter.

```csharp
IMessage msg = SystemMessages.ArgumentNull.Generic.New("argumentName");
logger.LogMessage(msg, LogLevel.Critical);
msg.LogTo(logger, LogLevel.Critical);
```


