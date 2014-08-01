using System.Xml.Serialization;

namespace CInovonics.Domain.Inovonics
{
    [XmlTypeAttribute(AnonymousType = true)]
    public class EventData
    {
        [XmlElement("Info", Type = typeof(Info))]
        public Info EventInfo { get; set; }

        [XmlElement("ValueState", Type = typeof(ValueState))]
        public ValueState EventValueState { get; set; }
    }
}
