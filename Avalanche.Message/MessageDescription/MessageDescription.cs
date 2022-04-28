// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Message;
using System;
using System.Runtime.Serialization;
using Avalanche.Template;
using Avalanche.Utilities;

/// <summary>Application event message description</summary>
public class MessageDescription : ReadOnlyAssignableClass.UserDataContainerLazyConstructed, IMessageDescription, ISerializable, IReadOnly
{
    /// <summary>Numericsl identifier</summary>
    protected int? code;
    /// <summary>HResult code</summary>
    protected int? hresult;
    /// <summary>Status code string identifier</summary>
    protected string key;
    /// <summary>Message template format where placeholders are named, e.g. "{entry}: {exception}". Compatible with ILogger frameworks.</summary>
    protected ITemplateText templateText;
    /// <summary>Message severity information for logging.</summary>
    protected MessageLevel? severity;
    /// <summary>Description about event. May contain xml.</summary>
    protected string? description;
    /// <summary>Exception info as: <see cref="Type"/>, <see cref="string"/> or <see cref="Delegate"/> constructor.</summary>
    protected object? exception;
    /// <summary>Link to the help Uniform Resource Name (URN) or Uniform Resource Locator (URL).</summary>
    protected string? helpLink;

    /// <summary>Numerical identifier</summary>
    public virtual int? Code { get => code; set => this.AssertWritable().code = value; }
    /// <summary>HResult</summary>
    public virtual int? HResult { get => hresult; set => this.AssertWritable().hresult = value; }
    /// <summary>Status code string identifier</summary>
    public virtual string Key { get => key; set => this.AssertWritable().key = value; }
    /// <summary>Message template format where arguments are named, e.g. "{entry}: {exception}". Compatible with ILogger frameworks.</summary>
    public virtual ITemplateText Template { get => templateText; set => this.AssertWritable().templateText = value; }
    /// <summary>Message severity information for logging.</summary>
    public virtual MessageLevel? Severity { get => severity; set => this.AssertWritable().severity = value; }
    /// <summary>Description about event. May contain xml.</summary>
    public virtual string? Description { get => description; set => this.AssertWritable().description = value; }
    /// <summary>Exception info as: <see cref="Type"/>, <see cref="string"/> or <see cref="Delegate"/> constructor.</summary>
    public virtual object? Exception { get => exception; set => this.AssertWritable().exception = value; }
    /// <summary>Link to the help Uniform Resource Name (URN) or Uniform Resource Locator (URL).</summary>
    public virtual string? HelpLink { get => helpLink; set => this.AssertWritable().helpLink = value; }

    /// <summary>Create empty uninitialized service event.</summary>
    public MessageDescription()
    {
        key = null!;
        templateText = null!;
    }

    /// <summary>Create service event.</summary>
    /// <param name="key"></param>
    /// <param name="code">Code between -2147483648 .. 4294967295u</param>
    /// <param name="template">Message template</param>
    public MessageDescription(string key, long? code, ITemplateText template)
    {
        // Assign values
        this.code = code == null ? null : code >= int.MinValue && code <= uint.MaxValue ? unchecked((int)code) : throw new ArgumentException(nameof(code));
        this.key = key;
        this.templateText = template;
    }

    /// <summary>Create service event.</summary>
    /// <param name="key"></param>
    /// <param name="code">Code between -2147483648 .. 4294967295u</param>
    /// <param name="messageTemplate">Message template format, where arguments are named, e.g. "{entry}: {exception}".</param>
    public MessageDescription(string key, long? code, string messageTemplate)
    {
        // Assign values
        this.code = code == null ? null : code >= int.MinValue && code <= uint.MaxValue ? unchecked((int)code) : throw new ArgumentOutOfRangeException(nameof(code));
        this.key = key;
        this.templateText = TemplateFormat.Brace.Breakdown[messageTemplate];
    }

    /// <summary>Deserialize exception</summary>
    protected MessageDescription(SerializationInfo info, StreamingContext context)
    {
        this.code = info.GetInt32(nameof(Code));
        this.key = info.GetString(nameof(Key))!;
        this.description = info.GetString(nameof(Description))!;
        this.severity = info.GetValue(nameof(Severity), typeof(MessageLevel)) is MessageLevel msg ? msg : null;
        this.templateText = TemplateFormat.Brace.Breakdown[info.GetString(nameof(Template)) ?? ""];
    }

    /// <summary>Serialize exception</summary>
    public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue(nameof(Code), Code);
        info.AddValue(nameof(Key), Key);
        info.AddValue(nameof(Description), Description);
        info.AddValue(nameof(Severity), Severity);
        info.AddValue(nameof(Template), TemplateFormat.Brace.Assemble[this.Template.Breakdown]);
    }
    /// <summary>Print information</summary>
    public override string ToString() => $"MessageDescription(Code={Code:X8}, Key={Key}, HResult={HResult:X8}, Severity={Severity}, Template=\"{Template}\", \"{Description}\")";
    /// <summary></summary>
    public override int GetHashCode() => MessageDescriptionComparer.Instance.GetHashCode();
    /// <summary></summary>
    public override bool Equals(object? obj) => MessageDescriptionComparer.Instance.Equals(obj);

}
