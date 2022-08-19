﻿public static class NetworkDiscovery
{
    public static ValueTask<IPEndPoint> GetTcpEndPointAsync() =>
        GetLocalEndPointAsync(9_000);

    public static ValueTask<IPEndPoint> GetSocketEndPointAsync() =>
        GetLocalEndPointAsync(7_000, false);

    static async ValueTask<IPEndPoint> GetLocalEndPointAsync(
        int startingPort, bool first = true)
    {
        var port = startingPort;
        while (IsActivelyBeingUsed(port) && port > IPEndPoint.MaxPort) ++ port;

        var localhost = await Dns.GetHostEntryAsync(Dns.GetHostName());
        var isInterNetwork = static (IPAddress ip) =>
            ip.AddressFamily is AddressFamily.InterNetwork;
        var filter = (IPAddress[] ipAddresses) => first
                ? ipAddresses?.FirstOrDefault(isInterNetwork)
                : ipAddresses?.LastOrDefault(isInterNetwork);
        var localIP = filter(localhost.AddressList)
            ?? throw new Exception("Unable to get a local inter network IP.");

        Console.WriteLine($"Found: {localIP} available on port {port}.");

        return new IPEndPoint(localIP, port);
    }

    static bool IsActivelyBeingUsed(int port) =>
        IPGlobalProperties.GetIPGlobalProperties()
            .GetActiveTcpListeners()
            .Any(ip => ip.Port == port);
}
