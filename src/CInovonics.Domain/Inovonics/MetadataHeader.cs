using System;
using System.Xml.Serialization;

namespace CInovonics.Domain.Inovonics
{
    [XmlTypeAttribute(AnonymousType = true)]
    public class MetadataHeader
    {
        public decimal MetaVersion { get; set; }
        public string MetaID { get; set; }
        public Guid MetaSourceID { get; set; }
        public int MetaSourceLocalID { get; set; }
        public DateTime MetaTime { get; set; }
        public int MetaPriority { get; set; }
    }
}
