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
            for (int i = 1; i < noOfIpAddresses; i++)
                Ping($"{subnet}.{i}");

            stopWatch.Stop();
            string timeTaken = stopWatch.Elapsed.Duration().ToString();
            Console.WriteLine($"Synchorous Ping took : {timeTaken} to finish pinging {noOfIpAddresses - 1} IP Addresses.");
        }

        public static NetworkDetail Ping(string ip)
        {
            int currentCore = Thread.GetCurrentProcessorId();
            NetworkDetail networkDetail = new(ip);
            Ping ping = new();
            PingReply pingReply = ping.Send(ip, 10000);
            try
            {
                IPHostEntry host = Dns.GetHostEntry(IPAddress.Parse(ip));
                if (host != null)
                {
                    bool hasReply = pingReply.Status == IPStatus.Success;
                    networkDetail.Status = hasReply ? Status.Up : Status.Down;
                }
            }
            catch (SocketException)
            {
                networkDetail.Status = Status.Vacant;
            }
            Console.WriteLine($"IP Address: {networkDetail.IPAddress}; Status: {networkDetail.Status}; Processed on Thread #{currentCore}.");
            return networkDetail;
        }

        public static async Task<NetworkDetail> PingAsync(string ip)
        {
            int currentCore = Thread.GetCurrentProcessorId();
            NetworkDetail networkDetail = new(ip);

            Ping ping = new();
            PingReply pingReply = await ping.SendPingAsync(ip, 10000);

            try
            {
                IPHostEntry host = await Dns.GetHostEntryAsync(IPAddress.Parse(ip));
                if(host != null)
                {
                    bool hasReply = pingReply.Status == IPStatus.Success;
                    networkDetail.Status = hasReply ? Status.Up : Status.Down;
                }
            }
            catch (SocketException)
            {
                networkDetail.Status = Status.Vacant;
            }

            Console.WriteLine($"IP Address: {networkDetail.IPAddress}; Status: {networkDetail.Status}; Processed on Thread #{currentCore}.");
            return networkDetail;
        }


        /// <summary>
        /// Pings a range of IP Addresses in Parallel.
        /// </summary>
        public static void ParallelPing(string subnet, int noOfIpAddresses = 255)
        {
            Console.WriteLine("==================== Asynchronous Ping ====================");

            Stopwatch stopWatch = new();
            stopWatch.Start();
            List<Task> tasks = new();

            Parallel.For(1, noOfIpAddresses, i => { tasks.Add(PingAsync($"{subnet}.{i}")); });

            Task.WaitAll(tasks.ToArray());

            stopWatch.Stop();
            string timeTaken = stopWatch.Elapsed.Duration().ToString();
            Console.WriteLine($"Asynchorous Ping took : {timeTaken} to finish pinging {noOfIpAddresses - 1} IP Addresses.");
        }
    }
}
