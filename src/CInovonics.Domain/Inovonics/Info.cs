using System.Xml.Serialization;

namespace CInovonics.Domain.Inovonics
{
    [XmlTypeAttribute(AnonymousType = true)]
    public class Info
    {
        public int ID { get; set; }
        public string Type {get;set;}
        public string Description { get; set; }
        public object[] PartitionList { get; set; }
        public string SCI { get; set; }
        public int SCICode { get; set; }
    }
}
