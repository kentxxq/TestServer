using IP2Region.Net.XDB;
using TestServer.Service;

namespace TestServer.Tools.Ip.Ip2Region;

/// <summary>Ip2Region工具</summary>
public static class Ip2RegionTool
{
    /// <summary>获取ip信息</summary>
    /// <param name="ip"></param>
    /// <returns></returns>
    public static IpServiceModel GetIpInfo(string ip)
    {
        var search = new Searcher(CachePolicy.Content, "ip2region.xdb");
        var data = search.Search(ip);
        IpServiceModel result;
        if (!string.IsNullOrEmpty(data))
        {
            var dataList = data.Split("|");
            result = new IpServiceModel
            {
                Status = IpServiceQueryStatus.success,
                IP = ip,
                Country = dataList[0],
                RegionName = dataList[2],
                Isp = dataList[4],
                City = dataList[3]
            };
        }
        else
        {
            result = new IpServiceModel
            {
                Status = IpServiceQueryStatus.fail,
                IP = ip,
                Country = string.Empty,
                RegionName = string.Empty,
                Isp = string.Empty,
                City = string.Empty
            };
        }

        return result;
    }
}
