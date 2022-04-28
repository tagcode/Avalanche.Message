<b>Avalanche.Message.Localization</b> contains localization extensions for messaging,
[[git]](https://github.com/tagcode/Avalanche.Message/Avalanche.Message.Localization/), 
[[www]](https://avalanche.fi/Avalanche.Core/Avalanche.Message/docs/localization/index.html), 
[[licensing]](https://avalanche.fi/Avalanche.Core/license/index.html).


This library requires [Avalanche.Localization](https://avalanche.fi/Avalanche.Core/Avalanche.Localization/docs/index.html) and [Avalanche.Message](https://avalanche.fi/Avalanche.Core/Avalanche.Message/docs/index.html).

<b>.Localize(localization, cultureProvider?)</b> localizes the template text of the 'messageDescription'. 'messageDescription' cannot be in read-only state.

```csharp
// Create localization
ILocalization localization = new Localization().AddLine("fi", "NULL.S_OK", "Detect", "Operaatio onnistui");
// Create message description.
IMessageDescription ok = new MessageDescription("NULL.S_OK", 0x00000000, "Operation successful").Localize(localization);
// "Operation successful"
WriteLine(ok.New().Print(CultureInfo.InvariantCulture));
// "Operaatio onnistui"
WriteLine(ok.New().Print(CultureInfo.GetCultureInfo("fi")));
```

'cultureProvider' can be used to provide default culture externally.

```csharp
// Create localization
ILocalization localization = new Localization().AddLine("fi", "NULL.S_OK", "Detect", "Operaatio onnistui");
// Create culture provider
ICultureProvider cultureProvider = new CultureProvider();
// Create message description.
IMessageDescription ok = new MessageDescription("NULL.S_OK", 0x00000000, "Operation successful").Localize(localization, cultureProvider);
// Set language
cultureProvider.Culture = "en";
// "Operation successful"
WriteLine(ok.New().Print());
// Set language
cultureProvider.Culture = "fi";
// "Operaatio onnistui"
WriteLine(ok.New().Print());
```

<b>.Localized(localization, cultureProvider?)</b> creates decorated version of 'messageDescription' where text is localized. Can be used with 'messageDescription' that is in read-only state.

```csharp
// Create localization
ILocalization localization = new Localization().AddLine("fi", HResult.S_OK.Key, "Detect", "Operaatio onnistui");
// Decorate to localize
IMessageDescription ok = HResult.S_OK.Localized(localization);
// "Operation successful"
WriteLine(ok.New().Print(CultureInfo.InvariantCulture));
// "Operaatio onnistui"
WriteLine(ok.New().Print(CultureInfo.GetCultureInfo("fi")));
```

<b>Localization.Default</b> is the default localization [singleton](https://avalanche.fi/Avalanche.Core/Avalanche.Localization/docs/localization/index.html) that is preconfigured to search localization files under "Resources/" folder. 

```csharp
// Decorate to localize
IMessageDescription ok = HResult.S_OK.Localized(Localization.Default);
```

<b>.LocalizeMessages(localization)</b> can be used to localize entire message description table.

```csharp
/// <summary></summary>
public class StatusCodes : MessageDescriptions
{
    /// <summary>Base id for datatype codes.</summary>
    public const int BaseCode = 0x20A10000;
    /// <summary>Singleton</summary>
    static Lazy<StatusCodes> instance = new Lazy<StatusCodes>(() => new StatusCodes().ReadSummaryXmls().LocalizeMessages(Localization.Default).SetAllReadOnly<StatusCodes>().SetReadOnly());
    /// <summary>Singleton</summary>
    public static StatusCodes Instance => instance.Value;

    /// <summary>Message description fields</summary>
    IMessageDescription badUnexpected = Create(0xA0A10001, nameof(badUnexpected), "'{object}': Unexpected error");
    IMessageDescription badArgumentNull = Create(0xA0A10002, nameof(badArgumentNull), "'{object}': Unexpected error");
    IMessageDescription goodResult = Create(0x20A10003, nameof(goodResult), "'{object}': Unexpected error");

    /// <summary>Create description</summary>
    static IMessageDescription Create(long id, string key, string templateText)
        => new MessageDescription($"Library.{char.ToUpper(key[0])}{key.Substring(1)}", id, templateText).SetHResult(id);

    /// <summary>Captured unexpected <see cref="System.Exception"/>.</summary>
    public IMessageDescription BadUnexpected { get => badUnexpected; set => this.AssertWritable().badUnexpected = value; }
    /// <summary>Captured unexpected <see cref="System.Exception"/>.</summary>
    public IMessageDescription BadArgumentNull { get => badArgumentNull; set => this.AssertWritable().badArgumentNull = value; }
    /// <summary>Good result.</summary>
    public IMessageDescription GoodResult { get => goodResult; set => this.AssertWritable().goodResult = value; }
}
```

A new table may need to be apply localization.

```csharp
// Create localized message description table
BasicMessages basicMessages = new BasicMessages()
    .Initialize()
    .LocalizeMessages(Localization.Default)
    .SetAllReadOnly<BasicMessages>()
    .SetReadOnly();
// Get message description
IMessageDescription fail = basicMessages.E_FAIL;
// "Unspecified error"
WriteLine(fail.New().Print(CultureInfo.InvariantCulture));
// "Määrittelemätön virhe"
WriteLine(fail.New().Print(CultureInfo.GetCultureInfo("fi")));
```

Resources/NULL.yaml:

```yaml
TemplateFormat: Brace
PluralRules: Unicode.CLDR

Invariant:
- Culture: ""
  Items:
  - Key: NULL.S_OK
    Text: "Operation successful"
  - Key: NULL.E_FAIL
    Text: "Unspecified error"

Finnish:
- Culture: fi
  Items:
  - Key: NULL.S_OK
    Text: "Operatio onnistui"
  - Key: NULL.E_FAIL
    Text: "Määrittelemätön virhe"
```


