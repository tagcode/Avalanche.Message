// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Message;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Avalanche.Utilities;

/// <summary>Extension methods for <see cref="IValidable"/>.</summary>
public static class ValidableExtensions_
{
    /// <summary>Convert messages into aggregate exception</summary>
    /// <returns>Compiled validation exception if unexpected codes were found.</returns>
    public static ValidationException? CreateException<L>(ref L list) where L : IList<IMessage>
    {
        // 
        if (list.Count == 0) return null;
        // Got one
        if (list.Count == 1) return (ValidationException)list[0].NewException<ValidationException>(assignExceptionToMessage: true);
        // Make each into exception
        Exception[] errors = new Exception[list.Count];
        for (int i = 0; i < errors.Length; i++)
        {
            // Get message
            IMessage message = list[i];
            // Print to sb
            string print = message.Print();
            // Create inner exception
            Exception innerException = new ValidationException(print);
            // Assign HResult
            if (message.MessageDescription.HResult.HasValue) innerException.HResult = message.MessageDescription.HResult!.Value;
            // Assign message
            MessageExceptionExtensions.AttachMessage(innerException, message);
            // Add to list
            errors[i] = innerException;
        }
        // Create outer exception
        ValidationException outerError = new ValidationException("", errors);
        MessageExceptionExtensions.AttachMessage(outerError, list[0]);
        if (list[0].MessageDescription.HResult.HasValue) outerError.HResult = list[0].MessageDescription.HResult!.Value;
        // Throw 
        return outerError;
    }

    /// <summary>Asserts <paramref name="instance"/> is not bad.</summary>
    /// <exception cref="ValidationException">If validation fails</exception>
    [DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T AssertNotBad<T>(this T instance) where T : IValidable
    {
        // Place here bad status messages
        StructList4<IMessage> bads = new();
        // Get bad ones
        foreach (IMessage status in instance.Validate())
            if (status.MessageDescription.IsBad()) bads.Add(status);
        // Got one
        if (bads.Count == 0) return instance;
        //
        throw ValidableExtensions_.CreateException<StructList4<IMessage>>(ref bads)!;
    }

    /// <summary>Asserts <paramref name="instance"/> is not good.</summary>
    /// <exception cref="ValidationException">If validation fails</exception>
    [DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T AssertGood<T>(this T instance) where T : IValidable
    {
        // Place here bad status messages
        StructList4<IMessage> bads = new();
        // Get bad ones
        foreach (IMessage status in instance.Validate())
            if (status.MessageDescription.IsNotGood()) bads.Add(status);
        // Got one
        if (bads.Count == 0) return instance;
        //
        throw ValidableExtensions_.CreateException<StructList4<IMessage>>(ref bads)!;
    }

    /// <summary>Get most critical validate message. If none exists, return good status message.</summary>
    /// <return>
    /// Single validation message.
    /// 
    /// If <paramref name="instance"/> produced no statuses, returns <see cref="CoreMessages.UncertainValidation"/>.
    /// </return>
    public static IMessage ValidateSingle(this IValidable instance)
    {
        // Message with worst severity
        IMessage? messageWithWorstSeverity = null;
        int severity = -1;
        // 
        foreach (IMessage message in instance.Validate())
        {
            // Get severity
            int _severity = message.MessageDescription.GetSeverityLevel();
            // Assign message
            if (_severity > severity) { messageWithWorstSeverity = message; severity = _severity; }
        }
        // Return
        return messageWithWorstSeverity ?? CoreMessages.Instance.UncertainValidation.New();
    }
}

