// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Message;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalanche.Utilities;

/// <summary>Table of status messages.</summary>
public class MessageDescriptionsTable : MessageDescriptionTable<MessageDescription>
{
    /// <summary>Read from <paramref name="source"/> all the fields and properties that implement <see cref="IMessageDescription"/> and add them to <paramref name="dest"/>.</summary>
    public static void ReadFields(IMessageDescriptions source, IMessageDescriptions dest)
    {
        // Get fields
        IEnumerable<FieldInfo> fields = source.GetType().GetFields().Where(fi => fi.FieldType.IsAssignableTo(typeof(IMessageDescription)) && fi.IsPublic && !fi.IsStatic);
        // Sort by code
        fields = fields.OrderBy(fi => ((IMessageDescription)fi.GetValue(source)!).Code & 0x0FFFFFFF);
        // Iterate each
        foreach (FieldInfo fi in fields)
        {
            // Get status code
            IMessageDescription sc = (IMessageDescription)fi.GetValue(source)!;
            // Assign field name
            if (sc is IUserDataContainer userDataContainer) userDataContainer.UserData["FieldName"] = fi.Name;
            // Make message description read-only
            //if (sc is IReadOnly _readonly) _readonly.ReadOnly = true;
            // Add to record
            dest.Add(sc);
        }

        // Get properties
        IEnumerable<PropertyInfo> properties = source.GetType().GetProperties().Where(pi => pi.PropertyType.IsAssignableTo(typeof(IMessageDescription)) && pi.GetMethod != null && pi.GetMethod.IsPublic && !pi.GetMethod.IsStatic);
        // Sort by code
        properties = properties.OrderBy(fi => ((IMessageDescription)fi.GetValue(source)!).Code & 0x0FFFFFFF);
        // Iterate each
        foreach (PropertyInfo pi in properties)
        {
            // Get status code
            IMessageDescription sc = (IMessageDescription)pi.GetValue(source)!;
            // Assign field name
            if (sc is IUserDataContainer userDataContainer) userDataContainer.UserData["FieldName"] = pi.Name;
            // Make message description read-only
            //if (sc is IReadOnly _readonly) _readonly.ReadOnly = true;
            // Add to record
            dest.Add(sc);
        }
    }
}

/// <summary>Table of statuscodes of <typeparamref name="T"/>.</summary>
public class MessageDescriptionTable<T> : MessageDescriptions<T> where T : IMessageDescription, new()
{
    /// <summary>Create event info record that reads local fields</summary>
    public MessageDescriptionTable() : base()
    {
        Initialize();
        MessageDescriptionsTable.ReadFields(this, this);
    }


}


