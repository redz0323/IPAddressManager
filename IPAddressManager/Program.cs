using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

string subnet = "192.168.1";
//SequentialPing();
ParallelPing();

void SequentialPing()
{
    Console.WriteLine("==================== Synchronous Ping ====================");

    int noOfIpAddresses = 16;
    Stopwatch stopWatch = new Stopwatch();
    stopWatch.Start();
    for (int i = 1; i < noOfIpAddresses; i++)
        PingIPAddress($"{subnet}.{i}");

    stopWatch.Stop();
    string timeTaken = stopWatch.Elapsed.Duration().ToString();
    Console.WriteLine($"Synchorous Ping took : {timeTaken} to finish pinging {noOfIpAddresses - 1} IP Addresses.");
}

void ParallelPing()
{
    Console.WriteLine("==================== Asynchronous Ping ====================");

    int noOfIpAddresses = 255;
    Stopwatch stopWatch = new Stopwatch();
    stopWatch.Start();
    Parallel.For(1, noOfIpAddresses, i => { PingIPAddress($"{subnet}.{i}"); });
    stopWatch.Stop();
    string timeTaken = stopWatch.Elapsed.Duration().ToString();
    Console.WriteLine($"Asynchorous Ping took : {timeTaken} to finish pinging {noOfIpAddresses - 1} IP Addresses.");
}

void PingIPAddress(string ip)
{
    Ping ping = new Ping();
    PingReply reply = ping.Send(ip, 100);

    int currentCore = Thread.GetCurrentProcessorId();
    try
    {
        IPHostEntry host = Dns.GetHostEntry(IPAddress.Parse(ip));
        if (host != null)
        {
            string status = reply.Status == IPStatus.Success ? "UP" : "DOWN";
            Console.WriteLine($"{ip} : {host.HostName} => {status} :: Proccessed on Thread #{currentCore}");
        }
    }
    catch (SocketException)
    {
        Console.WriteLine($"{ip} => VACANT :: Proccessed on Thread #{currentCore}");
    }
}