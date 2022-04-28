// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Message;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Avalanche.Template;
using Avalanche.Utilities;

/// <summary>
/// <see cref="IMessageDescription"/> comparer.
/// 
/// Sorts by criteria in order:
///     <see cref="IMessageDescription.Code"/>
///     <see cref="IMessageDescription.Key"/>
///     <see cref="IMessageDescription.Template"/>
/// </summary>
public class MessageDescriptionComparer : IComparer<IMessageDescription>, IEqualityComparer<IMessageDescription>
{
    /// <summary>Singleton</summary>
    static MessageDescriptionComparer instance = new MessageDescriptionComparer();
    /// <summary>Singleton</summary>
    public static MessageDescriptionComparer Instance => instance;

    /// <summary>Compare <paramref name="x"/> to <paramref name="y"/></summary>
    public int Compare(IMessageDescription? x, IMessageDescription? y)
    {
        // Check nulls
        if (x == null && y == null) return 0;
        if (x == null) return -1;
        if (y == null) return 1;

        // Code
        int? xid = x!.Code, yid = y!.Code;
        if (xid == null && yid == null) return 0;
        if (xid == null) return -1;
        if (yid == null) return 1;
        int d = (xid.Value & 0x0FFFFFFF) - (yid.Value & 0x0FFFFFFF);
        if (d != 0) return d;
        d = ((xid.Value >> 28) & 0xF) - ((yid.Value >> 28) & 0xF);
        if (d != 0) return d;

        // Key
        string? xk = x?.Key, yk = y?.Key;
        if (xk == null && yk == null) return 0;
        if (xk == null) return -1;
        if (yk == null) return 1;
        d = AlphaNumericComparer.InvariantCultureIgnoreCase.Compare(xk, yk);
        if (d != 0) return d;

        // Template
        ITemplateText? xm = x?.Template, ym = y?.Template;
        if (xm == null && y == null) return 0;
        if (xm == null) return -1;
        if (ym == null) return 1;
        d = TemplateTextComparer.Instance.Compare(xm, ym);
        if (d != 0) return d;

        // Equals
        return 0;
    }

    /// <summary>Compare equality of <paramref name="x"/> to <paramref name="y"/></summary>
    public bool Equals(IMessageDescription? x, IMessageDescription? y)
    {
        // Key
        if (!StringComparer.OrdinalIgnoreCase.Equals(x?.Key, y?.Key)) return false;
        // Code
        int? xid = x?.Code, yid = y?.Code;
        if (xid.HasValue != yid.HasValue) return false;
        if (xid.HasValue && yid.HasValue && xid.Value != yid.Value) return false;
        // Template
        if (!TemplateTextComparer.Instance.Equals(x?.Template, y?.Template)) return false;
        //
        return true;
    }

    /// <summary>Create hash</summary>
    public int GetHashCode([DisallowNull] IMessageDescription messageDescription)
    {
        // Init
        int hash = -2128831035;
        // Key
        hash = (hash ^ (messageDescription.Key == null ? 0 : messageDescription.Key.GetHashCode())) * 16777619;
        // Code
        hash = messageDescription.Code.HasValue ? (hash ^ messageDescription.Code.Value) * 16777619 : hash;
        // Template
        hash = (hash ^ (messageDescription.Template == null ? 0 : TemplateTextComparer.Instance.GetHashCode(messageDescription.Template))) * 16777619;
        // Return
        return hash;
    }
}
