using TestServer.Tools.Ip.Models;

namespace TestServer.Tools.Ip;

/// <summary>
/// 获取ip信息的静态类
/// </summary>
public static class IPInfo
{
    /// <summary>
    /// 通过ip-api域名获取ip信息
    /// </summary>
    /// <param name="ip"></param>
    /// <returns></returns>
    /// <exception cref="ApplicationException"></exception>
    public static async Task<IpServiceModel> GetIpInfo(string ip)
    {
        var httpClient = new HttpClient();
        var data = await httpClient.GetFromJsonAsync<IpApiModel>($"http://ip-api.com/json/{ip}?lang=zh-CN");
        if (data!.Status != "success") throw new ApplicationException("查询失败");
        var result = new IpServiceModel
        {
            Status = data.Status, Country = data.Country, RegionName = data.RegionName, Isp = data.Isp, City = data.City
        };
        return result;
    }
}