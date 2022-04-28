// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Message;

/// <summary>Extension methods for <see cref="ILocalizationError"/></summary>
public static class LocalizationErrorMessageExtensions
{
    /// <summary>Convert to message</summary>
    public static IMessage AsMessage(this ILocalizationError localizationError)
        => new Message
        {
            MessageDescription = LocalizationMessages.Instance.Codes[localizationError.Code],
            Severity = MessageLevel.Error,
            Arguments = new object?[] { localizationError.Message, localizationError.Culture, localizationError.Key, localizationError.Text.Position }
        };
}
