using System;
using System.Xml.Serialization;

namespace CInovonics.Domain.Inovonics
{
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class DeviceInfo
    {
        public string deviceName { get; set; }
        public Guid deviceID { get; set; }
        public object deviceDescription { get; set; }
        public string deviceLocation { get; set; }
        public string systemContact { get; set; }
        public object model { get; set; }
        public uint serialNumber { get; set; }
        public string macAddress { get; set; }
        public string firmwareVersion { get; set; }
        public string bootVersion { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public DateTime manufactureDate { get; set; }
    }
}