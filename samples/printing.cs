using System.Globalization;
using System.Text;
using Avalanche.Message;
using Avalanche.StatusCode;
using Avalanche.Template;
using static System.Console;

class printing
{
    public static void Run()
    {
        {
            // Create message
            IMessage message = SystemMessages.IO.FileNotFoundFileName.New("MyFile.txt");
            // Print as string
            string printOut = message.Print(CultureInfo.InvariantCulture);
            // Write print
            WriteLine(printOut); // "Could not find file 'MyFile.txt'."
        }
        {
            // Create message
            IMessage message = SystemMessages.IO.FileNotFoundFileName.New("MyFile.txt");
            // Estimate length
            int length = message.EstimatePrintLength(CultureInfo.InvariantCulture);
            // Allocate span
            Span<char> span = length < 512 ? stackalloc char[length] : new char[length];
            // Print to span
            length = message.PrintTo(span, CultureInfo.InvariantCulture);
            // Print to stdout, zero heap.
            Console.Out.Write(span.Slice(0, length));
            WriteLine();
        }

        {
            // Create message
            IMessage message = SystemMessages.IO.FileNotFoundFileName.New("MyFile.txt");
            // Estimate length
            int length = message.EstimatePrintLength(CultureInfo.InvariantCulture);
            // Allocate span
            Span<char> span = length < 512 ? stackalloc char[length] : new char[length];
            // Print to span
            if (message.TryPrintTo(span, out length, CultureInfo.InvariantCulture))
                Console.Out.Write(span.Slice(0, length));
            WriteLine();
        }

        {
            // Create message
            IMessage message = SystemMessages.IO.FileNotFoundFileName.New("MyFile.txt");
            // Create string builder
            StringBuilder sb = new(1024);
            // Append to string builder
            message.AppendTo(sb, CultureInfo.InvariantCulture);
            // Print
            WriteLine(sb); // "Could not find file 'MyFile.txt'."
        }
        {
            // Create message
            IMessage message = SystemMessages.IO.FileNotFoundFileName.New("MyFile.txt");
            // Assign text writer
            TextWriter textWriter = Console.Out;
            // Write to writer
            message.WriteTo(textWriter, CultureInfo.InvariantCulture); // "Could not find file 'MyFile.txt'."
            WriteLine();
        }

        {
            // Create template
            ITemplateText text = TemplateFormat.BraceNumeric.Text["Time={0}."];
            // Create message description
            IMessageDescription messageDescription = new MessageDescription().SetTemplate(text);
            // Create message
            IMessage message = messageDescription.New(DateTime.Now);
            // Get cultures
            CultureInfo fi = CultureInfo.GetCultureInfo("fi"), en = CultureInfo.GetCultureInfo("en");
            // Use as IFormattable or ISpanFormattable
            WriteLine(String.Format(fi, "Message: {0}", message)); // "Message: Time=10.1.2022 18.53.06."
            WriteLine(String.Format(en, "Message: {0}", message)); // "Message: Time=1/10/2022 6:53:06 PM."
        }


    }
}
