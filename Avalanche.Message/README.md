# Introduction
Avalanche.Message unifies various coding facets,
[[git]](https://github.com/tagcode/Avalanche.Message), 
[[www]](https://avalanche.fi/Avalanche.Core/Avalanche.Message/docs/), 
[[licensing]](xref:order).

Add package reference to .csproj.
```xml
<PropertyGroup>
    <RestoreAdditionalProjectSources>https://avalanche.fi/Avalanche.Core/nupkg/index.json</RestoreAdditionalProjectSources>
</PropertyGroup>
<ItemGroup>
    <PackageReference Include="Avalanche.Message"/>
</ItemGroup>
```

[Message](xref:message) is an argumentable record.

```csharp
IMessage message = new Message()
    .SetMessageDescription(SystemMessages.ArgumentNull.Generic)
    .SetArguments("argumentName")
    .SetNow()
    .SetId(123)
    .SetUserData("User-data", new object())
    .SetReadOnly();
```

Messages are [validation](xref:validation) statuscodes.

```csharp
// Observable
IObservable<IMessage> observable = new MyObservable();
// Observer
MyObserver observer = new MyObserver();
// Observe 
observable.Subscribe(observer);
// Print status codes
foreach (IMessage statuscode in observer.StatusCodes) WriteLine(statuscode);
```

Messages are events.

```csharp
// Create event description
IMessageDescription fileCreatedEventDescription = new MessageDescription("FileCreated", 0x20012301, "File '{filename}' was created.");
// Create event
IMessage @event = fileCreatedEventDescription
    .New("filename.txt")
    .SetTime(DateTime.Now)
    .SetId(IdGenerators.Guid.Next)
    .SetUserData("EventSource", "Server-45")
    .SetReadOnly();
```

Messages are [throwable](xref:messageexception).

```csharp
throw SystemMessages.ArgumentNull.Generic.New("argumentName").SetTime(DateTime.Now).NewException();
```

Messages are <a href="https://docs.microsoft.com/en-us/dotnet/api/system.string.format?view=net-6.0">string.Format()</a> printable.

```csharp
// Create message
IMessage message = SystemMessages.ArgumentNull.Generic.New("argumentName");
// Get format text
string formatText = message.MessageDescription.Template.Breakdown.FormatTemplate();
// Formulate
string formulation = string.Format(formatText, message.Arguments);
// Print
WriteLine(formulation);
```

Messages are [loggable](xref:messagelogging).

```csharp
SystemMessages.Argument.EnumValueNotFound.New("Value").LogTo(logger);
```

<img src="message/logger.png" alt="Log line" style="width: 300px;"/>

Messages are [localizable](xref:message.localization) (with [Avalanche.Localization](xref:avalanche.localization)).

```csharp
// Create localization
ILocalization localization = new Localization()
    .AddLine("fi", HResult.S_OK.Key, "Detect", "Operaatio onnistui");
// Decorate to localize
IMessageDescription s_ok = HResult.S_OK.Localized(localization);
// "Operation successful"
WriteLine(s_ok.New().Print(CultureInfo.GetCultureInfo("en")));
// "Operaatio onnistui"
WriteLine(s_ok.New().Print(CultureInfo.GetCultureInfo("fi")));
```

Message texts are [pluralizable](xref:localization.pluralization) (with [Avalanche.Localization](xref:avalanche.localization)).

```csharp
// Create message description.
MessageDescription filesDeleted = new MessageDescription("System.IO.FilesDeleted", 0x25110001, "{count} file(s) were deleted.");
// Create localization
ILocalization localization = new Localization()
    .AddLine("", filesDeleted.Key, "Detect", "No files were deleted.", "Unicode.CLDR", "count:cardinal:zero:en")
    .AddLine("", filesDeleted.Key, "Detect", "One file was deleted.", "Unicode.CLDR", "count:cardinal:one:en")
    .AddLine("", filesDeleted.Key, "Detect", "{count} files were deleted.", "Unicode.CLDR", "count:cardinal:other:en");
// Localize message description
filesDeleted.Localize(localization).SetReadOnly();
// "No files were deleted."
WriteLine(filesDeleted.New(0).Print());
// "One file was deleted."
WriteLine(filesDeleted.New(1).Print());
// "2 files were deleted."
WriteLine(filesDeleted.New(2).Print());
```

Messages can have [HResult](xref:hresult) code.

```csharp
try
{
    throw SystemMessages.InvalidOperation.Exception.NewException();
}
catch (Exception e) when (e.HResult == HResultIds.COR_E_INVALIDOPERATION)
{
}
```

Messages are processable in-process and out-of-process. 

```csharp
IMessage message = HResult.S_OK.New().SetId(IdGenerators.Integer.Next);
// Serialize as csv line
string[] cells = { message.MessageDescription.Code.ToString()!, message.Id!.ToString()!, Escaper.Comma.EscapeJoin(message.Arguments.Select(a=>a?.ToString()??"")) };
string csvLine = Escaper.Semicolon.EscapeJoin(cells);
```


The following <em>global_includes.cs</em> can be used to include extension methods.

```cs
global using Avalanche.Message;
global using Avalanche.Utilities;
global using Avalanche.Template;
```

<details>
<summary>Class libraries:</summary>
<ul>
<li>Avalanche.Message.dll contains implementations.</li>
<li>Avalanche.Message.Abstractions.dll contains interfaces.</li>
<li>Avalanche.Message.Logging.dll contains Microsoft.Extensions.Logging extensions.</li>
</ul>
<p>Dependency libraries, direct and indirect:</p>
<ul>
<li>Avalanche.Template.dll</li>
<li>Avalanche.Template.Abstractions.dll</li>
<li>Avalanche.Tokenizer.dll</li>
<li>Avalanche.Tokenizer.Abstractions.dll</li>
<li>Avalanche.Utilities.dll</li>
<li>Avalanche.Utilities.Abstractions.dll</li>
</ul>
<p>Tangential libraries:</p>
<ul>
<li>Avalanche.StatusCode.dll contains various public status codes (<a class="xref" href="../../Avalanche.StatusCode/docs/index.html">see more</a>).</li>
<li>Avalanche.Localization.dll</li>
<li>Avalanche.Localization.Abstractions.dll</li>
<li>Avalanche.Message.Localization.dll</li>
</ul>
</details>



