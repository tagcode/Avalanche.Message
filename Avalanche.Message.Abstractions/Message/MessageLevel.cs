// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Message;

// <docs>
/// <summary>Message severity level.</summary>
/// <remarks>Cast compatible with Microsoft.Extensions.Logging.LogLevel.</remarks>
public enum MessageLevel
{
    /// <summary>Detailed message. May contain sensitive application data.</summary>
    Trace,
    /// <summary>Message for development investigation.</summary>
    Debug,
    /// <summary>Application flow information.</summary>
    Information,
    /// <summary>Abnormal and unexpected events.</summary>
    Warning,
    /// <summary>Error in flow, but not application failure.</summary>
    Error,
    /// <summary>Application failure or crash.</summary>
    Critical,
    /// <summary>No logging.</summary>
    None
}
// </docs>
