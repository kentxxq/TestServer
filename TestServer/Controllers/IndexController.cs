using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Swagger;

namespace TestServer.Controllers;

/// <summary>
/// index控制器
/// </summary>
[ApiExplorerSettings(GroupName = "V1")]
[ApiController]
public class IndexController : ControllerBase
{
    private readonly ISwaggerProvider _swaggerProvider;

    /// <summary>
    /// 数据
    /// </summary>
    /// <param name="swaggerProvider"></param>
    public IndexController(ISwaggerProvider swaggerProvider)
    {
        _swaggerProvider = swaggerProvider;
    }

    /// <summary>
    /// 返回需要swagger的简要信息
    /// </summary>
    /// <returns></returns>
    [HttpGet("/")]
    public string Index()
    {
        var swaggerDoc = _swaggerProvider.GetSwagger("V1");
        var data = swaggerDoc.Paths.ToDictionary(path => path.Key, path => path.Value.Operations.First().Value.Summary);
        var result = JsonSerializer.Serialize(data, new JsonSerializerOptions()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
        return result;
    }
}