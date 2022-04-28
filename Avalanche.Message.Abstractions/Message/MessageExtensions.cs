// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Message;
using System.Runtime.CompilerServices;
using System.Text;
using Avalanche.Template;
using Avalanche.Utilities;

/// <summary>Extension methods for <see cref="IMessage"/></summary>
public static class MessageExtensions
{
    /// <summary>Return message code.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int? Code(this IMessage message) => message.MessageDescription.Code;

    /// <summary>Set event id</summary>
    public static S SetId<S>(this S message, object id) where S : IMessage { message.Id = id; return message; }
    /// <summary>Set <paramref name="messageDescription"/>.</summary>
    public static S SetMessageDescription<S>(this S message, IMessageDescription messageDescription) where S : IMessage { message.MessageDescription = messageDescription; return message; }
    /// <summary>Set <paramref name="arguments"/>.</summary>
    public static S SetArguments<S>(this S message, params object?[]? arguments) where S : IMessage { message.Arguments = arguments!; return message; }
    /// <summary></summary>
    public static S SetTime<S>(this S message, DateTime? time) where S : IMessage { message.Time = time; return message; }
    /// <summary></summary>
    public static S SetNow<S>(this S message) where S : IMessage { message.Time = DateTime.Now; return message; }
    /// <summary></summary>
    public static S SetError<S>(this S message, Exception? error) where S : IMessage { message.Error = error; return message; }
    /// <summary>Message severity information for logging.</summary>    
    public static S SetSeverity<S>(this S message, MessageLevel severity) where S : IMessage { message.Severity = severity; return message; }
    /// <summary>Set <paramref name="innerMessage"/>.</summary>
    public static S SetInnerMessage<S>(this S message, IMessage innerMessage) where S : IMessage { message.InnerMessages = new IMessage[] { innerMessage }; return message; }
    /// <summary>Set <paramref name="innerMessages"/>.</summary>
    public static S SetInnerMessages<S>(this S message, params IMessage[]? innerMessages) where S : IMessage { message.InnerMessages = innerMessages; return message; }

    /// <summary>Is severity good</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGood(this IMessage message) => (message.Code() & StatusCodes.SeverityMask) == StatusCodes.Good;
    /// <summary>Is severity not good</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotGood(this IMessage message) => (message.Code() & StatusCodes.SeverityMask) != StatusCodes.Good;
    /// <summary>Is severity bad (bit 31)</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBad(this IMessage message) => (message.Code() & StatusCodes.Bad) == StatusCodes.Bad;
    /// <summary>Is severity not bad (bit 31)</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotBad(this IMessage message) => (message.Code() & StatusCodes.Bad) != StatusCodes.Bad;
    /// <summary>Is severity uncertain</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUncertain(this IMessage message) => (message.Code() & StatusCodes.SeverityMask) == StatusCodes.Uncertain;
    /// <summary>Is severity not uncertain</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotUncertain(this IMessage message) => (message.Code() & StatusCodes.SeverityMask) != StatusCodes.Uncertain;

    /// <summary>Print <paramref name="message"/> to string</summary>
    /// <exception cref="InvalidOperationException">If print is not possible.</exception>
    public static string Print(this IMessage message, IFormatProvider? formatProvider = null) => message?.MessageDescription?.Template?.Print(formatProvider, message.Arguments) ?? "";

    /// <summary>Print <paramref name="message"/> to string</summary>
    /// <returns>Number of characters written to <paramref name="dst"/>, or -1 if failed</returns>
    public static int PrintTo(this IMessage message, Span<char> dst, IFormatProvider? formatProvider)
    {
        // Get text
        ITemplateText? templateText = message?.MessageDescription?.Template;
        //
        if (templateText == null) return -1;
        // Try print
        if (templateText.TryPrintTo(dst, out int length, formatProvider, message!.Arguments)) return length;
        // Failed
        return -1;
    }

    /// <summary>Print <paramref name="message"/> to string</summary>
    /// <param name="length">Number of characters written to <paramref name="dst"/>.</param>
    /// <returns>True if text was written, false if write failed.</returns>
    public static bool TryPrintTo(this IMessage message, Span<char> dst, out int length, IFormatProvider? formatProvider)
    {
        // Get text
        ITemplateText? templateText = message?.MessageDescription?.Template;
        //
        if (templateText == null) { length = 0; return false; }
        // Try print
        if (templateText.TryPrintTo(dst, out length, formatProvider, message!.Arguments)) return true;
        // Failed
        length = 0;
        return false;
    }

    /// <summary>Try estimate print length</summary>
    /// <exception cref="InvalidOperationException">If estimate fialed.</exception>
    public static int EstimatePrintLength(this IMessage message, IFormatProvider? formatProvider)
    {
        // Get text
        ITemplateText? templateText = message?.MessageDescription?.Template;
        // Try estimate
        if (templateText != null && templateText.TryEstimatePrintLength(out int length, formatProvider, message!.Arguments)) return length;
        // Failed
        throw new InvalidOperationException("Failed to estimate length.");
    }

    /// <summary>Print <paramref name="message"/> to <paramref name="sb"/></summary>
    /// <exception cref="InvalidOperationException">If print is not possible.</exception>
    public static IMessage AppendTo(this IMessage message, StringBuilder sb, IFormatProvider? formatProvider)
    {
        // Assert message
        if (message == null) throw new ArgumentNullException(nameof(message));
        // Get text
        ITemplateText? templateText = message?.MessageDescription?.Template;
        // Assert template text
        if (templateText == null) throw new InvalidOperationException("No template text");
        // Append
        templateText.AppendTo(sb, formatProvider, message!.Arguments);
        // Return
        return message;
    }

    /// <summary>Print <paramref name="message"/> to <paramref name="sb"/></summary>
    /// <exception cref="InvalidOperationException">If print is not possible.</exception>
    public static bool TryAppendTo(this IMessage message, StringBuilder sb, IFormatProvider? formatProvider)
    {
        // Assert message
        if (message == null) return false;
        // Get text
        ITemplateText? templateText = message?.MessageDescription?.Template;
        // Assert template text
        if (templateText == null) return false;
        // Append
        templateText.AppendTo(sb, formatProvider, message!.Arguments);
        // Return
        return true;
    }

    /// <summary>Print <paramref name="message"/> to <paramref name="textWriter"/></summary>
    /// <exception cref="InvalidOperationException">If print is not possible.</exception>
    public static IMessage WriteTo(this IMessage message, TextWriter textWriter, IFormatProvider? formatProvider)
    {
        // Assert message
        if (message == null) throw new ArgumentNullException(nameof(message));
        // Get text
        ITemplateText? templateText = message?.MessageDescription?.Template;
        // Assert template text
        if (templateText == null) throw new InvalidOperationException("No template text");
        // Write
        templateText.WriteTo(textWriter, formatProvider, message!.Arguments);
        // Return
        return message;
    }

    /// <summary>Print <paramref name="message"/> to <paramref name="textWriter"/></summary>
    /// <exception cref="InvalidOperationException">If print is not possible.</exception>
    public static bool TryWriteTo(this IMessage message, TextWriter textWriter, IFormatProvider? formatProvider)
    {
        // Assert message
        if (message == null) return false;
        // Get text
        ITemplateText? templateText = message?.MessageDescription?.Template;
        // Assert template text
        if (templateText == null) return false;
        // Write
        templateText.WriteTo(textWriter, formatProvider, message!.Arguments);
        // Return
        return true;
    }
}
