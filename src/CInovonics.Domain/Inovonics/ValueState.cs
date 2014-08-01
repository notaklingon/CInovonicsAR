using System.Xml.Serialization;

namespace CInovonics.Domain.Inovonics
{
[   XmlTypeAttribute(AnonymousType=true)]
    public class ValueState
    {
        public string System { get; set; }
        public string Device { get; set; }
        public string Comms { get; set; }
        public string Power { get; set; }
        public string Arm { get; set; }
        public string ArmReady { get; set; }
        public string IntrusionAlarm { get; set; }
        public string IntrusionFault { get; set; }
        public string IntrusionTrouble { get; set; }
        public string Bypass { get; set; }
        public string Latch { get; set; }
        public string Door { get; set; }
        public string AccessOverride { get; set; }
        public string Mask { get; set; }
        public string Tamper { get; set; }
        public string Granted { get; set; }
        public string Denied { get; set; }
        public string Connection { get; set; }
        public string InputFault { get; set; }
        public string Active { get; set; }
    }
}
