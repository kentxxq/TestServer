using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;

namespace TestServer.Controllers;

/// <summary>delay控制器</summary>
[ApiController]
[Route("[controller]")]
public class DelayController : ControllerBase
{
    [EndpointDescription("延迟多少毫秒返回结果")]
    [HttpGet("{millisecond:int}")]
    public async Task<string> Delay([Description("毫秒数")]int millisecond)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(millisecond));
        return $"delay {millisecond} ms";
    }
}
