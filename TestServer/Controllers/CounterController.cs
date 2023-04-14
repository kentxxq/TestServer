using Microsoft.AspNetCore.Mvc;

namespace TestServer.Controllers;

/// <summary>
/// counter控制器
/// </summary>
[ApiExplorerSettings(GroupName = "V1")]
[ApiController]
[Route("[controller]/[action]")]
public class CounterController : ControllerBase
{
    private static int _count;

    /// <summary>
    /// +1
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public int Count()
    {
        _count++;
        return _count;
    }

    /// <summary>
    /// 重置计数器
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public int Reset()
    {
        _count = 0;
        return _count;
    }
}