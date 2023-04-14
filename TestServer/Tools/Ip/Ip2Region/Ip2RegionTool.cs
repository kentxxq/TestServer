using IP2Region.Net.XDB;

namespace TestServer.Tools.Ip.Ip2Region;

public static class Ip2RegionTool
{
    public static IpServiceModel GetIpInfo(string ip)
    {
        var search = new Searcher();
        var data = search.Search(ip);
        var dataList = data.Split("|");
        var result = new IpServiceModel {
            Status = "success",
            Country = dataList[0],
            RegionName = dataList[2],
            Isp = dataList[4],
            City = dataList[3]
        };
        return result;
    }
}