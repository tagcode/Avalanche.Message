// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Message;
using Avalanche.Template;
using Avalanche.Utilities;

// <docs>
/// <summary>Message description</summary>
public interface IMessageDescription : IUserDataContainer
{
    /// <summary>Numeric identifier in undetermined numbering schema.</summary>
    int? Code { get; set; }
    /// <summary>HResult code.</summary>
    int? HResult { get; set; }
    /// <summary>String identifier as canonical string, e.g. <![CDATA["MyLibrary.MyEvent"]]>.</summary>
    string Key { get; set; }
    /// <summary>Message template that has parameters as placeholders.</summary>
    ITemplateText Template { get; set; }
    /// <summary>Message severity level for logging.</summary>
    MessageLevel? Severity { get; set; }
    /// <summary>Description about event. May contain xml.</summary>
    string? Description { get; set; }
    /// <summary>Link to the help Uniform Resource Name (URN) or Uniform Resource Locator (URL).</summary>
    string? HelpLink { get; set; }
    /// <summary>Exception info as: <see cref="Type"/>, <see cref="string"/> or <see cref="Delegate"/> constructor.</summary>
    object? Exception { get; set; }
}
// </docs>


