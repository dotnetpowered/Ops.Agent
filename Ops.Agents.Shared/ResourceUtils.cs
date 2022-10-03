using System;
using System.Reflection;

namespace Ops.Agents.Shared;

public static class ResourceUtils
{
    // To get a list of resource names:
    // var names = this.GetType().GetTypeInfo().Assembly.GetManifestResourceNames();

    public static string LoadEmbeddedResource<T>(string filename) where T: class
    {
        var type = typeof(T);
        filename = $"{type.Namespace}.{filename}";
        var stream = type.GetTypeInfo().Assembly.GetManifestResourceStream(filename);
        if (stream == null)
            throw new InvalidOperationException($"Unable to load {filename} resource.");
        var streamreader = new StreamReader(stream);
        var query = streamreader.ReadToEnd();
        return query;
    }
}

