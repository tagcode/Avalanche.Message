using System;
using Avalanche.Utilities;
using static System.Console;
using Avalanche.Message;
using Avalanche.StatusCode;

class messageexception
{
    public static void Run()
    {
        {
            try
            {
                throw SystemMessages.ArgumentNull.Generic.New("argumentName").NewException();
            }
            catch (Exception)
            {
            }
        }
        {
            try
            {
                throw SystemMessages.ArgumentNull.Generic.New("argumentName").NewException<InvalidOperationException>();
            }
            catch (Exception)
            {
            }
        }
        {
            Exception e =
                SystemMessages.ArgumentNull.Generic
                .New("argumentName")
                .SetTime(DateTime.Now)
                .SetId(IdGenerators.Guid)
                .NewException();
        }
        {
            try
            {
                SystemMessages.ArgumentNull.Generic.New("argumentName").Throw();
            }
            catch (Exception)
            {
            }
        }
        {
            try
            {
                SystemMessages.ArgumentNull.Generic.New("argumentName").Throw<InvalidOperationException>();
            }
            catch (Exception)
            {
            }
        }
        {
            try
            {
                throw SystemMessages.ArgumentNull.Generic.NewException("argumentName");
            }
            catch (Exception)
            {
            }
        }
        {
            try
            {
                throw SystemMessages.ArgumentNull.Generic.NewException<InvalidOperationException>("argumentName");
            }
            catch (Exception)
            {
            }
        }
        {
            try
            {
                SystemMessages.ArgumentNull.Generic.Throw("argumentName");
            }
            catch (Exception)
            {
            }
        }
        {
            try
            {
                SystemMessages.ArgumentNull.Generic.Throw<InvalidOperationException>("argumentName");
            }
            catch (Exception)
            {
            }
        }

        {
            try
            {
                throw SystemMessages.ArgumentNull.Generic.New("argumentName").NewException();
            }
            catch (Exception e) when (e.StatusMessage()?.MessageDescription.Code == SystemMessages.ArgumentNull.Generic.Code)
            {
            }
        }

        {
            try
            {
                throw SystemMessages.ArgumentNull.Generic.New("argumentName").NewException();
            }
            catch (Exception e) when (e.HResult == HResultIds.COR_E_ARGUMENT)
            {
            }
        }

        {
            try
            {
                try
                {
                    throw SystemMessages.ArgumentNull.Generic
                            .New("argumentName")
                            .SetTime(DateTime.Now)
                            .SetId(IdGenerators.Guid.Next)
                            .SetUserData("Hello", "World")
                            .NewException();
                }
                catch (Exception e) when (e.TryGetMessage(out IMessage message))
                {
                    // Print message
                    WriteLine(message); // "Value cannot be null."
                    // Print time
                    WriteLine(message.Time); // ""
                    // Print event id
                    WriteLine(message.Id); // "9016d669-ef99-44cd-8184-be4f8ab594d4"
                    // Print data
                    WriteLine(message.UserData["Hello"]); // "World"
                    // rethrow
                    throw;
                }
            }
            catch (Exception) { }
        }

        {
            try
            {
                try
                {
                    throw SystemMessages.ArgumentNull.Generic
                            .New("argumentName")
                            .SetTime(DateTime.Now)
                            .SetId(IdGenerators.Guid.Next)
                            .SetUserData("Hello", "World")
                            .NewException();
                }
                catch (Exception e) 
                {
                    // Get possible message
                    IMessage message = e.StatusMessage()!;
                    // Print time
                    WriteLine(message.Time); // "2.1.2022 12.31.06"
                    // Print event id
                    WriteLine(message.Id); // "9016d669-ef99-44cd-8184-be4f8ab594d4"
                    // Print data
                    WriteLine(message.UserData["Hello"]); // "World"
                    // rethrow
                    throw;
                }
            }
            catch (Exception) { }
        }

    }
}
