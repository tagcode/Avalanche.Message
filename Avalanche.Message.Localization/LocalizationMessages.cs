// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System.Runtime.CompilerServices;
using Avalanche.Message;
using Avalanche.Utilities;

/// <summary>Contains events and errors of this class library.</summary>
public class LocalizationMessages : MessageDescriptionsTable
{
    /// <summary>Base id for datatype codes. </summary>
    public const int BaseCode = CodeIds.DataType;
    /// <summary>Singleton</summary>
    static readonly LocalizationMessages instance = new LocalizationMessages();
    /// <summary>Singleton</summary>
    public static LocalizationMessages Instance => instance;

    /// <summary>Create new message description</summary>
    static MessageDescription bad(int id, [CallerArgumentExpression("id")] string? key = default)
    {
        if (key!.StartsWith("LocalizationMessageIds.")) key = key["LocalizationMessageIds.".Length..];
        return new MessageDescription("Avalanche.Localization." + key, id, key + " {Message} {Culture} {Key} {Position}").SetException(typeof(LocalizationException));
    }

    /// <summary>No "Key=..." key-value in localization line (error may be omited)</summary>
    public MessageDescription NoKey = bad(LocalizationMessageIds.NoKey);
    /// <summary>No "Text=..." key-value in localization line</summary>
    public MessageDescription NoText = bad(LocalizationMessageIds.NoText);
    /// <summary>No "TemplateFormat=..." key-value in localization line</summary>
    public MessageDescription NoTemplateFormat = bad(LocalizationMessageIds.NoTemplateFormat);
    /// <summary>No "Culture=..." key-value in localization line</summary>
    public MessageDescription NoCulture = bad(LocalizationMessageIds.NoCulture);
    /// <summary>Template format not found</summary>
    public MessageDescription TemplateFormatNotFound = bad(LocalizationMessageIds.TemplateFormatNotFound);
    /// <summary>Text malformed</summary>
    public MessageDescription TextMalformed = bad(LocalizationMessageIds.TextMalformed);
    /// <summary>Template format could not parse text</summary>
    public MessageDescription TextParseFailed = bad(LocalizationMessageIds.TextParseFailed);
    /// <summary>Plural rules not found.</summary>
    public MessageDescription PluralRulesNotFound = bad(LocalizationMessageIds.PluralRulesNotFound);
    /// <summary>Plurals failed to parse, "parameterName:category:case[:culture], ..." expected</summary>
    public MessageDescription PluralsParseFailed = bad(LocalizationMessageIds.PluralsParseFailed);
    /// <summary>Plurals refered to parameter that is not found in any localization line for the key</summary>
    public MessageDescription PluralsParameterNotFound = bad(LocalizationMessageIds.PluralsParameterNotFound);
    /// <summary>Duplicate line with same "Plurals" value</summary>
    public MessageDescription PluralsDuplicateAssignment = bad(LocalizationMessageIds.PluralsDuplicateAssignment);
    /// <summary>Duplicate "Plurals" assignment on a specific parameter name</summary>
    public MessageDescription PluralsParameterDuplicateAssignment = bad(LocalizationMessageIds.PluralsParameterDuplicateAssignment);
    /// <summary>Missing required plurality cases.</summary>
    public MessageDescription PluralsMissingCases = bad(LocalizationMessageIds.PluralsMissingCases);
    /// <summary>Key not found</summary>
    public MessageDescription KeyNotFound = bad(LocalizationMessageIds.KeyNotFound);
    /// <summary>Unexpected exception</summary>
    public MessageDescription Unexpected = bad(LocalizationMessageIds.Unexpected);
}
