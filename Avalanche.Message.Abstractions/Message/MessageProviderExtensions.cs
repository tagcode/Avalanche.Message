// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Message;

/// <summary>Extension methods for <see cref="IMessageProvider"/></summary>
public static class MessageProviderExtensions
{
    /// <summary>Return code.</summary>
    public static int? Code(this IMessageProvider? messageContainer) => messageContainer?.Message?.MessageDescription?.Code;
    /// <summary>Event Key</summary>
    public static string? Key(this IMessageProvider? messageContainer) => messageContainer?.Message?.MessageDescription?.Key;
    /// <summary>Parameter names.</summary>
    public static IEnumerable<string?>? ParameterNames(this IMessageProvider? messageContainer) => messageContainer?.Message?.MessageDescription?.Template?.ParameterNames;
    /// <summary>Description about event. May contain xml.</summary>
    public static string? Description(this IMessageProvider? messageContainer) => messageContainer?.Message?.MessageDescription?.Description;
    /// <summary>Set <paramref name="message"/> to <paramref name="messageContainer"/>.</summary>
    public static SP SetMessage<SP>(this SP messageContainer, IMessage? message) where SP : IMessageProvider { messageContainer.Message = message; return messageContainer; }
}
