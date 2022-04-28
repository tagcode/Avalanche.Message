using System;
using System.Collections.Generic;
using Avalanche.Utilities;
using Avalanche.Message;
using static System.Console;
using Avalanche.StatusCode;

class validation
{
    public static void Run()
    {
        {
            MyClass myClass = new MyClass(null, null);
            IMessage message = myClass.ValidateSingle();
            WriteLine(message); // Parameter 'Name' cannot be null.
        }
        {
            MyClass myClass = new MyClass(null, null);
            IEnumerable<IMessage> messages = myClass.Validate();
            WriteLine(string.Join(", ", messages)); // Parameter 'Name' cannot be null., Parameter 'Label' cannot be null.
        }
        {
            MyClass myClass = new MyClass("ABC", "Hello");
            IMessage message = myClass.ValidateSingle();
            WriteLine(message); // "Operation successful"
        }
        {
            MyClass myClass = new MyClass("ABC", "Hello").AssertGood();
        }
        try
        {
            MyClass myClass = new MyClass(null, null).AssertNotBad();
        }
        catch (Exception e) when (e.HResult == SystemMessages.ArgumentNull.WithParamName.HResult)
        {
            // "Avalanche.Message.ValidationException:  (Parameter 'Name' cannot be null.) (Parameter 'Label' cannot be null.)"
            // "   at validable.Run() in validable.cs:line 41"
            WriteLine(e);
        }
    }

    public class MyClass : IValidable
    {
        public string? Name;
        public string? Label;

        public MyClass(string? name, string? label)
        {
            Name = name;
            Label = label;
        }

        /// <summary>Validate class</summary>
        /// <returns>Return validation errors.</returns>
        public IEnumerable<IMessage> Validate()
        {
            // Place codes here
            StructList2<IMessage> codes = new();
            // Validate 'Name' not null
            if (Name == null) codes.Add( SystemMessages.ArgumentNull.WithParamName.NameOf(Name) );
            // Validate 'Label' not null
            if (Label == null) codes.Add( SystemMessages.ArgumentNull.WithParamName.NameOf(Label) );
            // Good
            if (codes.Count == 0) codes.Add(HResult.S_OK.New());
            // Return
            return codes.ToArray(); // If empty returns singleton T[0].
        }
    }
}
