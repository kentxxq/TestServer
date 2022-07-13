using Microsoft.AspNetCore.Mvc;

namespace TestServer.Controllers;

/// <summary>
/// websocket控制器
/// </summary>
[ApiExplorerSettings(GroupName = "V1")]
[ApiController]
[Route("api/[controller]")]
public class DelayController
{
    /// <summary>
    /// 延迟多少毫秒返回结果
    /// </summary>
    /// <param name="millisecond">毫秒数</param>
    /// <returns></returns>
    [HttpGet("{millisecond:int}")]
    public async Task<string> Delay(int millisecond)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(millisecond));
        return $"delay {millisecond} seconds";
    }
}