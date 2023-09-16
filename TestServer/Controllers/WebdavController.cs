using System.Net;
using Microsoft.AspNetCore.Mvc;
using TestServer.RO;
using WebDav;

namespace TestServer.Controllers;

/// <summary>delay控制器</summary>
[ApiExplorerSettings(GroupName = "V1")]
[ApiController]
[Route("[controller]")]
public class WebdavController : ControllerBase
{
    /// <summary>获取特定webdav文件</summary>
    /// <param name="webdavInfo">webdav的文件信息</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetWebdavFile([FromQuery] WebdavInfo webdavInfo)
    {
        var clientParams = new WebDavClientParams
        {
            BaseAddress = new Uri(webdavInfo.Server),
            Credentials = new NetworkCredential(webdavInfo.Username, webdavInfo.Password)
        };
        var client = new WebDavClient(clientParams);
        var stream = await client.GetRawFile(webdavInfo.FilePath);
        Response.Headers.Add("Content-Disposition", $"attachment; filename={Path.GetFileName(webdavInfo.FilePath)}");
        // clash必须是text/plain
        // return new FileStreamResult(stream.Stream, "application/octet-stream");
        return File(stream.Stream, "text/plain");
    }
}
