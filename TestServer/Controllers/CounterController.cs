using Microsoft.AspNetCore.Mvc;

namespace TestServer.Controllers;

/// <summary>counter控制器</summary>
[ApiExplorerSettings(GroupName = "V1")]
[ApiController]
[Route("[controller]/[action]")]
public class CounterController : ControllerBase
{
    private static int _count;

    /// <summary>+1</summary>
    /// <returns></returns>
    [HttpGet]
    public string Count()
    {
        _count++;
        // int会被改成json格式返回,而string不会
        return _count.ToString();
    }

    /// <summary>重置计数器</summary>
    /// <returns></returns>
    [HttpGet]
    public string Reset()
    {
        _count = 0;
        return _count.ToString();
    }
}
