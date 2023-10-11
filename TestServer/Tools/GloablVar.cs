namespace TestServer.Tools;

public class GloablVar
{
    /// <summary>最后一次请求ip-api.com失败的时间. 一旦无法联通ip-api.com,一定时间不再重新尝试.</summary>
    public DateTime IpApiErrorTime { get; set; }
}
