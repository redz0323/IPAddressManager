using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace IPAddressManager.Utilities
{
    internal static class PingUtil
    {
        /// <summary>
        /// Pings a range of IP Addresses in a sequentially.
        /// </summary>
        public static void SequentialPing(string subnet, int noOfIpAddresses = 255)
        {
            Console.WriteLine("==================== Synchronous Ping ====================");

            Stopwatch stopWatch = new();
            stopWatch.Start();
            for (int i = 1; i < 255; i++)
                PingIPAddress($"{subnet}.{i}");

            stopWatch.Stop();
            string timeTaken = stopWatch.Elapsed.Duration().ToString();
            Console.WriteLine($"Synchorous Ping took : {timeTaken} to finish pinging {noOfIpAddresses - 1} IP Addresses.");
        }

        /// <summary>
        /// Pings a range of IP Addresses in Parallel.
        /// </summary>
        public static void ParallelPing(string subnet, int noOfIpAddresses = 255)
        {
            Console.WriteLine("==================== Asynchronous Ping ====================");

            Stopwatch stopWatch = new();
            stopWatch.Start();
            Parallel.For(1, noOfIpAddresses, i => { PingIPAddress($"{subnet}.{i}"); });
            stopWatch.Stop();
            string timeTaken = stopWatch.Elapsed.Duration().ToString();
            Console.WriteLine($"Asynchorous Ping took : {timeTaken} to finish pinging {noOfIpAddresses - 1} IP Addresses.");
        }

        public static void PingIPAddress(string ip)
        {
            Ping ping = new();
            PingReply reply = ping.Send(ip, 100);

            int currentCore = Thread.GetCurrentProcessorId();
            try
            {
                IPHostEntry host = Dns.GetHostEntry(IPAddress.Parse(ip));
                if (host != null)
                {
                    string status = reply.Status == IPStatus.Success ? "UP" : "DOWN";
                    Console.WriteLine("=========================");
                    Console.WriteLine($"IP Address: {ip}");
                    Console.WriteLine($"Status: {status}");
                    if(reply.Status == IPStatus.Success)
                    {
                        Console.WriteLine($"Host Name: {host.HostName}");
                        string macAddress = MacUtil.GetMacAddress(ip);
                        Console.WriteLine($"Mac Address: {macAddress}");
                        string manufacturer = MacUtil.GetManufacturer(macAddress);
                        Console.WriteLine($"Manufacturer: {manufacturer}");
                    }
                    Console.WriteLine($"Proccessed on Thread #{currentCore}");
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("=========================");
                Console.WriteLine($"IP Address: {ip}");
                Console.WriteLine($"Status: VACANT");
                Console.WriteLine($"Proccessed on Thread #{currentCore}");
            }
        }
    }
}
