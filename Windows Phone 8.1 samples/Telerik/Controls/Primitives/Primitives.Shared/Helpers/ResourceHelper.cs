using System;
using System.IO;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    internal static class ResourceHelper
    {
        public static object LoadEmbeddedResource(Type type, string resourcePath, object key)
        {
            Assembly assembly = type.GetTypeInfo().Assembly;
            using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
            {
                StreamReader reader = new StreamReader(stream);
                ResourceDictionary dictionary = XamlReader.Load(reader.ReadToEnd()) as ResourceDictionary;
                return dictionary[key];
            }
        }

        public static byte[] LoadManifestStreamBytes(Type type, string resourcePath)
        {
            Assembly assembly = type.GetTypeInfo().Assembly;
            using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
            {
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);

                return bytes;
            }
        }
    }
}
