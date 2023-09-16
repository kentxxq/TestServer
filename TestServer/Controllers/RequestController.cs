using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace TestServer.Controllers;

/// <summary>request控制器</summary>
[ApiExplorerSettings(GroupName = "V1")]
[ApiController]
[Route("[controller]")]
public class RequestController : Controller
{
    /// <summary>返回请求的相关信息</summary>
    /// <returns></returns>
    [HttpGet]
    [HttpPost]
    [HttpDelete]
    [HttpPut]
    [HttpHead]
    [HttpPatch]
    [HttpOptions]
    public async Task<string> ReturnRequestInfo()
    {
        Dictionary<string, object> data = new();

        var headers = HttpContext.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
        if (headers.Count > 0)
        {
            data.Add("Headers", headers);
        }

        data.Add("method", HttpContext.Request.Method);
        data.Add("schema", HttpContext.Request.Scheme);
        data.Add("host", HttpContext.Request.Host);
        data.Add("path", HttpContext.Request.Path);
        data.Add("queryString",
            string.IsNullOrEmpty(HttpContext.Request.QueryString.Value)
                ? "无查询参数"
                : HttpContext.Request.QueryString.Value);

        using var reader = new StreamReader(HttpContext.Request.Body);
        var requestBody = await reader.ReadToEndAsync();

        if (!string.IsNullOrEmpty(requestBody))
        {
            data.Add("body", requestBody);
        }

        // if (!string.IsNullOrEmpty(requestBody))
        // {
        //     if (headers["Content-Type"] == "application/json")
        //     {
        //         var json = JsonSerializer.Serialize(JsonDocument.Parse(requestBody).RootElement, new JsonSerializerOptions
        //         {
        //             Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        //             WriteIndented = false
        //         });
        //         data.Add("body",json);
        //     }
        //     else
        //     {
        //         data.Add("body",requestBody);
        //     }
        // }

        return JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
    }
}
