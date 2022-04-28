// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Message;
using System.Collections.Generic;

// <docs>
/// <summary>Interface for classes whose state can be verified to be valid.</summary>
public interface IValidable
{
    /// <summary>Check object for invalid state causes.</summary>
    /// <returns>Return validation messages.</returns>
    IEnumerable<IMessage> Validate();
}
// </docs>
