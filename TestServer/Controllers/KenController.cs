﻿using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using TestServer.Service;

namespace TestServer.Controllers;

/// <summary>ken控制器</summary>
[ApiExplorerSettings(GroupName = "v1")]
[ApiController]
[Route("[controller]")]
public class KenController : ControllerBase
{
    private const string ProxyServerUrl = "https://ghp.ci/";
    private readonly IpService _ipService;
    private readonly ILogger<KenController> _logger;

    private readonly List<string> KenPlatform = new()
    {
        "ken-win-x64.exe", "ken-win-x86.exe", "ken-win-arm64.exe", "ken-win-arm.exe",
        "ken-osx-x64", "ken-osx-arm64",
        "ken-linux-x64", "ken-linux-musl-x64", "ken-linux-arm64", "ken-linux-arm"
    };

    /// <inheritdoc />
    public KenController(ILogger<KenController> logger, IpService ipService)
    {
        _logger = logger;
        _ipService = ipService;
    }

    [EndpointDescription("列出所有的平台版本")]
    [HttpGet]
    public string ListPlatform()
    {
        return string.Join("\n", KenPlatform);
    }


    [EndpointDescription("跳转到特定的版本")]
    [HttpGet("{name?}")]
    public async Task<IActionResult> DownloadRedirect([Description("版本号")]string name)
    {
        string url;
        if (!string.IsNullOrEmpty(name) && KenPlatform.Contains(name))
        {
            var inChina = await _ipService.InChina(HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
            _logger.LogInformation($"ip检测在中国:{inChina}");
            url = inChina
                ? $"{ProxyServerUrl}https://github.com/kentxxq/kentxxq.Cli/releases/latest/download/{name}"
                : $"https://github.com/kentxxq/kentxxq.Cli/releases/latest/download/{name}";

            return RedirectPermanent(url);
        }

        return Content("版本不存在");
    }
}
