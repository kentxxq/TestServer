namespace TestServer.Tools.Ip.IpApi;

public static class IpApiTool
{
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