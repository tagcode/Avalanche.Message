using Avalanche.Message;
using Avalanche.StatusCode;
using Avalanche.Template;
using Avalanche.Utilities;
using static System.Console;

class message
{
    public static void Run()
    {
        {
            object obj = new object();
            IMessage message = new Message(
                messageDescription: CoreMessages.Instance.GoodValid,
                arguments: obj
            );
            IMessage goodStatus = CoreMessages.Instance.GoodValid.New(obj);
            IMessage errorStatus = CoreMessages.Instance.BadNotValid.New(obj, "Unexpected");
            IMessage errorStatusNullArg = CoreMessages.Instance.BadNullArgument.New(obj, "Name");
        }
        {
            ValueMessage message = new ValueMessage(
                messageDescription: CoreMessages.Instance.GoodValid,
                arguments: new object()
            );
        }
        {
            object argument = null!;
            IMessage msg = SystemMessages.ArgumentNull.WithParamName.NameOf(argument);
            WriteLine(msg); // Parameter 'argument' cannot be null.
        }
        {
            object obj = new object();
            IMessage msg = CoreMessages.Instance.BadNotValid.New(obj, "Unknown").SetId(IdGenerators.Guid.Next);
            WriteLine(msg.Id); // "679b2f1a-3341-4cc6-81d6-5c55bbdde164"
        }

        {
            object obj = new object();
            IMessage msg = CoreMessages.Instance.BadNotValid.New(obj, "Unknown").SetTime(DateTime.Now);
            WriteLine(msg.Time); // 
        }

        {
            object obj = new object();
            IMessage msg = CoreMessages.Instance.BadNotValid.New(obj, "Unknown").SetNow();
            WriteLine(msg.Time); // 
        }


        {
            // Create message
            IMessage msg = CoreMessages.Instance.BadReadOnly.New();
            // Set data
            msg.UserData["Hello"] = "World";
            // Read data
            WriteLine(msg.UserData["Hello"]); // "World"
        }

        {
            // Create message
            IMessage msg = CoreMessages.Instance
                .BadReadOnly
                .New()
                .SetUserData("Hello", "World");
            // Read data
            WriteLine(msg.UserData["Hello"]); // "World"
        }

        {
            // Create message without argument
            IMessage msg = CoreMessages.Instance.BadUnexpected.New();
            // Cast to dictionary
            IDictionary<string, object?> args = msg;
            // Assign argument to parameter
            args["object"] = "MyObject";
            // "'MyObject': Unexpected error"
            WriteLine(msg);
        }

        {
            // Create inner message
            IMessage msg1 = CoreMessages.Instance.BadReadOnly.New();
            // Create outer message
            IMessage msg2 = CoreMessages.Instance.BadUnexpected.New().SetInnerMessage(msg1);
        }

        {
            // Create arguments
            string user = "User";
            DateTime time = DateTime.Now;
            long errorCode = 0x80000000;
            // Create message
            IMessage message = new Message().SetInterpolated($"Hello {user}, time is {time}, error is [0x{errorCode:X8}].");
            // Print text
            WriteLine(message); // "Hello User, time is 10.1.2022 23.22.55, error is [0x80000000]."
            // Print template text
            WriteLine(message.MessageDescription.Template); // "Hello {0}, time is {1}, error is [0x{2:X8}]."
            // Print argument
            WriteLine(message.Arguments[0]); // "User"

            // Reconstruct template
            string assembly = TemplateFormat.Percent.Assemble[message.MessageDescription.Template.Breakdown];
            // Print assembly
            WriteLine(assembly); // "Hello %1, time is %2, error is [0x%3]."
        }
        {
            // Create template
            ITemplateText text = TemplateFormat.Percent.Text["Hello %1, time is %2."];
            // Create message description
            IMessageDescription messageDescription = new MessageDescription().SetTemplate(text);
            // Create message
            Message message = messageDescription.New("User", DateTime.Now);
            // Cast to FormattableString
            FormattableString formattableString = message;
            // Use like string interpolation
            WriteLine(FormattableString.Invariant(formattableString)); // "Hello User, time is 01/10/2022 21:56:49."
        }
    }
}
