// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Message;
using Avalanche.Localization;
using Avalanche.Template;
using Avalanche.Utilities;

/// <summary><see cref="IMessage"/> localization extensions.</summary>
public static class MessageDescriptionLocalizationExtensions
{
    /// <summary>Localize template text in message description</summary>
    public static ILocalization Localize(this ILocalization localization, IMessageDescription messageDescription, ICultureProvider? cultureProvider = null)
    {
        // Localize text
        messageDescription.Template = localization.Localize(messageDescription.Template, messageDescription.Key, cultureProvider);
        // Return localiation
        return localization;
    }

    /// <summary>Localize template text in message description</summary>
    public static MD Localize<MD>(this MD messageDescription, ILocalization localization, ICultureProvider? cultureProvider = null) where MD : IMessageDescription
    {
        // Localize text
        messageDescription.Template = localization.Localize(messageDescription.Template, messageDescription.Key, cultureProvider);
        // Return localiation
        return messageDescription;
    }

    /// <summary>Creates decoration that localizes text.</summary>
    public static IMessageDescription Localized(this IMessageDescription messageDescription, ILocalization localization, ICultureProvider? cultureProvider = null)
    {
        // Localized message description
        messageDescription = new LocalizedMessageDescription(messageDescription, localization, cultureProvider);
        // Return localiation
        return messageDescription;
    }

    /// <summary>Localize template text in message description</summary>
    public static ILocalization LocalizeMessages(this ILocalization localization, IEnumerable<IMessageDescription> messageDescriptions, ICultureProvider? cultureProvider = null)
    {
        // Localize texts
        foreach (IMessageDescription messageDescription in messageDescriptions)
            messageDescription.Template = localization.Localize(messageDescription.Template, messageDescription.Key, cultureProvider);
        // Return localiation
        return localization;
    }

    /// <summary>Localize template text in message description</summary>
    public static T LocalizeMessages<T>(this T messageDescriptions, ILocalization localization, ICultureProvider? cultureProvider = null) where T : IEnumerable<IMessageDescription>
    {
        // Localize texts
        foreach (IMessageDescription messageDescription in messageDescriptions)
            messageDescription.Template = localization.Localize(messageDescription.Template, messageDescription.Key, cultureProvider);
        // Return localiation
        return messageDescriptions;
    }

    /// <summary>Creates localized message description</summary>
    public static IEnumerable<IMessageDescription> LocalizedMessages<T>(this IEnumerable<IMessageDescription> messageDescriptions, ILocalization localization, ICultureProvider? cultureProvider = null)
    {
        // Localized texts
        foreach (IMessageDescription messageDescription in messageDescriptions)
            yield return new LocalizedMessageDescription(messageDescription, localization, cultureProvider);
    }

    /// <summary>Decorates Template with localization</summary>
    class LocalizedMessageDescription : IMessageDescription, IDecoration
    {
        /// <summary></summary>
        public bool IsDecoration { get => true; set => throw new InvalidOperationException(); }
        /// <summary></summary>
        public object? Decoree { get => messageDescription; set => throw new InvalidOperationException(); }
        /// <summary></summary>
        public int? Code { get => messageDescription.Code; set => messageDescription.Code = value; }
        /// <summary></summary>
        public int? HResult { get => messageDescription.HResult; set => messageDescription.HResult = value; }
        /// <summary></summary>
        public string Key { get => messageDescription.Key; set => messageDescription.Key = value; }
        /// <summary></summary>
        public MessageLevel? Severity { get => messageDescription.Severity; set => messageDescription.Severity = value; }
        /// <summary></summary>
        public string? Description { get => messageDescription.Description; set => messageDescription.Description = value; }
        /// <summary></summary>
        public string? HelpLink { get => messageDescription.HelpLink; set => messageDescription.HelpLink = value; }
        /// <summary></summary>
        public object? Exception { get => messageDescription.Exception; set => messageDescription.Exception = value; }
        /// <summary></summary>
        public bool HasUserData => messageDescription.HasUserData;
        /// <summary></summary>
        public bool UserDataInitializedOnGet => messageDescription.UserDataInitializedOnGet;
        /// <summary></summary>
        public IDictionary<string, object?> UserData { get => messageDescription.UserData; set => messageDescription.UserData = value; }

        /// <summary>Localize source text</summary>
        public ITemplateText Template
        {
            get
            {
                // Get cache line
                var _cache = cache;
                // Get source text
                ITemplateText? sourceText = messageDescription.Template;
                // No text in source
                if (sourceText == null) return null!;
                // Get cached text
                if (_cache.sourceText != null && _cache.sourceText == messageDescription.Template) return _cache.localized;
                // Get key
                string _key = messageDescription.Key;
                // No key
                if (_key == null) return sourceText;
                // Localize text
                ITemplateText localized = localization.Localize(sourceText, _key, cultureProvider);
                // Assign to cache
                cache = (sourceText, localized);
                // Return
                return localized;
            }
            set => messageDescription.Template = value;
        }

        /// <summary>Source message description</summary>
        protected IMessageDescription messageDescription;
        /// <summary>Localization context</summary>
        protected ILocalization localization;
        /// <summary>Localizing culture provider</summary>
        protected ICultureProvider? cultureProvider;
        /// <summary>Cached text</summary>
        protected (ITemplateText? sourceText, ITemplateText localized) cache;

        /// <summary></summary>
        public LocalizedMessageDescription(IMessageDescription messageDescription, ILocalization localization, ICultureProvider? cultureProvider)
        {
            this.messageDescription = messageDescription ?? throw new ArgumentNullException(nameof(messageDescription));
            this.localization = localization ?? throw new ArgumentNullException(nameof(localization));
            this.cultureProvider = cultureProvider;
        }
    }
}
