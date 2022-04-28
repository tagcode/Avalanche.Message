using Avalanche.Message;
using Avalanche.Utilities;
using Microsoft.Extensions.Logging;

class messagelevel
{
    public static void Run()
    {
        {
#pragma warning disable CS0219
            // MessageLevel -> LogLevel
            Microsoft.Extensions.Logging.LogLevel logLevel = (LogLevel)(int)MessageLevel.Critical;
            // LogLevel -> MessageLevel
            MessageLevel messageLevel = (MessageLevel)(int)Microsoft.Extensions.Logging.LogLevel.Critical;
#pragma warning restore CS0219
        }
        {
            IMessageDescription bad =
                new MessageDescription("MyLibrary.Bad", 0x0AC40000 | StatusCodes.Bad, "'{object}': Bad")
                .SetSeverity(MessageLevel.Error)
                .SetReadOnly();
        }
        {
            IMessageDescription bad =
                new MessageDescription("MyLibrary.Bad", 0x0AC40000 | StatusCodes.Bad, "'{object}': Bad")
                .SetSeverity(MessageLevel.Error);

            // Change message level
            IMessage message = bad.New().SetSeverity(MessageLevel.Critical);
        }
        {
            IMessageDescription bad =
                new MessageDescription("MyLibrary.Bad", 0x0AC40000 | StatusCodes.Bad, "'{object}': Bad")
                .SetSeverity(LogLevel.Error);
        }
        {
            IMessageDescription bad =
                new MessageDescription("MyLibrary.Bad", 0x0AC40000 | StatusCodes.Bad, "'{object}': Bad")
                .SetSeverity(LogLevel.Error)
                .SetReadOnly();

            // Change message level
            IMessage message = bad.New().SetSeverity(LogLevel.Error);
        }

    }

}
