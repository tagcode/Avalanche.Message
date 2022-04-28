// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Message;

// <docs>
/// <summary>Interface for classes that can provide <see cref="IMessage"/>.</summary>
public interface IMessageProvider
{
    /// <summary>Associated message</summary>
    IMessage? Message { get; set; }
}
// </docs>
