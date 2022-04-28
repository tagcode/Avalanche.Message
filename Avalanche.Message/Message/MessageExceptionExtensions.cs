// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Message;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using Avalanche.Utilities;

/// <summary><see cref="IMessage"/> <see cref="Exception"/> handling extension methods.</summary>
public static class MessageExceptionExtensions
{
    /// <summary>Key to use for message</summary>
    public const string Key = "Avalanche.Utilities.IMessage";
    /// <summary>Attach <paramref name="message"/> object to <paramref name="e"/>.</summary>
    public static void AttachMessage(this Exception e, IMessage message) => e.Data[Key] = message;
    /// <summary>Remove status from <paramref name="e"/>.</summary>
    public static void RemoveMessageObject(this Exception e) => e.Data.Remove(Key);
    /// <summary>Try get message.</summary>
    public static bool TryGetMessage(this Exception e, out IMessage message) 
    {
        // Get
        message = (e.Data[Key] as IMessage)!;
        // return
        return message != null;
    }
    /// <summary>Get <see cref="IMessage"/>.</summary>
    public static IMessage? StatusMessage(this Exception e) => e.Data[Key] as IMessage;
    /// <summary>Get <see cref="IMessage"/> code.</summary>
    public static int? MessageCode(this Exception e) => (e.Data[Key] as IMessage)?.MessageDescription?.Code;

    /// <summary>Try get message argument.</summary>
    public static bool TryGetMessageArgument(this Exception e, string parameterName, out object? argument) 
    {
        // Get
        IMessage message = (e.Data[Key] as IMessage)!;
        // No message
        if (message == null) { argument = null!; return false; }
        // Get value
        argument = message?[parameterName];
        // Return 
        return argument != null;
    }

    /// <summary>Create exception.</summary>
    /// <param name="messageDescription"></param>
    /// <param name="exceptionType">requested exception type</param>
    /// <param name="message">Message to print message from and to derive inner exception</param>
    public static Exception CreateException(IMessageDescription? messageDescription, Type? exceptionType, IMessage message)
    {
        // Use exception constructor
        if (messageDescription?.Exception is Delegate @delegate && (exceptionType??typeof(Exception)).IsAssignableFrom(@delegate.Method.ReturnType))
        {
            // Get parameters
            ParameterInfo[] @params = @delegate.Method.GetParameters();
            // Place here exception
            Exception? e = null;
            // No params
            if (@params.Length == 0) e = (Exception)@delegate.DynamicInvoke(null)!;
            // params[0] = IMessage
            else if (@params.Length == 1 && @params[0].ParameterType.IsAssignableFrom(message.GetType())) e = (Exception)@delegate.DynamicInvoke(message)!;
            // params[0] = Exception
            else if (@params.Length == 1 && @params[0].ParameterType.IsAssignableFrom(typeof(Exception))) e = (Exception)@delegate.DynamicInvoke(message.Error)!;
            // params[0] = IMessage, params[1] = Exception
            else if (@params.Length == 2 && @params[0].ParameterType.IsAssignableFrom(message.GetType()) && @params[1].ParameterType.IsAssignableFrom(typeof(Exception))) e = (Exception)@delegate.DynamicInvoke(message, message.Error)!;
            // params[0] = Exception, params[1] = IMessage
            else if (@params.Length == 2 && @params[1].ParameterType.IsAssignableFrom(message.GetType()) && @params[0].ParameterType.IsAssignableFrom(typeof(Exception))) e = (Exception)@delegate.DynamicInvoke(message.Error, message)!;
            // Not the of the explicitly requested type
            if (e != null) if (exceptionType != null && !e.GetType().IsAssignableTo(exceptionType)) e = null;
            // Got a passable exception
            return e!;
        }

        // Exception type
        if (exceptionType == null && messageDescription?.Exception is Type type) exceptionType = type;
        // Resolve exception type name
        if (exceptionType == null && messageDescription?.Exception is string typeName) exceptionType = Type.GetType(typeName, throwOnError: false, ignoreCase: false);
        // Fallback
        if (exceptionType == null) exceptionType = typeof(Exception);
        // Print out
        string print = message.MessageDescription?.Template.Print(null, message.Arguments) ?? "";
        // Try create with utility
        if (ExceptionUtilities.TryCreate2(exceptionType, message.Error, print, out Exception ee)) return ee;
        // Fallback
        return new Exception(print, message.Error);
    }

    /// <summary>Create exception but don't throw it</summary>
    public static Exception NewException(this IMessage message, bool assignExceptionToMessage = true)
    {
        // Get message description
        IMessageDescription? messageDescription = message.MessageDescription;
        // Create exception
        Exception e = CreateException(messageDescription, null, message);
        // Attach message to exception
        e.AttachMessage(message);
        // Assign exception to message
        if (assignExceptionToMessage) message.Error = e;
        // Get HResult
        int? hresult = messageDescription?.HResult;
        // Assign HResult
        if (hresult.HasValue) e.HResult = hresult.Value;
        // Get HelpLink
        string? helpLink = messageDescription?.HelpLink;
        // Assign HelpLink
        if (helpLink != null) e.HelpLink = helpLink;
        // Return
        return e;
    }

    /// <summary>Create exception but don't throw it</summary>
    public static Exception NewException<E>(this IMessage message, bool assignExceptionToMessage = true) where E : Exception
    {
        // Get message description
        IMessageDescription? messageDescription = message.MessageDescription;
        // Create exception
        Exception e = CreateException(messageDescription, typeof(E), message);
        // Attach message to exception
        e.AttachMessage(message);
        // Assign exception to message
        if (assignExceptionToMessage) message.Error = e;
        // Get HResult
        int? hresult = messageDescription?.HResult;
        // Assign HResult
        if (hresult.HasValue) e.HResult = hresult.Value;
        // Get HelpLink
        string? helpLink = messageDescription?.HelpLink;
        // Assign HelpLink
        if (helpLink != null) e.HelpLink = helpLink;
        // Return
        return e;
    }
    /// <summary>Throw as excpetion</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden, DoesNotReturn]
    public static void Throw(this IMessage message, bool assignExceptionToMessage = true) => throw message.NewException(assignExceptionToMessage);
    /// <summary>Throw as excpetion</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden, DoesNotReturn]
    public static void Throw<E>(this IMessage message, bool assignExceptionToMessage = true) where E : Exception => throw message.NewException<E>(assignExceptionToMessage);

    /// <summary>Create exception but don't throw it</summary>
    public static Exception NewException(this IMessageDescription messageDescription, params object?[]? arguments) => messageDescription.New(arguments).NewException(assignExceptionToMessage: true);
    /// <summary>Create exception but don't throw it</summary>
    public static Exception NewException<E>(this IMessageDescription messageDescription, params object?[]? arguments) where E : Exception => messageDescription.New(arguments).NewException<E>(assignExceptionToMessage: true);
    /// <summary>Throw as excpetion</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden, DoesNotReturn]
    public static void Throw(this IMessageDescription messageDescription, params object?[]? arguments) => throw messageDescription.NewException(arguments);
    /// <summary>Throw as excpetion</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden, DoesNotReturn]
    public static void Throw<E>(this IMessageDescription messageDescription, params object?[]? arguments) where E : Exception => throw messageDescription.NewException<E>(arguments);

}
