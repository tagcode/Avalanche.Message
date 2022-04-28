using System.Globalization;
using Avalanche.Localization;
using Avalanche.Message;
using Avalanche.StatusCode;
using Avalanche.Template;
using Avalanche.Utilities;
using static System.Console;

class example
{
    public static void Run()
    {
        {
            IMessage message = new Message()
                .SetMessageDescription(SystemMessages.ArgumentNull.Generic)
                .SetArguments("argumentName")
                .SetNow()
                .SetId(123)
                .SetUserData("User-data", new object())
                .SetReadOnly();
        }

        {
            try
            {
                throw SystemMessages.ArgumentNull.Generic.New("argumentName").SetTime(DateTime.Now).NewException();
            }
            catch (Exception) { }
        }
        {
            // Observable
            IObservable<IMessage> observable = new MyObservable();
            // Observer
            MyObserver observer = new MyObserver();
            // Observe 
            observable.Subscribe(observer);
            // Print status codes
            foreach (IMessage statuscode in observer.StatusCodes) WriteLine(statuscode);
        }
        {
            // Create message
            IMessage message = SystemMessages.ArgumentNull.Generic.New("argumentName");
            // Get format text
            string formatText = message.MessageDescription.Template.Breakdown.FormatTemplate();
            // Formulate
            string formulation = string.Format(formatText, message.Arguments);
            // Print
            WriteLine(formulation);
        }


        {
            // Create localization
            ILocalization localization = new Localization()
                .AddLine("fi", HResult.S_OK.Key, "Detect", "Operaatio onnistui");
            // Decorate to localize
            IMessageDescription s_ok = HResult.S_OK.Localized(localization);
            // "Operation successful"
            WriteLine(s_ok.New().Print(CultureInfo.GetCultureInfo("en")));
            // "Operaatio onnistui"
            WriteLine(s_ok.New().Print(CultureInfo.GetCultureInfo("fi")));
        }
        {
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
        }
        {
            // Create event description
            IMessageDescription fileCreatedEventDescription = new MessageDescription("FileCreated", 0x20012301, "File '{filename}' was created.");
            // Create event
            IMessage @event = fileCreatedEventDescription
                .New("filename.txt")
                .SetTime(DateTime.Now)
                .SetId(IdGenerators.Guid.Next)
                .SetUserData("EventSource", "Server-45")
                .SetReadOnly();
        }
        {
            try
            {
                throw SystemMessages.InvalidOperation.Exception.NewException();
            }
            catch (Exception e) when (e.HResult == HResultIds.COR_E_INVALIDOPERATION)
            {
            }
        }
        {
            IMessage message = HResult.S_OK.New().SetId(IdGenerators.Integer.Next);
            // Serialize as csv line
            string[] cells = { message.MessageDescription.Code.ToString()!, message.Id!.ToString()!, Escaper.Comma.EscapeJoin(message.Arguments.Select(a => a?.ToString() ?? "")) };
            string csvLine = Escaper.Semicolon.EscapeJoin(cells);
            WriteLine(csvLine); // "0;0;"
        }
    }

    /// <summary></summary>
    public class MyObservable : IObservable<IMessage>
    {
        /// <summary></summary>
        public IDisposable Subscribe(IObserver<IMessage> observer)
        {
            observer.OnNext(new Message());
            observer.OnCompleted();
            return null!;
        }
    }

    /// <summary></summary>
    public class MyObserver : IObserver<IMessage>
    {
        /// <summary></summary>
        public readonly List<IMessage> StatusCodes = new();
        /// <summary></summary>
        public void OnCompleted() { }
        /// <summary></summary>
        public void OnError(Exception error) { }
        /// <summary></summary>
        public void OnNext(IMessage statusCode) { StatusCodes.Add(statusCode); }
    }

}
