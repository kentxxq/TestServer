using Microsoft.AspNetCore.Mvc;
using TestServer.Service;

namespace TestServer.Controllers;

/// <summary>cpu控制器</summary>
[ApiExplorerSettings(GroupName = "V1")]
[ApiController]
[Route("[controller]/[action]")]
public class CpuController : ControllerBase
{
    private readonly ICpuLoadService _cpuLoadService;

    public CpuController(ICpuLoadService cpuLoadService)
    {
        _cpuLoadService = cpuLoadService;
    }

    /// <summary>
    ///     轻量负载
    /// </summary>
    /// <returns></returns>
    [HttpGet("light")]
    public IActionResult LightLoad()
    {
        _cpuLoadService.LightLoad();
        return Ok("Light load completed");
    }

    /// <summary>
    ///     中等负载
    /// </summary>
    /// <returns></returns>
    [HttpGet("medium")]
    public IActionResult MediumLoad()
    {
        _cpuLoadService.MediumLoad();
        return Ok("Medium load completed");
    }

    /// <summary>
    ///     重度负载
    /// </summary>
    /// <returns></returns>
    [HttpGet("heavy")]
    public IActionResult HeavyLoad()
    {
        _cpuLoadService.HeavyLoad();
        return Ok("Heavy load completed");
    }

    /// <summary>
    ///     自定义负载次数,int最大值2147483647
    /// </summary>
    /// <returns></returns>
    [HttpGet("{count}")]
    public IActionResult CustomLoad(int count)
    {
        _cpuLoadService.CustomLoad(count);
        return Ok($"{count} load completed");
    }
}
