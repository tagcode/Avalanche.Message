// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Avalanche.Template;
using Avalanche.Utilities;

/// <summary>Table of <see cref="IMessageDescription"/>.</summary>
public class MessageDescriptions : ReadOnlyAssignableClass, IEnumerable<IMessageDescription>, IMessageDescriptions
{
    /// <summary>Message descriptions by key</summary>
    protected Dictionary<string, IMessageDescription> keys = new();
    /// <summary>Message descriptions by hresult</summary>
    protected MapList<int, IMessageDescription> hresults = new();
    /// <summary>Message descriptions by code</summary>
    protected Dictionary<int, IMessageDescription> codes = new();
    /// <summary>Message descriptions</summary>
    protected ArrayList<IMessageDescription> list = new ArrayList<IMessageDescription>.Sorted(MessageDescriptionComparer.Instance);

    /// <summary>Message descriptions by key</summary>
    protected IDictionary<string, IMessageDescription> keysReadOnly;
    /// <summary>Message descriptions by hresult</summary>
    protected IDictionary<int, List<IMessageDescription>> hresultsReadOnly;
    /// <summary>Message descriptions by code</summary>
    protected IDictionary<int, IMessageDescription> codesReadOnly;

    /// <summary>Message descriptions by key</summary>
    public virtual IDictionary<string, IMessageDescription> Keys { get => keysReadOnly; set => throw new InvalidOperationException(); }
    /// <summary>Message descriptions by hresult</summary>
    public virtual IDictionary<int, List<IMessageDescription>> HResults { get => hresultsReadOnly; set => throw new InvalidOperationException(); }
    /// <summary>Message descriptions by code</summary>
    public virtual IDictionary<int, IMessageDescription> Codes { get => codesReadOnly; set => throw new InvalidOperationException(); }
    /// <summary>Message descriptions</summary>
    public virtual IList<IMessageDescription> List { get => list.Array; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    protected object mLock = new();

    /// <summary></summary>
    public MessageDescriptions() : base()
    {
        keysReadOnly = new ReadOnlyDictionary<string, IMessageDescription>(keys);
        hresultsReadOnly = new ReadOnlyDictionary<int, List<IMessageDescription>>(hresults);
        codesReadOnly = new ReadOnlyDictionary<int, IMessageDescription>(codes);
    }

    /// <summary></summary>
    public MessageDescriptions(params IMessageDescription[] messageDescriptions) : this()
    {
        foreach (var messageDescription in messageDescriptions) Add(messageDescription);
    }

    /// <summary></summary>
    public MessageDescriptions(params IEnumerable<IMessageDescription>[] messageDescriptionSources) : this()
    {
        foreach (var messageDescriptionSource in messageDescriptionSources) 
            foreach (var messageDescription in messageDescriptionSource)
                Add(messageDescription);
    }

    /// <summary>Initialize field values</summary>
    public virtual MessageDescriptions Initialize() { return this; }

    /// <summary>Add <paramref name="messageDescription"/>.</summary>
    /// <exception cref="InvalidOperationException">On key or code collision.</exception>
    void IMessageDescriptions.Add(IMessageDescription messageDescription) => Add(messageDescription);

    /// <summary>Add <paramref name="messageDescription"/>.</summary>
    /// <exception cref="InvalidOperationException">On key or code collision.</exception>
    public MessageDescriptions Add(IMessageDescription messageDescription)
    {
        // Lock
        lock (mLock)
        {
            //
            this.AssertWritable();
            // Assert 
            if (messageDescription.Code.HasValue && Codes.ContainsKey(messageDescription.Code.Value)) throw new InvalidOperationException($"{nameof(MessageDescription)} by id {messageDescription.Code} already existed.");
            if (!String.IsNullOrEmpty(messageDescription.Key) && Keys.ContainsKey(messageDescription.Key)) throw new InvalidOperationException($"{nameof(MessageDescription)} by key {messageDescription.Key} already existed.");

            // Add to id map
            if (messageDescription.Code.HasValue)
            {
                if (!codes.TryAdd(messageDescription.Code.Value, messageDescription)) throw new InvalidOperationException($"{nameof(MessageDescription)} by code {messageDescription.Code} already existed.");
            }
            // Add to key map
            if (!String.IsNullOrEmpty(messageDescription.Key))
            {
                if (!keys.TryAdd(messageDescription.Key, messageDescription)) throw new InvalidOperationException($"{nameof(MessageDescription)} by key {messageDescription.Key} already existed.");
            }
            // Add to hresult
            int? hresult = messageDescription.HResult;
            if (hresult.HasValue) hresults.Add(hresult.Value, messageDescription);
            // Add to list
            list.Add(messageDescription);
        }
        //
        return this;
    }

    /// <summary>Add <paramref name="messageDescriptions"/> to <see cref="List"/>, and possibly to <see cref="Keys"/> and <see cref="Codes"/>.</summary>
    /// <param name="messageDescriptions"></param>
    /// <exception cref="InvalidOperationException">If messageDescription by same key or id exists</exception>
    public MessageDescriptions AddRange(IEnumerable<IMessageDescription> messageDescriptions)
    {
        // Add each
        foreach (IMessageDescription messageDescription in messageDescriptions) Add(messageDescription);
        // Return
        return this;
    }

    /// <summary>Create and add new <see cref="IMessageDescription"/> and put into readonly state</summary>
    public virtual IMessageDescription New(string key, int code, string templateText)
    {
        // Create
        MessageDescription messageDescription = new MessageDescription(key, code, templateText);
        // Read-only
        //messageDescription.SetReadOnly();
        // Add
        Add(messageDescription);
        // Return
        return messageDescription;
    }


    /// <summary></summary>
    public IEnumerator<IMessageDescription> GetEnumerator() => ((IEnumerable<IMessageDescription>)list.Array).GetEnumerator();
    /// <summary></summary>
    IEnumerator IEnumerable.GetEnumerator() => list.Array.GetEnumerator();

    /// <summary>Print as markdown table.</summary>
    public override string ToString()
    {
        // Start building string
        StringBuilder sb = new();
        // Get snapshot
        IMessageDescription[] messageDescriptions = list.Array;
        // Append header
        sb.AppendLine("| Key                                                                             |StatusCode| Description                                                                 | Message Template                                                | Exception           |");
        sb.AppendLine("|:--------------------------------------------------------------------------------|:---------|:----------------------------------------------------------------------------|:----------------------------------------------------------------|:--------------------|");
        foreach (IMessageDescription messageDescription in messageDescriptions)
            sb.AppendLine($"| {(messageDescription.Key ?? "").PadRight(80)}| {messageDescription.Code:X8} | {Escape(messageDescription.Description).PadRight(76)}| {(messageDescription.Template.Text ?? "").PadRight(64)}| {(messageDescription.GetExceptionTypeName() ?? "").PadRight(20)}|");
        sb.AppendLine();

        // Print
        return sb.ToString();
    }

    /// <summary>Escape for <see cref="ToString"/></summary>
    protected virtual string Escape(string? text)
    {
        // Escape to html
        text = HttpUtility.HtmlEncode(text ?? "");
        // Convert linefeeds to <br/>
        text = text.Replace("\n", "<br/>").Replace("\r", "");
        // &lt;see cref="T:Avalanche.Service.IEntry" /&gt; -> <span style="color:white;">ToCache</span>
        text = Regex.Replace(text, @"&lt;see cref=&quot;.:([^&()]*)\.([^\.&()#`]*)([^&]*)&quot; /&gt;", s => $"<em>{s.Groups[2].Value}</em>");
        // Return escaped
        return text;
    }
}

/// <summary>Stores <see cref="IMessageDescription"/> in a map.</summary>
public abstract class MessageDescriptionsT : MessageDescriptions
{
    /// <summary>MessageDescription info type</summary>
    public abstract Type MessageDescriptionType { get; }
}

/// <summary>Stores <see cref="IMessageDescription"/> in a map.</summary>
public class MessageDescriptions<T> : MessageDescriptionsT where T : IMessageDescription, new()
{
    /// <summary>Status code type</summary>
    public override Type MessageDescriptionType => typeof(T);

    /// <summary>Create and add new <typeparamref name="T"/>.</summary>
    public override IMessageDescription New(string key, int code, string messageTemplate)
    {
        // Create
        T @messageDescription = new T { Key = key, Code = code, Template = TemplateFormat.Brace.Breakdown[messageTemplate] };
        // Read-only
        //if (@messageDescription is IReadOnly readonlyAssignable) readonlyAssignable.ReadOnly = true;
        // Add
        Add(@messageDescription);
        // Return
        return @messageDescription;
    }
}
