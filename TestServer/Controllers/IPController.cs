using kentxxq.Utils;
using kentxxq.Utils.Models;
using Microsoft.AspNetCore.Mvc;

namespace TestServer.Controllers;

/// <summary>
/// ip控制器
/// </summary>
[ApiExplorerSettings(GroupName = "V1")]
[ApiController]
[Route("api/[controller]/[action]")]
public class IPController:Controller
{
    /// <summary>
    /// 访问者的ip信息
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IpInfo> Info()
    {
        var ipService = new IP(new HttpClient());
        var ipInfo =await ipService.GetIpInfoByIp(HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
        return ipInfo;
    }
}