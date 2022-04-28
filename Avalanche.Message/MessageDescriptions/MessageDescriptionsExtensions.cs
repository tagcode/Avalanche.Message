// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Message;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary></summary>
public static class MessageDescriptionsExtensions_
{
    /// <summary>Read summary xmls and assign as description</summary>
    public static T ReadSummaryXmls<T>(this T store) where T : MessageDescriptions
    {
#if DEBUG
        // Create comment reader
        IProvider<Assembly, XmlDocument> docProvider = AssemblyDocumentProvider.Instance.Cached();
        IProvider<FieldInfo, string> fieldXmlComments = new AssemblyDocumentProvider.Field(docProvider);
        IProvider<PropertyInfo, string> propertyXmlComments = new AssemblyDocumentProvider.Property(docProvider);
#endif
        // Get fields
        IEnumerable<FieldInfo> fields = store.GetType().GetFields().Where(fi => fi.FieldType.IsAssignableTo(typeof(IMessageDescription)) && fi.IsPublic && !fi.IsStatic);
        // Sort by code
        fields = fields.OrderBy(fi => ((IMessageDescription)fi.GetValue(store)!).Code & 0x0FFFFFFF);
        // Iterate each
        foreach (FieldInfo fi in fields)
        {
            // Get status code
            IMessageDescription sc = (IMessageDescription)fi.GetValue(store)!;
#if DEBUG
            // Read comments and place in description
            try
            {
                // Place as description
                if (fieldXmlComments != null && fieldXmlComments.TryGetValue(fi, out string summary))  sc.Description = summary;
            }
            catch (Exception) { }
#endif
        }

        // Get properties
        IEnumerable<PropertyInfo> properties = store.GetType().GetProperties().Where(pi => pi.PropertyType.IsAssignableTo(typeof(IMessageDescription)) && pi.GetMethod != null && pi.GetMethod.IsPublic && !pi.GetMethod.IsStatic);
        // Sort by code
        properties = properties.OrderBy(fi => ((IMessageDescription)fi.GetValue(store)!).Code & 0x0FFFFFFF);
        // Iterate each
        foreach (PropertyInfo pi in properties)
        {
            // Get status code
            IMessageDescription sc = (IMessageDescription)pi.GetValue(store)!;
#if DEBUG
            // Read comments and place in description
            try
            {
                // Place as description
                if (propertyXmlComments != null && propertyXmlComments.TryGetValue(pi, out string summary)) sc.Description = summary;
            }
            catch (Exception) { }
#endif
        }
        //
        return store;
    }

    /// <summary>Assign all message descriptions as read-only</summary>
    public static T SetAllReadOnly<T>(this T store) where T : IEnumerable<IMessageDescription>
    {
        foreach (IMessageDescription md in store)
            if (md is IReadOnly @readonly)
                @readonly.ReadOnly = true;
        return store;
    }
}
