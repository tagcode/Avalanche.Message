// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Message.Internal;
using System.Runtime.CompilerServices;
using Avalanche.Template;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary></summary>
[InterpolatedStringHandler]
public ref struct MessageInterpolatedStringHandler
{
    /// <summary>Cache of integer to string prints.</summary>
    static readonly IProvider<int, string> intInterner = Providers.Func((int i) => i.ToString()).Cached();
    /// <summary>Place here parts</summary>
    public StructList8<ITemplatePart> parts = new();
    /// <summary>Place here placeholders</summary>
    public ITemplatePlaceholderPart[] placeholders;
    /// <summary>Place here argument values</summary>
    public object?[] arguments;

    /// <summary>Index for <see cref="placeholders"/> and <see cref="arguments"/></summary>
    int parameterIx = 0;

    /// <summary>Create handler</summary>
    /// <param name="literalLength"></param>
    /// <param name="formattedCount"></param>
    /// <param name="message">message object</param>
    public MessageInterpolatedStringHandler(int literalLength, int formattedCount, object message)
    {
        placeholders = new ITemplatePlaceholderPart[formattedCount];
        arguments = new object?[formattedCount];
    }

    /// <summary>Append literal</summary>
    public void AppendLiteral(string s)
    {
        // Create text part
        ITemplateTextPart text = new TemplateTextPart().SetTexts(s, Escaper.Brace);
        // Add to parts
        parts.Add(text);
    }

    /// <summary>Append parameter and argument</summary>
    public void AppendFormatted<T>(T value)
    {
        // "0"
        string parameterName = intInterner[parameterIx];
        // Create parameter
        ITemplateParameterPart parameter = new TemplateParameterPart { ParameterIndex = parameterIx }.SetTexts(parameterName);
        // Create placeholder
        ITemplatePlaceholderPart placeholder = new TemplatePlaceholderPart { Parameter = parameter }.SetTexts(parameterName);
        // Add part
        parts.Add(placeholder);
        arguments[parameterIx] = value;
        placeholders[parameterIx++] = placeholder;
    }

    /// <summary>Append parameter and argument</summary>
    public void AppendFormatted<T>(T value, int alignment)
    {
        // "0"
        string parameterName = intInterner[parameterIx];
        // Create parameter
        ITemplateParameterPart parameter = new TemplateParameterPart { ParameterIndex = parameterIx }.SetTexts(parameterName);
        // Create alignment
        ITemplateAlignmentPart? alignment1 = alignment == 0 ? null : new TemplateAlignmentPart { Alignment = alignment }.SetTexts(intInterner[alignment]);
        // Create placeholder
        ITemplatePlaceholderPart placeholder = new TemplatePlaceholderPart { Parameter = parameter, Alignment = alignment1 }.SetTexts(parameterName);
        // Add part
        parts.Add(placeholder);
        arguments[parameterIx] = value;
        placeholders[parameterIx++] = placeholder;
    }

    /// <summary>Append parameter and argument</summary>
    public void AppendFormatted<T>(T value, string? format)
    {
        // "0"
        string parameterName = intInterner[parameterIx];
        // Create parameter
        ITemplateParameterPart parameter = new TemplateParameterPart { ParameterIndex = parameterIx }.SetTexts(parameterName);
        // Create format
        ITemplateFormattingPart? format1 = format == null ? null : new TemplateFormattingPart().SetTexts(format);
        // Create placeholder
        ITemplatePlaceholderPart placeholder = new TemplatePlaceholderPart { Parameter = parameter, Formatting = format1 }.SetTexts(parameterName);
        // Add part
        parts.Add(placeholder);
        arguments[parameterIx] = value;
        placeholders[parameterIx++] = placeholder;
    }

    /// <summary>Append parameter and argument</summary>
    public void AppendFormatted<T>(T value, int alignment, string? format)
    {
        // "0"
        string parameterName = intInterner[parameterIx];
        // Create parameter
        ITemplateParameterPart parameter = new TemplateParameterPart().SetTexts(parameterName).SetParameterIndex(parameterIx);
        // Create alignment
        ITemplateAlignmentPart? alignment1 = alignment == 0 ? null : new TemplateAlignmentPart().SetTexts(intInterner[alignment]).SetAlignment(alignment);
        // Create format
        ITemplateFormattingPart? format1 = format == null ? null : new TemplateFormattingPart().SetTexts(format);
        // Create placeholder
        ITemplatePlaceholderPart placeholder = new TemplatePlaceholderPart().SetTexts(parameterName, Escaper.Brace).SetParameter(parameter).SetAlignment(alignment1).SetFormatting(format1);
        // Add part
        parts.Add(placeholder);
        arguments[parameterIx] = value;
        placeholders[parameterIx++] = placeholder;
    }

    /// <summary>Template breakdown</summary>
    ITemplateBreakdown? templateBreakdown = null;
    /// <summary>Template breakdown</summary>
    public ITemplateBreakdown TemplateBreakdown => templateBreakdown ?? (templateBreakdown = createTemplateBreakdown());

    /// <summary>Create breakdown</summary>
    ITemplateBreakdown createTemplateBreakdown()
    {
        // Create breakdown
        TemplateBreakdown templateBreakdown = new LazyTemplateBreakdown
        {
            Parts = parts.ToArray(),
            Placeholders = placeholders.ToArray(),
        };
        // Finalize parts
        foreach(ITemplatePart part in parts)
        {
            // Finalize placeholder
            if (part is ITemplatePlaceholderPart placeholder)
            {
                //if (placeholder.Alignment is IReadOnly readonly0) readonly0.ReadOnly = true;
                //if (placeholder.Parameter is IReadOnly readonly1) readonly1.ReadOnly = true;
                //if (placeholder.Formatting is IReadOnly readonly2) readonly2.ReadOnly = true;
            }
            // Assign read-only
            //if (part is IReadOnly readonly3) readonly3.SetReadOnly();
        }
        // Create and assign text
        //templateBreakdown.Text = TemplateFormat.BraceNumeric.AssembleCached[templateBreakdown];
        //
        templateBreakdown.TemplateFormat = TemplateFormat.BraceNumeric;
        //
        return templateBreakdown;
    }

    /// <summary>Customized template breakdown that lazily creates text.</summary>
    public class LazyTemplateBreakdown : TemplateBreakdown
    {
        /// <summary>Escaped format string "Welcome, \"{0}\". You received {1,15:N0} credit(s).</summary>
        public override string Text { get => text ?? (text = createText()!); set => this.AssertWritable().text = value; }

        /// <summary>Lazy construction of text</summary>
        /// <returns></returns>
        protected virtual string? createText()
        {
            // Get format
            ITemplateFormat? templateFormat = TemplateFormat;
            // No format
            if (templateFormat == null) return null;
            // Assemble template text
            return templateFormat.Assemble[this];
        }
    }
}
