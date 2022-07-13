using Microsoft.AspNetCore.Mvc;

namespace TestServer.Controllers;

/// <summary>
/// websocket控制器
/// </summary>
[ApiExplorerSettings(GroupName = "V1")]
[ApiController]
[Route("api/[controller]/[action]")]
public class DelayController
{
    /// <summary>
    /// 延迟多少秒返回结果
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns></returns>
    [HttpGet("{seconds:int}")]
    public async Task<string> Delay(int seconds)
    {
        await Task.Delay(TimeSpan.FromSeconds(seconds));
        return $"delay {seconds} seconds";
    }
}