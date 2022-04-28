// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Message;
using System.Runtime.Serialization;

/// <summary>Message record</summary>
public class Message : MessageBase
{
    /// <summary>Create message</summary>
    public Message() : base() { }

    /// <summary>Create message with arguments</summary>
    public Message(IMessageDescription messageDescription, params object?[] arguments) : base(messageDescription, arguments)
    {
        this.messageDescription = messageDescription;
        this.arguments = arguments;
    }

    /// <summary>Copy message from <paramref name="copyFrom"/>.</summary>
    public Message(IMessage copyFrom) : base(copyFrom) { }

    /// <summary>Deserialize exception</summary>
    protected Message(SerializationInfo info, StreamingContext context) : base(info, context) { }
}

