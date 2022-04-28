// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Message;

/// <summary>Extension methods for <see cref="IMessageProvider"/></summary>
public static class MessageProviderExtensions_
{
    /// <summary></summary>
    public static T SetMessage<T>(this T instance, IMessageDescription statusInfo, object?[] arguments) where T : IMessageProvider 
    { 
        // Create status
        Message status = new Message(statusInfo, arguments);
        // Assign status
        instance.Message = status;
        // Assign error
        if (instance is Exception e) status.Error = e;
        // Return
        return instance; 
    }
}
