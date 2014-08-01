using System.Xml.Serialization;

namespace CInovonics.Domain.Inovonics
{
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = "AreaControlEvent")]
    public class AreaControlEvent
    {
        [XmlElement("MetadataHeader", Type = typeof(MetadataHeader))]
        public MetadataHeader Header { get; set; }

        [XmlElement("EventData", Type = typeof(EventData))]
        public EventData Event { get; set; }
    }
}
