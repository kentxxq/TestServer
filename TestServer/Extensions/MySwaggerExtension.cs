using Microsoft.OpenApi.Models;

namespace TestServer.Extensions;

/// <summary>
/// swagger-拓展方法
/// </summary>
public static class MySwaggerExtension
{
    /// <summary>
    /// 添加swagger配置
    /// </summary>
    /// <param name="service"></param>
    /// <returns></returns>
    public static IServiceCollection AddMySwagger(this IServiceCollection service)
    {
        
        service.AddEndpointsApiExplorer();
        service.AddSwaggerGen(s =>
        {
            s.SwaggerDoc("V1", new OpenApiInfo { Title = "V1", Version = "V1" });

            // xmlDoc
            var filePath = Path.Combine(AppContext.BaseDirectory, "MyApi.xml");
            s.IncludeXmlComments(filePath);
        });

        return service;
    }
}