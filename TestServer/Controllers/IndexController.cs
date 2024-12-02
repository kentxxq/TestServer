using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using TestServer.Common;

namespace TestServer.Controllers;

/// <summary>index控制器</summary>
[ApiExplorerSettings(GroupName = "v1")]
[ApiController]
public class IndexController : ControllerBase
{
    private readonly IApiDescriptionGroupCollectionProvider _apiDescriptionGroupCollectionProvider;

    public IndexController(IApiDescriptionGroupCollectionProvider apiDescriptionGroupCollectionProvider)
    {
        _apiDescriptionGroupCollectionProvider = apiDescriptionGroupCollectionProvider;
    }


    /// <summary>返回需要openapi的简要信息</summary>
    /// <returns></returns>
    [HttpGet("/")]
    public string Index()
    {
        var apiDescriptionGroup = _apiDescriptionGroupCollectionProvider.ApiDescriptionGroups.Items.First(t => t.GroupName == "v1");
        var dict = apiDescriptionGroup.Items.ToDictionary(t => $"{t.HttpMethod} /{t.RelativePath}",
            t =>
            {
                var desc =
                    t.ActionDescriptor.EndpointMetadata.FirstOrDefault(m => m is EndpointDescriptionAttribute) as
                        EndpointDescriptionAttribute;
                return desc?.Description ?? t.ActionDescriptor.DisplayName;
            });

        var result = JsonSerializer.Serialize(dict, MyJsonSerializerOptions.Default);
        return result;
    }
}
