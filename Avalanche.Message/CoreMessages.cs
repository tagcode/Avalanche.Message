// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Message;
using Avalanche.Utilities;

/// <summary>Contains events and errors of this class library.</summary>
public class CoreMessages : MessageDescriptionsTable
{
    /// <summary>Base id for datatype codes. </summary>
    public const int BaseCode = CodeIds.Core;
    /// <summary>Singleton</summary>
    static CoreMessages instance = new CoreMessages();
    /// <summary>Singleton</summary>
    public static CoreMessages Instance => instance;


    /// <summary>Create new event info</summary>
    static MessageDescription good(int id, string key, string messageTemplate) => new MessageDescription("Avalanche.Core." + key, BaseCode | id | StatusCodes.Good, messageTemplate).SetHResult(BaseCode | id | StatusCodes.Good);
    /// <summary>Create new event info</summary>
    static MessageDescription uncertain(int id, string key, string messageTemplate) => new MessageDescription("Avalanche.Core." + key, BaseCode | id | StatusCodes.Uncertain, messageTemplate).SetHResult(BaseCode | id | StatusCodes.Uncertain);
    /// <summary>Create new event info</summary>
    static MessageDescription bad(int id, string key, string messageTemplate) => new MessageDescription("Avalanche.Core." + key, BaseCode | id | StatusCodes.Bad, messageTemplate).SetHResult(BaseCode | id | StatusCodes.Bad);

    //// ////

    //// Generic ////
    /// <summary>Valid result.</summary>
    public MessageDescription Good = good(0, nameof(Good), "Good status.");
    /// <summary>Status uncertain.</summary>
    public MessageDescription Uncertain = uncertain(0, nameof(Uncertain), "Status uncertain.");
    /// <summary>Captured unexpected <see cref="System.Exception"/>.</summary>
    public MessageDescription Bad = bad(0, nameof(Bad), "Bad status.");

    /// <summary>Captured unexpected <see cref="System.Exception"/>.</summary>
    public MessageDescription BadUnexpected = bad(1, nameof(BadUnexpected), "'{object}': Unexpected error");
    /// <summary>Error object is in read-only state.</summary>
    public MessageDescription BadReadOnly = new MessageDescription("InvalidOperation_ReadOnly", BaseCode | 2 | StatusCodes.Bad, "Instance is read-only.").SetException(typeof(InvalidOperationException)).SetHResult(0x80131509);
    /// <summary>Error that occurs when a disposed object is used.</summary>
    public MessageDescription BadDisposed = bad(3, nameof(BadDisposed), "{object} disposed.").SetException(typeof(ObjectDisposedException)).SetException((IMessage message) => new ObjectDisposedException(message?["object"]?.ToString(), message?.Error));

    //// Validation ////
    /// <summary>Error object is not valid.</summary>
    public MessageDescription BadNotValid = bad(5, nameof(BadNotValid), "'{object}': Not valid '{message}'").SetException(typeof(ValidationException));
    /// <summary>Uncertain validation result.</summary>
    public MessageDescription UncertainValidation = uncertain(5, nameof(UncertainValidation), "'{object}': Uncertain validatation '{message}'");
    /// <summary>Object is valid.</summary>
    public MessageDescription GoodValid = good(5, nameof(GoodValid), "'{object}': Is valid.");
    /// <summary>Malformed format string.</summary>
    public MessageDescription BadMalformed = bad(6, nameof(BadMalformed), "Malformed \"{string}\" at {startIx}:{endIx}").SetHResult(0x80131537).SetException(typeof(FormatException));

    /// <summary>Error object cannot be type casted.</summary>
    public MessageDescription BadNotCastable = bad(8, nameof(BadNotCastable), "'{object}': Could not type-cast to {targetType}");
    /// <summary>Unexpected 'null' value.</summary>
    public MessageDescription BadNull = bad(9, nameof(BadNull), "Unexpected null value").SetException(typeof(ArgumentNullException));
    /// <summary>Null argument.</summary>
    public MessageDescription BadNullArgument = bad(10, nameof(BadNullArgument), "'{object}': null argument '{argName}'").SetException(typeof(ArgumentNullException));
    /// <summary>Null property.</summary>
    public MessageDescription BadNullProperty = bad(11, nameof(BadNullProperty), "'{object}': Property '{propertyName}' has null value").SetException(typeof(ArgumentNullException));
    /// <summary>Null property element of array.</summary>
    public MessageDescription BadNullPropertyElement = bad(12, nameof(BadNullPropertyElement), "'{object}': Property '{propertyName}' element at {index} has null value");
}


