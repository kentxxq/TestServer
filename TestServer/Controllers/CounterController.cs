using Microsoft.AspNetCore.Mvc;

namespace TestServer.Controllers;

/// <summary>counter控制器</summary>
[ApiExplorerSettings(GroupName = "v1")]
[ApiController]
[Route("[controller]/[action]")]
public class CounterController : ControllerBase
{
    private static int _count;

    [EndpointDescription("+1")]
    [HttpGet]
    public string Count()
    {
        _count++;
        // int会被改成json格式返回,而string不会
        return _count.ToString();
    }

    [EndpointDescription("重置计数器")]
    [HttpGet]
    public string Reset()
    {
        _count = 0;
        return _count.ToString();
    }
}
