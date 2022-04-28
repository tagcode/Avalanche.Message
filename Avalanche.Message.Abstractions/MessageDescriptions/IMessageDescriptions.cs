// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Message;
using System.Collections.Generic;
using Avalanche.Utilities;

// <docs>
/// <summary>Table of <see cref="IMessageDescriptions"/>.</summary>
public interface IMessageDescriptions : IReadOnly
{
    /// <summary>Message descriptions by key</summary>
    IDictionary<string, IMessageDescription> Keys { get; set; }
    /// <summary>Message descriptions by hresult</summary>
    IDictionary<int, List<IMessageDescription>> HResults { get; set; }
    /// <summary>Message descriptions by code</summary>
    IDictionary<int, IMessageDescription> Codes { get; set; }
    /// <summary>Message descriptions as list</summary>
    IList<IMessageDescription> List { get; set; }

    /// <summary>Add <paramref name="messageDescription"/>.</summary>
    /// <exception cref="InvalidOperationException">On key or code collision.</exception>
    void Add(IMessageDescription messageDescription);
}
// </docs>
