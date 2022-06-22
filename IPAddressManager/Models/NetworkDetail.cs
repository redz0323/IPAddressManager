using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPAddressManager.Models
{
    internal class NetworkDetail
    {
        public string? Name { get; set; }
        public string IPAddress { get; set; }
        public string? MacAddress { get; set; }
        public string? Manufacturer { get; set; }
        public Status Status { get; set; }
    }
}
