// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Message;
using Microsoft.Extensions.Logging;

/// <summary><see cref="IMessage"/> logging extension methods.</summary>
public static class MessageDescriptionLoggingExtensions
{
    /// <summary>Message severity using <paramref name="logLevel"/>.</summary>    
    public static S SetSeverity<S>(this S message, LogLevel logLevel) where S : IMessageDescription { message.Severity = (MessageLevel)(int)logLevel; return message; }

}
