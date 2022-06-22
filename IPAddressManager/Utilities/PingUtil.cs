using IPAddressManager.Models;
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
            Parallel.For(1, noOfIpAddresses,  async i => { await PingIPAddress($"{subnet}.{i}"); });
            stopWatch.Stop();
            string timeTaken = stopWatch.Elapsed.Duration().ToString();
            Console.WriteLine($"Asynchorous Ping took : {timeTaken} to finish pinging {noOfIpAddresses - 1} IP Addresses.");
        }

        public async static Task PingIPAddress(string ip)
        {
            await Task.Run(() =>
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
                        if (reply.Status == IPStatus.Success)
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
                catch (SocketException)
                {
                    Console.WriteLine("=========================");
                    Console.WriteLine($"IP Address: {ip}");
                    Console.WriteLine($"Status: VACANT");
                    Console.WriteLine($"Proccessed on Thread #{currentCore}");
                }
            });
        }

        public static async Task<NetworkDetail> GetNetworkDetail(string ip)
        {
            NetworkDetail networkDetail = new NetworkDetail();
            networkDetail.IPAddress = ip;

            Ping ping = new();
            PingReply pingReply = await ping.SendPingAsync(ip, 10000);

            try
            {
                IPHostEntry host = await Dns.GetHostEntryAsync(IPAddress.Parse(ip));
                if(host != null)
                {
                    bool hasReply = pingReply.Status == IPStatus.Success;
                    networkDetail.Status = hasReply ? Status.Up : Status.Down;
                    if (hasReply)
                    {
                        networkDetail.MacAddress = MacUtil.GetMacAddress(ip);
                        networkDetail.Manufacturer = await MacUtil.GetManufacturerAsync(networkDetail.MacAddress);
                    }
                }
            }
            catch (SocketException)
            {
                networkDetail.Status = Status.Vacant;
            }

            Console.WriteLine("=========================================");
            Console.WriteLine($"IP Address: {networkDetail.IPAddress}");
            Console.WriteLine($"Computer Name: {networkDetail.Name}");
            Console.WriteLine($"Mac Address: {networkDetail.MacAddress}");
            Console.WriteLine($"Status: {networkDetail.Status}");
            return networkDetail;
        }


        /// <summary>
        /// Pings a range of IP Addresses in Parallel.
        /// </summary>
        public static void ParallelPingAsync(string subnet, int noOfIpAddresses = 255)
        {
            Console.WriteLine("==================== Asynchronous Ping ====================");

            Stopwatch stopWatch = new();
            stopWatch.Start();
            List<Task> tasks = new();

            Parallel.For(1, noOfIpAddresses, i => { tasks.Add(GetNetworkDetail($"{subnet}.{i}")); });

            Task.WaitAll(tasks.ToArray());

            stopWatch.Stop();
            string timeTaken = stopWatch.Elapsed.Duration().ToString();
            Console.WriteLine($"Asynchorous Ping took : {timeTaken} to finish pinging {noOfIpAddresses - 1} IP Addresses.");
        }
    }
}
