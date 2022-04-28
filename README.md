<b>Avalanche.Message</b> unifies various coding facets,
[[git]](https://github.com/tagcode/Avalanche.Message), 
[[www]](https://avalanche.fi/Avalanche.Core/Avalanche.Message/docs/), 
[[licensing]](https://avalanche.fi/Avalanche.Core/license/index.html).

Add package reference to .csproj.
```xml
<PropertyGroup>
    <RestoreAdditionalProjectSources>https://avalanche.fi/Avalanche.Core/nupkg/index.json</RestoreAdditionalProjectSources>
</PropertyGroup>
<ItemGroup>
    <PackageReference Include="Avalanche.Message"/>
</ItemGroup>
```

[Message](https://avalanche.fi/Avalanche.Core/Avalanche.Message/docs/message/index.html) is an argumentable record.

```csharp
IMessage message = new Message()
    .SetMessageDescription(SystemMessages.ArgumentNull.Generic)
    .SetArguments("argumentName")
    .SetNow()
    .SetId(123)
    .SetUserData("User-data", new object())
    .SetReadOnly();
```

Messages are [validation](https://avalanche.fi/Avalanche.Core/Avalanche.Message/docs/validation/index.html) statuscodes.

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

Messages are [throwable](https://avalanche.fi/Avalanche.Core/Avalanche.Message/docs/message/messageexception.html).

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

Messages are [loggable](https://avalanche.fi/Avalanche.Core/Avalanche.Message/docs/logging/index.html).

```csharp
SystemMessages.Argument.EnumValueNotFound.New("Value").LogTo(logger);
```

```none
[A345005A E] Requested value 'Value' was not found.
```

Messages are [localizable](https://avalanche.fi/Avalanche.Core/Avalanche.Message/docs/localization/index.html) (with [Avalanche.Localization](https://avalanche.fi/Avalanche.Core/Avalanche.Localization/docs/index.html)).

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

Message texts are [pluralizable](https://avalanche.fi/Avalanche.Core/Avalanche.Localization/docs/pluralization/index.html) (with [Avalanche.Localization](https://avalanche.fi/Avalanche.Core/Avalanche.Localization/docs/index.html)).

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

Messages can have [HResult](https://avalanche.fi/Avalanche.Core/Avalanche.StatusCode/docs/hresult/index.html) code.

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
