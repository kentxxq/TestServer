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
    ///     混合负载,顺序执行轻度-中度-重度
    /// </summary>
    /// <returns></returns>
    [HttpGet("mix")]
    public IActionResult MixLoad()
    {
        _cpuLoadService.LightLoad();
        _cpuLoadService.MediumLoad();
        _cpuLoadService.HeavyLoad();
        return Ok("mixed load completed");
    }
}
