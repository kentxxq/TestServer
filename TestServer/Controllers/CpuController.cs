using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using TestServer.Service;

namespace TestServer.Controllers;

/// <summary>cpu控制器</summary>
[ApiExplorerSettings(GroupName = "v1")]
[ApiController]
[Route("[controller]/[action]")]
public class CpuController : ControllerBase
{
    private readonly ICpuLoadService _cpuLoadService;

    public CpuController(ICpuLoadService cpuLoadService)
    {
        _cpuLoadService = cpuLoadService;
    }

    [EndpointDescription("轻量负载,运算10**6次开平方")]
    [HttpGet("light")]
    public IActionResult LightLoad()
    {
        _cpuLoadService.LightLoad();
        return Ok("Light load completed");
    }

    [EndpointDescription("中等负载,运算10**7次开平方")]
    [HttpGet("medium")]
    public IActionResult MediumLoad()
    {
        _cpuLoadService.MediumLoad();
        return Ok("Medium load completed");
    }

    [EndpointDescription("重度负载,运算10**8次开平方")]
    [HttpGet("heavy")]
    public IActionResult HeavyLoad()
    {
        _cpuLoadService.HeavyLoad();
        return Ok("Heavy load completed");
    }

    [EndpointDescription("自定义负载次数,int最大值2147483647")]
    [HttpGet("{count}")]
    public IActionResult CustomLoad([Description("负载次数")]int count)
    {
        _cpuLoadService.CustomLoad(count);
        return Ok($"{count} load completed");
    }
}
