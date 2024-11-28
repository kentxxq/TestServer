using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;

namespace TestServer.Controllers;

/// <summary>counter控制器</summary>
[ApiExplorerSettings(GroupName = "v1")]
[ApiController]
[Route("[controller]/[action]")]
public class LogController : ControllerBase
{
    private readonly ILogger<LogController> _logger;

    /// <inheritdoc />
    public LogController(ILogger<LogController> logger)
    {
        _logger = logger;
    }

    [EndpointDescription("记录特定条数的日志")]
    [HttpGet("{count:int}")]
    public string LogNum([Description("日志数")]int count)
    {
        _logger.LogInformation($"收到请求，记录{count}次日志");
        for (var i = 1; i < count + 1; i++)
        {
            _logger.LogInformation($"这是第{i}条记录");
        }

        return $"已记录{count}条日志";
    }
}
