using System;
using System.Xml.Serialization;

namespace CInovonics.Domain.Inovonics
{
    [Serializable]
    public class Partition
    {
        [XmlElement("ID")]
        public int ID { get; set; }
        [XmlElement("Description")]
        public string Description { get; set; }
    }
}
