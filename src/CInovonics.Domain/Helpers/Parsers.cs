using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace CInovonics.Domain.Helpers
{
    public static class Parsers
    {
        public static T ParseXML<T>(this XmlReader reader) where T: class
        {
            return new XmlSerializer(typeof(T)).Deserialize(reader) as T;
        }


        public static T ParseXML<T>(this string xml) where T : class
        {
            return new XmlSerializer(typeof(T)).Deserialize(new StringReader(xml)) as T;
        }
    }
}
