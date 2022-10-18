namespace IPAddressManager.Models
{
    internal class NetworkDetail
    {
        public string? Name { get; set; }
        public string IPAddress { get; set; }
        public string? MacAddress { get; set; }
        public string? Manufacturer { get; set; }
        public Status Status { get; set; }

        internal NetworkDetail(string ipAddress)
        {
            this.IPAddress = ipAddress;
        }
    }
}
