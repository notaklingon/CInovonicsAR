using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CInovonics.Domain.Events
{
    public class PanicEvent
    {
        public string Device { get; set; }
        public string SCI { get; set; }
        public int SCICode { get; set; }

        public string Location { get; set; }

        public override string ToString()
        {
            return String.Format("{0} {1} {2} {3}", Device, SCICode, SCI, Location).Trim();
        }
    }
}
