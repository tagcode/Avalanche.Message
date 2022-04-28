// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Message;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

/// <summary><see cref="IMessage"/> comparer.</summary>
public class MessageComparer : IComparer<IMessage>, IEqualityComparer<IMessage>
{
    /// <summary>Singleton</summary>
    static MessageComparer instance = new MessageComparer(MessageDescriptionComparer.Instance, MessageDescriptionComparer.Instance);
    /// <summary>Singleton</summary>
    public static MessageComparer Instance => instance;

    /// <summary></summary>
    protected IComparer<IMessageDescription> messageDescriptionComparer;
    /// <summary></summary>
    protected IEqualityComparer<IMessageDescription> messageDescriptinEqualityComparer;

    /// <summary></summary>
    public MessageComparer(IComparer<IMessageDescription> messageDescriptionComparer, IEqualityComparer<IMessageDescription> messageDescriptinEqualityComparer)
    {
        this.messageDescriptionComparer = messageDescriptionComparer ?? throw new ArgumentNullException(nameof(messageDescriptionComparer));
        this.messageDescriptinEqualityComparer = messageDescriptinEqualityComparer ?? throw new ArgumentNullException(nameof(messageDescriptinEqualityComparer));
    }

    /// <summary>Compare <paramref name="x"/> to <paramref name="y"/></summary>
    public int Compare(IMessage? x, IMessage? y)
    {
        // Check nulls
        if (x == null && y == null) return 0;
        if (x == null) return -1;
        if (y == null) return 1;

        // StatusCode
        int d = messageDescriptionComparer.Compare(x.MessageDescription, y.MessageDescription);
        if (d != 0) return d;

        // Arguments
        object?[]? xa = x.Arguments, ya = y.Arguments;
        if (xa == null && ya == null) { }
        else if (xa == null) return -1;
        else if (ya == null) return 1;
        else
        {
            int c = Math.Min(xa.Length, ya.Length);
            for (int i = 0; i < c; i++)
            {
                object? xo = x.Arguments[i], yo = y.Arguments[i];
                d = Comparer<object>.Default.Compare(xo, yo);
                if (d != 0) return d;
            }

            if (xa.Length > ya.Length) return 1;
            if (xa.Length < ya.Length) return -1;
        }

        // Equals
        return 0;
    }

    /// <summary>Compare equality of <paramref name="x"/> to <paramref name="y"/></summary>
    public bool Equals(IMessage? x, IMessage? y)
    {
        // Compare status codes
        if (!messageDescriptinEqualityComparer.Equals(x?.MessageDescription, y?.MessageDescription)) return false;
        // Get arguments
        object?[]? xa = x?.Arguments, ya = y?.Arguments;
        //
        if (xa == null && ya == null) { }
        else if (xa == null) return false;
        else if (ya == null) return false;
        else
        {
            if (xa.Length != ya.Length) return false;
            int c = xa.Length;
            for (int i = 0; i < c; i++)
            {
                object? xe = xa[i], ye = ya[i];
                if (xe == null && ye == null) { }
                else if (xe == null) return false;
                else if (ye == null) return false;
                else if (!EqualityComparer<object>.Default.Equals(xe, ye)) return false;
            }
        }

        //
        return true;
    }

    /// <summary>Create hash</summary>
    public int GetHashCode([DisallowNull] IMessage @event)
    {
        // Init
        int hash = 234234235;
        // StatusCode
        IMessageDescription? statusCode = @event.MessageDescription;
        if (statusCode != null) hash = (hash ^ messageDescriptinEqualityComparer.GetHashCode(statusCode)) * 16777619;
        // Arguments
        if (@event.Arguments != null)
        {
            int c = @event.Arguments.Length;
            for (int i = 0; i < c; i++)
            {
                object? xe = @event.Arguments[i];
                hash = (hash ^ (xe == null ? 0 : xe.GetHashCode())) * 16777619;
            }
        }
        // Return
        return hash;
    }
}
