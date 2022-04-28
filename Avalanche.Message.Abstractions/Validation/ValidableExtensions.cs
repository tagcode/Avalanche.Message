// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Message;

/// <summary>Extension methods for <see cref="IValidable"/>.</summary>
public static class ValidableExtensions
{
    /// <summary>Test whether <paramref name="instance"/> is valid.</summary>
    public static bool IsGood(this IValidable instance)
    {
        //
        foreach (IMessage status in instance.Validate())
        {
            if (!status.MessageDescription.IsGood()) return false;
        }
        //
        return true;
    }

    /// <summary>Test whether <paramref name="instance"/> is valid without throwing an exception.</summary>
    public static bool IsNotGood(this IValidable instance)
    {
        //
        foreach (IMessage status in instance.Validate())
        {
            if (status.MessageDescription.IsNotGood()) return true;
        }
        //
        return false;
    }

    /// <summary>Test whether <paramref name="instance"/> is valid without throwing an exception.</summary>
    public static bool IsBad(this IValidable instance)
    {
        //
        foreach (IMessage status in instance.Validate())
        {
            if (status.MessageDescription.IsBad()) return true;
        }
        //
        return false;
    }

    /// <summary>Test whether <paramref name="instance"/> is valid without throwing an exception.</summary>
    public static bool IsNotBad(this IValidable instance)
    {
        //
        foreach (IMessage status in instance.Validate())
        {
            if (status.MessageDescription.IsBad()) return false;
        }
        //
        return true;
    }

}
