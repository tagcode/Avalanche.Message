// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Message;
using System.Diagnostics;
using System.Runtime.CompilerServices;

/// <summary>Extension methods for <see cref="IMessageDescription"/>.</summary>
public static class MessageDescriptionExtensions_
{
    /// <summary>Create new status of this code</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public static Message New(this IMessageDescription messageDescription) => new Message(messageDescription, Array.Empty<object>());

    /// <summary>Create new status of this code</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public static Message New(this IMessageDescription messageDescription, params object?[]? arguments) => new Message(messageDescription, arguments ?? Array.Empty<object?>());

    /// <summary>Create new status of this code</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public static Message NameOf(this IMessageDescription messageDescription, object? arg0, [CallerArgumentExpression("arg0")] string? expression = null) => new Message(messageDescription, expression, arg0);

    /// <summary>Create clone of <paramref name="messageDescription"/>.</summary>
    /// <returns>Clone in writable state.</returns>
    public static MessageDescription Clone(this IMessageDescription messageDescription)
    {
        // Create description
        MessageDescription result = new MessageDescription
        {
            Code = messageDescription.Code,
            Description = messageDescription.Description,
            Exception = messageDescription.Exception,
            HelpLink = messageDescription.HelpLink,
            HResult = messageDescription.HResult,
            Key = messageDescription.Key,
            Severity = messageDescription.Severity,
            Template = messageDescription.Template,
        };
        // Copy user-data
        if (messageDescription.HasUserData)
            foreach (var line in messageDescription.UserData)
                result.SetUserData(line.Key, line.Value);
        // Return
        return result;
    }
}

