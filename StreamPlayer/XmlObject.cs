using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace StreamPlayer
{
    public abstract class XmlObject<T>
        where T: XmlObject<T>
    {
        private static readonly XmlWriterSettings WriterSettings = new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 };
        private static readonly XmlSerializerNamespaces XmlNamespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });

        public void SerializeAsXml(Stream toStream)
        {
            using (var writer = XmlWriter.Create(toStream, WriterSettings))
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(writer, this, XmlNamespaces);
            }
        }

        public void SaveToXmlFile(string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                SerializeAsXml(stream);
            }
        }

        public static T DeserializeFromXml(Stream fromStream)
        {
            using (var reader = XmlReader.Create(fromStream))
            {
                var serializer = new XmlSerializer(typeof(T));
                if (!serializer.CanDeserialize(reader))
                    return default(T);

                try
                {
                    return serializer.Deserialize(reader) as T;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return default(T);
                }
            }
        }

        public static T LoadFromXmlFile(string filePath)
        {
            using (var stream = File.OpenRead(filePath))
            {
                return DeserializeFromXml(stream);
            }
        }
    }
}
