// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Message;

using System.Runtime.CompilerServices;
using Avalanche.Template;
using Microsoft.Extensions.Logging;

/// <summary><see cref="IMessage"/> logging extension methods.</summary>
public static class MessageLoggingExtensions
{
    /// <summary>Log <paramref name="logger"/>.</summary>
    static void _LogMessage(ILogger logger, IMessage message, LogLevel? logLevel)
    {
        // Get message description
        IMessageDescription? messageDescription = message.MessageDescription;
        // Get code
        int? code = messageDescription?.Code;

        // Choose log level
        if (!logLevel.HasValue)
        {
            // Choose message level
            MessageLevel? messageLevel = message.Severity ?? messageDescription?.Severity;
            // Choose from HResult
            if (!messageLevel.HasValue) 
            {
                // Get hresult
                int? hresult = messageDescription?.HResult;
                // Error flag
                if (hresult.HasValue) messageLevel = (hresult.Value & unchecked((int)0x80000000)) == 0 ? MessageLevel.Debug : MessageLevel.Error;
            }
            // Choose from Code
            if (!messageLevel.HasValue && code.HasValue) messageLevel = (code.Value & unchecked((int)0x80000000)) == 0 ? MessageLevel.Debug : MessageLevel.Error;
            // Fallback
            if (!messageLevel.HasValue) messageLevel = MessageLevel.Debug;
            // Cast to log level
            logLevel = (LogLevel)(int)messageLevel;
        }

        // Get template breakdown
        ITemplateBreakdown? templateBreakdown = message.MessageDescription?.Template?.Breakdown;
        // Get logger compatible template text (TODO localization and pluralization)
        string loggerTemplate = templateBreakdown?.LoggerTemplate() ?? "";

        // Get exception
        Exception? exception = message.Error;
        // Get arguments
        object?[] arguments = message.Arguments ?? Array.Empty<object?>();

        // Got code
        if (code.HasValue)
        {
            // Create event id with message description code
            EventId eventId = new EventId(code.Value, message.MessageDescription?.Key);
            // Log message
            logger.Log(logLevel.Value, eventId, exception, loggerTemplate, arguments);
        } else
        // No code
        {
            // Log message
            logger.Log(logLevel.Value, exception, loggerTemplate, arguments);
        }
    }

    /// <summary>Log <paramref name="logger"/>.</summary>
    public static void LogMessage(this ILogger logger, IMessage message) => _LogMessage(logger, message, null);
    /// <summary>Log <paramref name="logger"/>.</summary>
    public static void LogMessage(this ILogger logger, IMessage message, LogLevel logLevel) => _LogMessage(logger, message, logLevel);
    /// <summary>Log <paramref name="logger"/>.</summary>
    public static void LogTo(this IMessage message, ILogger logger) => _LogMessage(logger, message, null);
    /// <summary>Log <paramref name="logger"/>.</summary>
    public static void LogTo(this IMessage message, ILogger logger, LogLevel logLevel) => _LogMessage(logger, message, logLevel);

    /// <summary>Message severity using <paramref name="logLevel"/>.</summary>    
    public static S SetSeverity<S>(this S message, LogLevel logLevel) where S : IMessage { message.Severity = (MessageLevel)(int)logLevel; return message; }

}
