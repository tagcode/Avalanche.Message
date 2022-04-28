// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Message;
using System.Collections;
using Avalanche.Utilities;

// <docs>
/// <summary>Message entry with arguments.</summary>
/// <remarks>
/// Message <see cref="IDictionary"/> implementation provides parameter name to argument mapping.
/// Message <see cref="object.ToString"/> prints <see cref="Arguments"/> in <see cref="IMessageDescription.Template"/> using 'null' culture.
/// </remarks>
public interface IMessage : IDictionary, IDictionary<string, object?>, IUserDataContainer, IFormattable, ISpanFormattable
{
    /// <summary>Message identification, such as <see cref="int"/> or <see cref="Guid"/></summary>
    object? Id { get; set; }
    /// <summary>Message description.</summary>
    IMessageDescription MessageDescription { get; set; }
    /// <summary>Arguments.</summary>
    object?[] Arguments { get; set; }
    /// <summary>Event occurance time.</summary>
    DateTime? Time { get; set; }
    /// <summary>Message severity level for logging.</summary>
    MessageLevel? Severity { get; set; }
    /// <summary>Captured error</summary>
    Exception? Error { get; set; }
    /// <summary>Inner messages</summary>
    IMessage[]? InnerMessages { get; set; }
}
// </docs>

