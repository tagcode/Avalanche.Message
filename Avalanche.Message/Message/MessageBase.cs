// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Message;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Avalanche.Message.Internal;
using Avalanche.Template;
using Avalanche.Utilities;

/// <summary>Message record</summary>
public class MessageBase : FormattableString, IReadOnly, IMessage//, ISerializable
{
    /// <summary>Is read-only state</summary>
    [IgnoreDataMember] protected bool @readonly;
    /// <summary>Is read-only state</summary>
    [IgnoreDataMember] bool IReadOnly.ReadOnly { get => @readonly; set { if (@readonly == value) return; if (!value) throw new InvalidOperationException("Read-only"); if (value) setReadOnly(); } }

    /// <summary>User-data</summary>
    protected IDictionary<string, object?>? userdata;
    /// <summary>Is <see cref="UserData"/> assigned.</summary>
    bool IUserDataContainer.HasUserData => userdata != null;
    /// <summary>Policy whether this implementation constructs <see cref="UserData"/> lazily.</summary>
    bool IUserDataContainer.UserDataInitializedOnGet => true;
    /// <summary>Lock object for concurrency</summary>
    protected object mLock = new object();
    /// <summary>User-data</summary>
    public virtual IDictionary<string, object?> UserData { get => userdata ?? getOrCreateUserData(); set => this.AssertWritable().userdata = value; }
    /// <summary>Get-or-create data</summary>
    protected virtual IDictionary<string, object?> getOrCreateUserData()
    {
        // Get reference
        var _userdata = this.userdata;
        // Got reference
        if (_userdata != null) return _userdata;
        lock (mLock)
        {
            // Get reference again
            _userdata = this.userdata;
            // Got reference
            if (_userdata != null) return _userdata;
            // Create 
            _userdata = this.userdata = new LockableDictionary<string, object?>(new ConcurrentDictionary<string, object?>());
            // Copy read-only state
            ((IReadOnly)_userdata).ReadOnly = this.@readonly;
            // Return
            return _userdata;
        }
    }
    /// <summary>Assign into read-only state</summary>
    protected virtual void setReadOnly()
    {
        // Lock dictionary
        lock (mLock)
        {
            // Get reference again
            var _userdata = this.userdata;
            // Assign map
            if (_userdata != null) ((LockableDictionary<string, object?>)_userdata).SetReadOnly();
        }
    }

    /// <summary>Get argument count</summary>
    public override int ArgumentCount => (Arguments ?? Array.Empty<object>()).Length;
    /// <summary>Get argument</summary>
    public override object? GetArgument(int index) => (Arguments ?? Array.Empty<object>())[index];
    /// <summary>Get arguments</summary>
    /// <exception cref="NotImplementedException"></exception>
    public override object?[] GetArguments() => (Arguments ?? Array.Empty<object>());
    /// <summary>Get string.Format compatible format</summary>
    public override string Format => messageDescription?.Template?.Breakdown?.FormatTemplate() ?? "";

    /// <summary>Event identification, such as <see cref="int"/> or <see cref="Guid"/></summary>
    protected object? id;
    /// <summary>Message description.</summary>
    protected IMessageDescription messageDescription = null!;
    /// <summary></summary>
    protected object?[] arguments = null!;
    /// <summary>Event occurance time.</summary>
    protected DateTime? time;
    /// <summary>Message severity level for logging.</summary>
    protected MessageLevel? severity;
    /// <summary>Captured error</summary>
    protected Exception? error;
    /// <summary>Inner messages</summary>
    protected IMessage[]? innerMessages;

    /// <summary>Invalidate cache. Override this if sub-class uses caching.</summary>
    protected virtual MessageBase InvalidateCache { get { return this; } }

    /// <summary>Event identification, such as <see cref="int"/> or <see cref="Guid"/></summary>
    public virtual object? Id { get => id; set => this.AssertWritable().InvalidateCache.id = value; }
    /// <summary>Message description.</summary>
    public virtual IMessageDescription MessageDescription { get => messageDescription; set => this.AssertWritable().InvalidateCache.messageDescription = value; }
    /// <summary></summary>
    public virtual object?[] Arguments { get => arguments; set => this.AssertWritable().InvalidateCache.arguments = value; }
    /// <summary>Event occurance time.</summary>
    public virtual DateTime? Time { get => time; set => this.AssertWritable().InvalidateCache.time = value; }
    /// <summary>Message severity level for logging.</summary>
    public virtual MessageLevel? Severity { get => severity; set => this.AssertWritable().severity = value; }
    /// <summary>Captured error</summary>
    public virtual Exception? Error { get => error; set => this.AssertWritable().InvalidateCache.error = value; }
    /// <summary>Inner messages</summary>
    public virtual IMessage[]? InnerMessages { get => innerMessages; set => this.AssertWritable().InvalidateCache.innerMessages = value; }

    /// <summary>Create message</summary>
    public MessageBase() : base() { }

    /// <summary>Create message entry with arguments</summary>
    public MessageBase(IMessageDescription messageDescription, params object?[] arguments) : base()
    {
        this.messageDescription = messageDescription;
        this.arguments = arguments;
    }

    /// <summary>Copy message from <paramref name="copyFrom"/>.</summary>
    public MessageBase(IMessage copyFrom) : base()
    {
        this.messageDescription = copyFrom.MessageDescription;
        this.arguments = copyFrom.Arguments;
        this.id = copyFrom.Id;
        this.time = copyFrom.Time;
        this.severity = copyFrom.Severity;
        this.error = copyFrom.Error;
        this.innerMessages = copyFrom.InnerMessages;
        if (copyFrom.HasUserData) foreach (KeyValuePair<string, object?> line in copyFrom.UserData) this.UserData[line.Key] = line.Value;
    }

    /// <summary>Set from <paramref name="interpolatedString"/>.</summary>
    /// <param name="interpolatedString"></param>
    public virtual MessageBase SetInterpolated([InterpolatedStringHandlerArgument("")] ref MessageInterpolatedStringHandler interpolatedString)
    {
        // Create template text
        ITemplateText text = interpolatedString.TemplateBreakdown;
        // Create message description
        MessageDescription = new MessageDescription().SetTemplate(text);
        // Assign arguments
        Arguments = interpolatedString.arguments.ToArray();
        // Return self
        return this;
    }

    /// <summary>Deserialize exception</summary>
    protected MessageBase(SerializationInfo info, StreamingContext context) : base()
    {
        this.messageDescription = (MessageDescription)info.GetValue(nameof(MessageDescription), typeof(MessageDescription))!;
        this.arguments = (info.GetValue(nameof(Arguments), typeof(object?[])) as object?[]) ?? new object?[0];
    }

    /// <summary>Serialize exception</summary>
    public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue(nameof(MessageDescription), MessageDescription);
        object?[] arguments_as_strings = Arguments.Select<object?, string>(a => a?.ToString() ?? "").ToArray();
    }

    /// <summary>Formulate arguments into status code template</summary>
    public override string ToString() => messageDescription?.Template?.Print(null, Arguments) ?? "";
    /// <summary></summary>
    public override string ToString(IFormatProvider? formatProvider) => messageDescription?.Template?.Print(formatProvider, Arguments) ?? "";
    /// <summary>Formulate arguments into status code template</summary>
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        // Get text
        ITemplateText? text = messageDescription?.Template;
        // No text
        if (text == null) { charsWritten = 0; return false; }
        // Try print
        bool ok = text.TryPrintTo(destination, out charsWritten, provider, this.Arguments);
        // Return status
        return ok;
    }
    /// <summary>Formulate arguments into status code template</summary>
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        // Get text
        ITemplateText? text = messageDescription?.Template;
        // No text
        if (text == null) return "";
        //
        string print = text.Print(formatProvider, this.Arguments) ?? "";
        //
        return print;
    }

    /// <summary>Arguments, array guaranteed</summary>
    protected object?[] _Arguments => Arguments ?? Array.Empty<object>();
    /// <summary>Parameter namesm array guaranteed</summary>
    protected string[] _ParameterNames => MessageDescription?.Template?.GetNonNullParameterNames().ToArray() ?? Array.Empty<string>();

    /// <summary>Fixed number of parameters</summary>
    bool IDictionary.IsFixedSize => true;
    /// <summary></summary>
    public bool IsReadOnly => @readonly;
    /// <summary>Parameter names</summary>
    ICollection<string> IDictionary<string, object?>.Keys => _ParameterNames;
    /// <summary>Parameter names</summary>
    ICollection IDictionary.Keys => _ParameterNames;
    /// <summary>Argument values</summary>
    ICollection IDictionary.Values => _Arguments;
    /// <summary>Argument values</summary>
    public ICollection<object?> Values => _Arguments;
    /// <summary>Parameter count</summary>
    public int Count => _ParameterNames.Length;
    /// <summary>Is internally synchronized as arguments collection.</summary>
    bool ICollection.IsSynchronized => true;
    /// <summary></summary>
    object ICollection.SyncRoot => mLock;

    /// <summary>Assign <paramref name="argument"/> to <paramref name="parameterName"/>.</summary>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="KeyNotFoundException"></exception>
    protected virtual void set(string parameterName, object? argument)
    {
        // Assert not null
        if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));
        // Assert not readonly
        this.AssertWritable();
        // Search parameter
        int parameterIx = _ParameterNames.IndexOf(parameterName);
        // No parameter
        if (parameterIx < 0) throw new KeyNotFoundException(parameterName);
        // Synchronized event
        lock (mLock)
        {
            // Get arguments
            object?[] _arguments = _Arguments;
            // Assign over
            if (parameterIx < _arguments.Length)
            {
                // Clone
                _arguments = (object?[])_Arguments.Clone();
            }
            // Need to create larger array
            else
            {
                // Create larger array
                object?[] newArguments = new object?[parameterIx + 1];
                // Copy elements
                _arguments.CopyTo(newArguments, 0);
                // Assign array
                _arguments = newArguments;
            }
            // Assign value
            _arguments[parameterIx] = argument;
            // Assign updated copy
            this.Arguments = _arguments;
        }
    }

    /// <summary>Get argument value of <paramref name="parameterName"/>.</summary>
    /// <exception cref="ArgumentNullException"></exception>
    public object? Get(object parameterName)
    {
        // Assert not null
        if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));
        // Search parameter
        int parameterIx = _ParameterNames.IndexOf(parameterName);
        // Not found
        if (parameterIx < 0) return null;
        // Get arguments
        var _arguments = _Arguments;
        // Return value
        return parameterIx < _arguments.Length ? _arguments[parameterIx] : null;
    }

    /// <summary>Get argument value of <paramref name="parameterName"/>.</summary>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="KeyNotFoundException"></exception>
    public object? Get2(string parameterName)
    {
        // Assert not null
        if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));
        // Search parameter
        int parameterIx = _ParameterNames.IndexOf(parameterName);
        // Not found
        if (parameterIx < 0) throw new KeyNotFoundException(parameterName?.ToString());
        // Get arguments
        var _arguments = _Arguments;
        // Return value
        return parameterIx < _arguments.Length ? _arguments[parameterIx] : null;
    }

    /// <summary>The argument that is correlated to <paramref name="parameterName"/></summary>
    public object? this[string parameterName] { get => Get2((string)parameterName); set => set((string)parameterName, value); }
    /// <summary>The argument that is correlated to <paramref name="parameterName"/></summary>
    object? IDictionary.this[object parameterName] { get => Get(parameterName); set => set((string)parameterName, value); }

    /// <summary>Assigns <paramref name="argument"/> to <paramref name="parameterName"/>. Recreates <see cref="Arguments"/> array.</summary>
    /// <exception cref="InvalidOperationException">If could not write argument assignment.</exception>
    void IDictionary.Add(object parameterName, object? argument) => set((string)parameterName, argument);
    /// <summary>Assigns <paramref name="argument"/> to <paramref name="parameterName"/>. Recreates <see cref="Arguments"/> array.</summary>
    /// <exception cref="InvalidOperationException">If could not write argument assignment.</exception>
    void IDictionary<string, object?>.Add(string parameterName, object? argument) => set(parameterName, argument);
    /// <summary>Assigns argument to parameter. Recreates <see cref="Arguments"/> array.</summary>
    void ICollection<KeyValuePair<string, object?>>.Add(KeyValuePair<string, object?> item) => set(item.Key, item.Value);
    /// <summary>Clear arguments</summary>
    void IDictionary.Clear() => this.AssertWritable().Arguments = Array.Empty<object>();
    /// <summary>Clear arguments</summary>
    void ICollection<KeyValuePair<string, object?>>.Clear() => this.AssertWritable().Arguments = Array.Empty<object>();

    /// <summary>Test whether <paramref name="parameterName"/> exists</summary>
    bool IDictionary<string, object?>.ContainsKey(string parameterName) => _ParameterNames.Contains(parameterName);
    /// <summary>Tests if key-value exists</summary>
    bool ICollection<KeyValuePair<string, object?>>.Contains(KeyValuePair<string, object?> item)
    {
        // Key not found
        if (!TryGetValue(item.Key, out object? value)) return false;
        // Both null
        if (value == null && item.Value == null) return true;
        // One is null
        if (value == null || item.Value == null) return false;
        // Equality
        return value.Equals(item.Value);
    }
    /// <summary>Test if argument value exists for <paramref name="parameterName"/>.</summary>
    bool IDictionary.Contains(object parameterName)
    {
        // Assert not null
        if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));
        // Search parameter
        int parameterIx = _ParameterNames.IndexOf(parameterName);
        // No parameter
        if (parameterIx < 0) return false;
        // Get arguments
        var _arguments = _Arguments;
        // Index out of range
        if (parameterIx >= _arguments.Length) return false;
        // Get value
        object? value = _arguments[parameterIx];
        // No value
        if (value == null) return false;
        // Got value
        return true;
    }

    /// <summary>Remove argument specified by <paramref name="parameterName"/>. Creates new argument array.</summary>
    public bool Remove(string parameterName)
    {
        // Assert not null
        if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));
        // Assert not readonly
        this.AssertWritable();
        // Search parameter
        int parameterIx = _ParameterNames.IndexOf(parameterName);
        // No parameter
        if (parameterIx < 0) return false;
        // Get arguments
        var _arguments = _Arguments;
        // Out of range
        if (parameterIx >= _arguments.Length) return false;
        // Synchronized action
        lock (mLock)
        {
            // Assert not readonly
            this.AssertWritable();
            // Clone
            _arguments = (object?[])_Arguments.Clone();
            // Assign null
            _arguments[parameterIx] = null;
            // Assign updated copy
            this.Arguments = _arguments;
            // Done
            return true;
        }
    }

    /// <summary>Remove argument specified by <paramref name="parameterName"/>. Creates new argument array.</summary>
    void IDictionary.Remove(object parameterName) => this.Remove((string)parameterName);

    /// <summary>Remove parameter argument pair</summary>
    bool ICollection<KeyValuePair<string, object?>>.Remove(KeyValuePair<string, object?> item)
    {
        // Assert not null
        if (item.Key == null) throw new ArgumentNullException(nameof(item), "Key is null");
        // Assert not readonly
        this.AssertWritable();
        // Search parameter
        int parameterIx = _ParameterNames.IndexOf(item.Key);
        // No parameter
        if (parameterIx < 0) return false;
        // Get arguments
        var _arguments = _Arguments;
        // Out of range
        if (parameterIx >= _arguments.Length) return false;
        // Synchronized action
        lock (mLock)
        {
            // Assert not readonly
            this.AssertWritable();
            // Clone
            _arguments = (object?[])_Arguments.Clone();
            // Out of range
            if (parameterIx >= _arguments.Length) return false;
            // Assign null
            _arguments[parameterIx] = null;
            // Assign updated copy
            this.Arguments = _arguments;
            // Done
            return true;
        }
    }
    /// <summary>Copy arguments to <paramref name="dstArguments"/> at <paramref name="index"/>.</summary>
    void ICollection.CopyTo(Array dstArguments, int index) => _Arguments.CopyTo(dstArguments, index);
    /// <summary>Copy parameter, argument pairs to <paramref name="dstPairs"/> at <paramref name="index"/>.</summary>
    void ICollection<KeyValuePair<string, object?>>.CopyTo(KeyValuePair<string, object?>[] dstPairs, int index)
    {
        // Get snapshot
        var _parameterNames = _ParameterNames;
        var _arguments = _Arguments;
        // Iterate
        for (int i = 0; i < _parameterNames.Length; i++)
        {
            // Get value
            object? argument = i < _arguments.Length ? _arguments[i] : null;
            // Assign
            dstPairs[index++] = new KeyValuePair<string, object?>(_parameterNames[i], argument);
        }
    }

    /// <summary>Enumerate parameter,argument keyvaluepairs</summary>
    IEnumerator IEnumerable.GetEnumerator()
    {
        // Get snapshot
        var _parameterNames = _ParameterNames;
        var _arguments = _Arguments;
        KeyValuePair<string, object?>[] lines = new KeyValuePair<string, object?>[_parameterNames.Length];
        // Iterate
        for (int i = 0; i < _parameterNames.Length; i++)
        {
            // Get value
            object? value = i >= _arguments.Length ? null : _arguments[i];
            // Assign
            lines[i] = new KeyValuePair<string, object?>(_parameterNames[i], value);
        }
        // Return
        return lines.GetEnumerator();
    }
    /// <summary>Get enumerator</summary>
    public Enumerator GetEnumerator() => new Enumerator(_ParameterNames, _Arguments);
    /// <summary>Get enumerator</summary>
    IDictionaryEnumerator IDictionary.GetEnumerator() => new Enumerator(_ParameterNames, _Arguments);
    /// <summary>Get enumerator</summary>
    IEnumerator<KeyValuePair<string, object?>> IEnumerable<KeyValuePair<string, object?>>.GetEnumerator() => new Enumerator(_ParameterNames, _Arguments);

    /// <summary>Try get the <paramref name="argument"/> that is assigned to <paramref name="parameterName"/>.</summary>
    /// <exception cref="ArgumentNullException"></exception>
    public bool TryGetValue(string parameterName, out object? argument)
    {
        // Assert not null
        if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));
        // Search parameter
        int parameterIx = _ParameterNames.IndexOf(parameterName);
        // No parameter
        if (parameterIx < 0) { argument = null; return false; }
        // Get arguments
        var _arguments = _Arguments;
        // Index out of range
        if (parameterIx >= _arguments.Length) { argument = null; return false; }
        // Get value
        argument = _arguments[parameterIx];
        // Got value
        return true;
    }

    /// <summary>Enumerator</summary>
    public class Enumerator : IDictionaryEnumerator, IEnumerator, IEnumerator<KeyValuePair<string, object?>>
    {
        /// <summary>Parameter names</summary>
        protected string[] parameterNames;
        /// <summary>Arguments </summary>
        protected object?[] arguments;
        /// <summary>Parameter index</summary>
        protected int cursor = -1;

        /// <summary>Message enumerator</summary>
        public Enumerator(string[]? parameterNames, object?[]? arguments)
        {
            this.parameterNames = parameterNames ?? Array.Empty<string>();
            this.arguments = arguments ?? Array.Empty<object>();
        }

        /// <summary>Key-value pair at cursor</summary>
        public DictionaryEntry Entry => new DictionaryEntry(Key, Value);
        /// <summary>Parameter name at cursor</summary>
        public object Key => cursor >= 0 && cursor < parameterNames.Length ? parameterNames[cursor] : null!;
        /// <summary>Argument value at cursor</summary>
        public object? Value => cursor >= 0 && cursor < arguments.Length ? arguments[cursor] : null!;
        /// <summary>Key-value pair at cursor</summary>
        public object Current => new KeyValuePair<string, object?>((string)Key, Value);
        /// <summary>Key-value pair at cursor</summary>
        KeyValuePair<string, object?> IEnumerator<KeyValuePair<string, object?>>.Current => new KeyValuePair<string, object?>((string)Key, Value);

        /// <summary>Move to next parameter.</summary>
        public bool MoveNext()
        {
            cursor++;
            return cursor >= 0 && cursor < parameterNames.Length;
        }
        /// <summary>Reset cursor</summary>
        public void Reset() => cursor = -1;

        /// <summary></summary>
        protected virtual void Dispose(bool disposing) { }
        /// <summary></summary>
        public void Dispose()
        {             
            // Take off from ~finalizer queue.
            GC.SuppressFinalize(this);
        }
    }

}

