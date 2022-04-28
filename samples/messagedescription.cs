using Avalanche.Message;
using Avalanche.Template;
using Avalanche.Utilities;
using static System.Console;

class messagedescription
{
    public static void Run()
    {
        {
            IMessageDescription messageDescription = new MessageDescription(
                key: "MyLibrary.Uncertain",
                code: 0x0AC40000 | StatusCodes.Uncertain,
                messageTemplate: "'{object}': Uncertain status"
            ).SetReadOnly();
        }
        {
            IMessageDescription messageDescription = new MessageDescription
            {
                Key = "MyLibrary.Uncertain",
                Code = 0x0AC40000 | StatusCodes.Uncertain,
                Template = TemplateFormat.BraceAlphaNumeric.Breakdown["'{object}': Uncertain status"],
                Description = "Status code is uncertain."
            }.SetReadOnly();
        }
        {
            IMessageDescription messageDescription = new MessageDescription("Uncertain", 0x4AC40000, "'{object}': Uncertain status");
            WriteLine(messageDescription.Template.Breakdown.FormatTemplate());             // "'{0}': Uncertain status"
            WriteLine(messageDescription.Template.Breakdown.FormatTemplate(), "MyObject"); // "'MyObject': Uncertain status"
        }
        {
            IMessageDescription messageDescription = new MessageDescription("Uncertain", 0x4AC40000, "'{object}': Uncertain status");
            WriteLine(messageDescription.Template.Breakdown.LoggerTemplate());             // "'{object}': Uncertain status"
        }
        {
            IMessageDescription messageDescription = new MessageDescription("Uncertain", 0x4AC40000, "'{object}': Uncertain status");
            WriteLine(string.Join(", ", messageDescription.Template.ParameterNames)); // "object"
        }
        {
            List<IMessageDescription> list = new List<IMessageDescription>
            {
                CoreMessages.Instance.BadNotValid,
                CoreMessages.Instance.Uncertain,
                CoreMessages.Instance.GoodValid
            };

            list.Sort(MessageDescriptionComparer.Instance);
        }
        {
            IMessageDescription good = new MessageDescription("MyLibrary.Good", 0x0AC40000, "'{object}': Good").SetReadOnly();
            IMessageDescription uncertain = new MessageDescription("MyLibrary.Uncertain", 0x4AC40000, "'{object}': Uncertain").SetReadOnly();
            IMessageDescription bad = new MessageDescription("MyLibrary.Bad", 0x8AC40000, "'{object}': Bad").SetReadOnly();

            WriteLine(good.IsGood());       // True
            WriteLine(good.IsBad());        // False
            WriteLine(good.IsUncertain());  // False
            WriteLine(good.IsNotBad());     // True
            WriteLine(good.IsNotGood());    // False
        }

        {
            IMessageDescription bad = new MessageDescription("MyLibrary.Bad", 0x8AC40000, "'{object}': Bad")
                .SetException(typeof(InvalidOperationException));
            WriteLine(bad.GetExceptionTypeName()); // "System.InvalidOperationException"
            InvalidOperationException e = (InvalidOperationException)bad.NewException("MyObj");
        }
        {
            IMessageDescription bad = new MessageDescription("MyLibrary.Bad", 0x8AC40000, "'{object}': Bad")
                .SetException("System.InvalidOperationException");
            // Print exception type
            WriteLine(bad.GetExceptionTypeName()); // "System.InvalidOperationException"
            // Create exception
            InvalidOperationException e = (InvalidOperationException)bad.NewException("MyObj");
        }
        {
            IMessageDescription bad = new MessageDescription("MyLibrary.Bad", 0x8AC40000, "'{object}': Bad")
                .SetException(InvalidOperationException (IMessage m) => new InvalidOperationException(m.ToString()));
            // Print exception type
            WriteLine(bad.GetExceptionTypeName()); // "System.InvalidOperationException"

            // Create exception
            InvalidOperationException e = (InvalidOperationException)bad.NewException("MyObj");
        }
        {
            IMessageDescription bad = new MessageDescription("MyLibrary.Bad", 0x8AC40000, "'{object}': Bad")
                .SetException(InvalidOperationException (IMessage m, Exception? innerException) => new InvalidOperationException(m.ToString(), innerException));
            // Print exception type
            WriteLine(bad.GetExceptionTypeName()); // "System.InvalidOperationException"

            // Create exception
            Exception e = bad.New("MyObj").SetError(new Exception("inner exception")).NewException();
            // Print inner exception
            WriteLine(e.InnerException);
        }


    }

}
