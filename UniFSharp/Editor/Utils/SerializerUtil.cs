using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using UniFSharp;

namespace UniFSharp
{

    public static class SerializerUtil
    {
        public static T Load<T>(T target)
        {
            var serializer = new XmlSerializer(typeof(T));
            var fileName = typeof(T).Name;
            var filePath = String.Format(@"{0}{1}.xml", FSharpBuildTools.settingsPath, fileName);
            if (File.Exists(filePath) == false)
            {
                return default(T);
            }
            else
            {
                using (var sr = new StreamReader(filePath, new UTF8Encoding(false)))
                {
                    return (T)serializer.Deserialize(sr);
                }
            }
        }

        public static void Save<T>(T target)
        {
            var fileName = typeof(T).Name;
            var filePath = String.Format(@"{0}{1}.xml", FSharpBuildTools.settingsPath, fileName);
            var serializer = new XmlSerializer(typeof(T));
            using (var sw = new StreamWriter(filePath, false, new UTF8Encoding(false)))
            {
                serializer.Serialize(sw, target);
            }
        }
    }
}