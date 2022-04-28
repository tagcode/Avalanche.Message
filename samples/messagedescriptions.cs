using System.Runtime.CompilerServices;
using Avalanche.Message;
using Avalanche.Utilities;
using static System.Console;


class messagedescriptions
{
    /// <summary>ABC</summary>
    public class Test<T>
    {
        /// <summary>DEF</summary>
        public Test<T> Do<K>() { return default!; }
    }


    public static void Run()
    {
        {
            // Print key
            WriteLine(LibraryMessagesTable.Instance.GoodResult); // "Library.GoodResult"

            // Create message
            IMessage message = LibraryMessagesTable.Instance.GoodResult.New("MyObject");
            // Print message
            WriteLine(message); // "'MyObject' Ok result"
        }
        {
            foreach (IMessageDescription code in LibraryMessagesTable.Instance.List) WriteLine(code);
            // MessageDescription(Code = A0A10001, Key = Library.BadUnexpected, HResult = A0A10001, Severity =, Template = "'{object}': Unexpected error", "Captured unexpected <see cref="T: System.Exception" />.")
            // MessageDescription(Code = A0A10002, Key = Library.BadArgumentNull, HResult = A0A10002, Severity =, Template = "'{object}': Bad argument 'null'", "Captured unexpected <see cref="T: System.Exception" />.")
            // MessageDescription(Code = 20A10003, Key = Library.GoodResult, HResult = 20A10003, Severity =, Template = "'{object}': Ok result", "Good result.")
        }
        {
            IMessageDescription statusCode = LibraryMessagesTable.Instance.Codes[0x20A10003];
            WriteLine(statusCode);
        }
        {
            IMessageDescription statusCode = LibraryMessages.Instance.Codes[0x20A10003];
            WriteLine(statusCode);
        }
        {
            IMessageDescription statusCode = LibraryMessagesTable.Instance.Keys["Library.GoodResult"];
            WriteLine(statusCode);
        }
        {
            IMessageDescription statusCodes = LibraryMessagesTable.Instance.HResults[unchecked((int)0xA0A10002)].First();
            WriteLine(statusCodes);
        }
        {
            IMessageDescriptions store = new MessageDescriptions()
                .Add(LibraryMessagesTable.Instance.GoodResult)
                .SetReadOnly();

            WriteLine(store.Keys["Library.GoodResult"]); // "'MyObject' Ok result"
        }
    }

    /// <summary></summary>
    public class LibraryMessages : MessageDescriptions
    {
        /// <summary>Base id for datatype codes.</summary>
        public const int BaseCode = 0x20A10000;
        /// <summary>Singleton</summary>
        static Lazy<LibraryMessages> instance = new Lazy<LibraryMessages>(() => new LibraryMessages().Initialize().SetAllReadOnly<LibraryMessages>().SetReadOnly());
        /// <summary>Singleton</summary>
        public static LibraryMessages Instance => instance.Value;

        /// <summary>Message description fields</summary>
        IMessageDescription badUnexpected = null!, badArgumentNull = null!, goodResult = null!;

        /// <summary>Initialize fields</summary>
        public override LibraryMessages Initialize()
        {
            // Create descriptions
            set(0xA0A10001, ref badUnexpected, "'{object}': Unexpected error");
            set(0xA0A10002, ref badArgumentNull, "'{object}': Unexpected error");
            set(0x20A10003, ref goodResult, "'{object}': Unexpected error");
            // Return 
            return this;
        }

        /// <summary>Create description</summary>
        void set(long id, ref IMessageDescription messageDescription, string templateText, [CallerArgumentExpression("messageDescription")] string key = null!)
            => Add(messageDescription = new MessageDescription($"Library.{char.ToUpper(key[0])}{key.Substring(1)}", id, templateText).SetHResult(id));

        /// <summary>Captured unexpected <see cref="System.Exception"/>.</summary>
        public IMessageDescription BadUnexpected { get => badUnexpected; set => this.AssertWritable().badUnexpected = value; }
        /// <summary>Captured unexpected <see cref="System.Exception"/>.</summary>
        public IMessageDescription BadArgumentNull { get => badArgumentNull; set => this.AssertWritable().badArgumentNull = value; }
        /// <summary>Good result.</summary>
        public IMessageDescription GoodResult { get => goodResult; set => this.AssertWritable().goodResult = value; }
    }

    /// <summary></summary>
    public class LibraryMessagesTable : MessageDescriptionsTable
    {
        /// <summary>Base id for datatype codes.</summary>
        public const int BaseCode = 0x20A10000;
        /// <summary>Singleton</summary>
        static Lazy<LibraryMessagesTable> instance = new Lazy<LibraryMessagesTable>(() => new LibraryMessagesTable().ReadSummaryXmls().SetAllReadOnly<LibraryMessagesTable>().SetReadOnly());
        /// <summary>Singleton</summary>
        public static LibraryMessagesTable Instance => instance.Value;

        /// <summary>Message description fields</summary>
        IMessageDescription badUnexpected = Create(0xA0A10001, nameof(badUnexpected), "'{object}': Unexpected error");
        IMessageDescription badArgumentNull = Create(0xA0A10002, nameof(badArgumentNull), "'{object}': Bad argument 'null'");
        IMessageDescription goodResult = Create(0x20A10003, nameof(goodResult), "'{object}': Ok result");

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

}
