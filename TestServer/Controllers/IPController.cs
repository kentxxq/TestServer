using Microsoft.AspNetCore.Mvc;
using TestServer.Tools.Ip;
using TestServer.Tools.Ip.Models;

namespace TestServer.Controllers;

/// <summary>
/// ip控制器
/// </summary>
[ApiExplorerSettings(GroupName = "V1")]
[ApiController]
[Route("[controller]")]
public class IPController : ControllerBase
{
    /// <summary>
    /// 访问者的ip信息
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IpServiceModel> VisitorInfo()
    {
        var result = await IPInfo.GetIpInfo(HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
        return result;
    }
    
    /// <summary>
    /// 查看特定的ip信息
    /// </summary>
    /// <returns></returns>
    [HttpGet("{ip?}")]
    public async Task<IpServiceModel> IpInfo(string ip)
    {
        var result = await IPInfo.GetIpInfo(ip);
        return result;
    }
}