using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TestServer.Service;
using TestServer.Tools;

namespace TestServer.Controllers;

/// <summary>
/// ip控制器
/// 虽然浏览器显示json格式有美化，但是为了在curl的时候更好看，还是选择了返回字符串
/// </summary>
[ApiExplorerSettings(GroupName = "V1")]
[ApiController]
[Route("[controller]")]
public class IPController : ControllerBase
{
    private readonly IpService _ipService;

    /// <summary>依赖注入</summary>
    /// <param name="ipService"></param>
    public IPController(IpService ipService)
    {
        _ipService = ipService;
    }

    /// <summary>访问者的ip信息</summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<string> VisitorInfo()
    {
        var result = await _ipService.GetIpInfo(HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
        return JsonSerializer.Serialize(result, StaticData.PrettyPrintJsonSerializerOptions);
    }

    /// <summary>查看特定的ip信息</summary>
    /// <returns></returns>
    [HttpGet("{ip?}")]
    public async Task<string> IpInfo(string ip)
    {
        var result = await _ipService.GetIpInfo(ip);
        return JsonSerializer.Serialize(result, StaticData.PrettyPrintJsonSerializerOptions);
    }
}
