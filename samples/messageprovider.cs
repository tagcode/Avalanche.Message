using Avalanche.Utilities;
using Avalanche.Message;
using Avalanche.StatusCode;

class messageprovider
{
    public static void Run()
    {
        // Create message
        IMessage msg = SystemMessages.InvalidCast.FromTo.New("int", "string");
        // Create object and attach message
        MyClass obj = new MyClass().SetMessage(msg).SetReadOnly();
    }

    public class MyClass : ReadOnlyAssignableClass, IMessageProvider
    {
        /// <summary>Associated message.</summary>
        protected IMessage? message;
        /// <summary>Associated message.</summary>
        public IMessage? Message { get => message; set => this.AssertWritable().message = value; }
    }
}
