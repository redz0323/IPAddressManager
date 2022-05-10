using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IPAddressManager.Utilities
{
    internal static class MacUtil
    {
        public static string GetManufacturer(string macAddress)
        {
            var uri = new Uri("https://api.macvendors.com/" + WebUtility.UrlEncode(macAddress));
            using (var wc = new HttpClient())
            {
                return Task.Run(async() => await wc.GetStringAsync(uri)).Result;
            }
        }


        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        static extern int SendARP(int destIp, int srcIP, byte[] macAddr, ref uint physicalAddrLen);

        public static string GetMacAddress(string ipAddress)
        {
            IPAddress dst = IPAddress.Parse(ipAddress); // the destination IP address

            byte[] macAddr = new byte[6];
            uint macAddrLen = (uint)macAddr.Length;

            if (SendARP(BitConverter.ToInt32(dst.GetAddressBytes(), 0), 0, macAddr, ref macAddrLen) != 0)
                throw new InvalidOperationException("SendARP failed.");

            string[] str = new string[(int)macAddrLen];
            for (int i = 0; i < macAddrLen; i++)
                str[i] = macAddr[i].ToString("x2");

            return string.Join(":", str);
        }
    }
}
