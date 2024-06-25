using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace TestServer.Common;

public static class StaticNetTool
{
    /// <summary>
    /// 获取本机ipv4内网ip<br />
    /// 如果网络不可用返回127.0.0.1<br />
    /// 如果之前网络可用，可能会返回之前保留下来的ip地址
    /// </summary>
    /// <returns></returns>
    public static IPAddress GetLocalIP()
    {
        try
        {
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
            socket.Connect("223.5.5.5", 53);
            var endPoint = socket.LocalEndPoint as IPEndPoint;
            endPoint ??= new IPEndPoint(IPAddress.Loopback, 0);
            return endPoint.Address;
        }
        catch (Exception)
        {
            return IPAddress.Loopback;
        }
    }
}
