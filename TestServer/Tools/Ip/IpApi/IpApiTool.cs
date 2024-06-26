﻿using TestServer.Service;

namespace TestServer.Tools.Ip.IpApi;

/// <summary>ip-api工具</summary>
public static class IpApiTool
{
    /// <summary>获取ip信息</summary>
    /// <param name="ip"></param>
    /// <returns></returns>
    /// <exception cref="ApplicationException"></exception>
    public static async Task<IpServiceModel> GetIpInfo(string ip)
    {
        var httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(5)
        };
        var data = await httpClient.GetFromJsonAsync<IpApiModel>($"http://ip-api.com/json/{ip}?lang=zh-CN");
        if (data!.Status != "success")
        {
            throw new TaskCanceledException("查询失败");
        }

        var result = new IpServiceModel
        {
            Status = IpServiceQueryStatus.success,
            IP = ip,
            Country = data.Country,
            RegionName = data.RegionName,
            Isp = data.Isp,
            City = data.City
        };
        return result;
    }
}
