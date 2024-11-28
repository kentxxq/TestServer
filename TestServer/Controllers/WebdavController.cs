using System.ComponentModel;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using WebDav;

namespace TestServer.Controllers;

/// <summary>delay控制器</summary>
[ApiExplorerSettings(GroupName = "v1")]
[ApiController]
[Route("[controller]")]
public class WebdavController : ControllerBase
{
    [EndpointDescription("获取特定webdav文件")]
    [HttpGet]
    public async Task<IActionResult> GetWebdavFile([Description("服务器")]string server,[Description("用户名")]string username,[Description("密码")]string password,[Description("文件路径(相对路径)")]string filePath)
    {
        var clientParams = new WebDavClientParams
        {
            BaseAddress = new Uri(server),
            Credentials = new NetworkCredential(username,password)
        };
        var client = new WebDavClient(clientParams);
        var stream = await client.GetRawFile(filePath);
        Response.Headers.Append("Content-Disposition", $"attachment; filename={Path.GetFileName(filePath)}");
        // clash必须是text/plain
        // return new FileStreamResult(stream.Stream, "application/octet-stream");
        return File(stream.Stream, "text/plain");
    }
}
