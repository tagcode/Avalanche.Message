// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Message;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Avalanche.Template;
using Avalanche.Utilities;

/// <summary>Extension methods for <see cref="IMessageDescription"/>.</summary>
public static class MessageDescriptionExtensions
{
    /// <summary>Set code numeric identifier</summary>
    /// <param name="code">Code between -2147483648 .. 4294967295u</param>
    public static S SetCode<S>(this S message, long? code) where S : IMessageDescription 
    {
        // No code
        if (!code.HasValue) message.Code = null;
        // Assign code
        else message.Code = code >= int.MinValue && code <= uint.MaxValue ? unchecked((int)code) : throw new ArgumentOutOfRangeException(nameof(code));
        // Return
        return message; 
    }
    /// <summary>Set HResult code</summary>
    public static S SetHResult<S>(this S message, long? hresult) where S : IMessageDescription 
    {
        // No code
        if (!hresult.HasValue) message.HResult = null;
        // Assign code
        else message.HResult = hresult >= int.MinValue && hresult <= uint.MaxValue ? unchecked((int)hresult) : throw new ArgumentOutOfRangeException(nameof(hresult));
        // return
        return message; 
    }
    /// <summary>Set string identifier as canonical string, e.g. <![CDATA["MyLibrary.MyEvent"]]>.</summary>
    public static S SetKey<S>(this S message, string key) where S : IMessageDescription { message.Key = key; return message; }
    /// <summary>Set message template format where arguments are named, e.g. "{entry}: {exception}". Compatible with ILogger frameworks.</summary>
    public static S SetTemplate<S>(this S message, ITemplateText template) where S : IMessageDescription { message.Template = template; return message; }
    /// <summary>Message severity information for logging.</summary>    
    public static S SetSeverity<S>(this S message, MessageLevel severity) where S : IMessageDescription { message.Severity = severity; return message; }
    /// <summary>Set description about event. May contain xml.</summary>
    public static S SetDescription<S>(this S message, string? description) where S : IMessageDescription { message.Description = description; return message; }
    /// <summary>Set default exception as type</summary>
    public static S SetException<S>(this S message, Type exceptionType) where S : IMessageDescription { message.Exception = exceptionType; return message; }
    /// <summary>Set default exception as type</summary>
    public static S SetException<S>(this S message, string exceptionTypeName) where S : IMessageDescription { message.Exception = exceptionTypeName; return message; }
    /// <summary>Set default exception constructor</summary>
    public static S SetException<S>(this S message, Delegate? exceptionConstructor) where S : IMessageDescription { message.Exception = exceptionConstructor; return message; }
    /// <summary>Get default exception type name</summary>
    public static string? GetExceptionTypeName(this IMessageDescription messageDescription)
    {
        object? o = messageDescription.Exception;
        if (o == null) return null;
        if (o is string typename) return typename;
        if (o is Type type) return type.FullName;
        if (o is Delegate @delegate) return @delegate.Method?.ReturnType?.FullName;
        return null;
    }
    /// <summary>Link to the help Uniform Resource Name (URN) or Uniform Resource Locator (URL).</summary>
    public static S SetHelpLink<S>(this S message, string? helpLink) where S : IMessageDescription { message.HelpLink = helpLink; return message; }

    /// <summary>Is severity good</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGood(this IMessageDescription message) => (message.Code & StatusCodes.SeverityMask) == StatusCodes.Good;
    /// <summary>Is severity not good</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotGood(this IMessageDescription message) => (message.Code & StatusCodes.SeverityMask) != StatusCodes.Good;
    /// <summary>Is severity uncertain</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUncertain(this IMessageDescription message) => (message.Code & StatusCodes.SeverityMask) == StatusCodes.Uncertain;
    /// <summary>Is severity not uncertain</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotUncertain(this IMessageDescription message) => (message.Code & StatusCodes.SeverityMask) != StatusCodes.Uncertain;
    /// <summary>Is severity bad (bit 31)</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBad(this IMessageDescription message) => (message.Code & StatusCodes.Bad) == StatusCodes.Bad;
    /// <summary>Is severity not bad (bit 31)</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotBad(this IMessageDescription message) => (message.Code & StatusCodes.Bad) != StatusCodes.Bad;
    /// <summary>Is severity severe</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSevere(this IMessageDescription message) => (message.Code & StatusCodes.SeverityMask) == StatusCodes.Severe;
    /// <summary>Is severity not severe</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotSevere(this IMessageDescription message) => (message.Code & StatusCodes.SeverityMask) != StatusCodes.Severe;
    /// <summary>Get severity from code then hresult.</summary>
    /// <returns>0=unassigned, 1=good, 2=uncertain, 3=bad, 4=severe/critical</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public static int GetSeverityLevel(this IMessageDescription message) => message.Code.HasValue ? StatusCodes.GetSeverityLevel(message.Code.Value) : message.HResult.HasValue ? StatusCodes.GetSeverityLevel(message.HResult.Value) : 0;
}

