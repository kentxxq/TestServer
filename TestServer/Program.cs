using System.Diagnostics;
using IP2Region.Net.Abstractions;
using IP2Region.Net.XDB;
using Microsoft.AspNetCore.HttpOverrides;
using Scalar.AspNetCore;
using Serilog;
using TestServer.Extensions;
using TestServer.Service;
using TestServer.Tools;

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Configuration.AddUserSecrets(typeof(Program).Assembly);
    var instanceId = Guid.NewGuid().ToString();
    var enableOpentelemetry = builder.Configuration.GetValue(LogExtensions.OpenTelemetryConfigName, false);
// AddMyOpenTelemetry必须一起使用
// if (enableOpentelemetry)
// {
//     // 必须在AddMyOpenTelemetry之前,不能和下面的放在一起
//     builder.AddMyOpenTelemetry(instanceId);
// }
    builder.Services.AddSerilog((services, lc) => {
        lc.AddCustomLogConfig(builder.Configuration);
        // 必须和AddMyOpenTelemetry配合使用
        // if (enableOpentelemetry)
        // {
        //     lc.AddMyOpenTelemetry(builder.Configuration,instanceId);
        // }
    });

    builder.Services.AddGrpc();
    builder.Services.AddSingleton<ISearcher>(new Searcher(CachePolicy.Content,
        Path.Combine(AppContext.BaseDirectory, "ip2region.xdb")));
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        // 允许非本机ip转发 options.KnownProxies.Add(IPAddress.Parse("127.0.10.1"));
        // 允许非规范header头 options.ForwardedForHeaderName = "X-Forwarded-For-My-Custom-Header-Name";
    });

    // ip服务
    builder.Services.AddTransient<IpService>();
    // 全局变量
    builder.Services.AddSingleton<GlobalVar>();
    // cpu负载服务
    builder.Services.AddScoped<ICpuLoadService, MathCpuLoadService>();

    var app = builder.Build();

    #region 生命周期

    app.Lifetime.ApplicationStarted.Register(() => { Log.Information("ApplicationStarted:启动完成"); });
    app.Lifetime.ApplicationStopping.Register(() =>
    {
        // shutdown会停止，直到下面的语句执行完成
        Log.Warning("ApplicationStopping:正在关闭");
    });
    app.Lifetime.ApplicationStopped.Register(() => { Log.Warning("ApplicationStopped:应用已停止"); });

    #endregion

    // header添加TraceId
    app.Use(async (context, next) =>
    {
        context.Response.Headers.Append("TraceId", Activity.Current?.TraceId.ToString());
        await next();
    });

    // 简化http输出
    app.UseSerilogRequestLogging();

    app.UseForwardedHeaders();

    app.MapOpenApi();
    app.MapScalarApiReference(option =>
    {
        // 使用正确的base url
        // https://github.com/dotnet/aspnetcore/issues/57332, 或者添加库 https://github.com/dotnet/aspnetcore/issues/57332#issuecomment-2479286855
        option.Servers = [];
    });

    if (enableOpentelemetry)
    {
        app.UseOpenTelemetryPrometheusScrapingEndpoint();
    }

    app.UseRouting();

    var webSocketOptions = new WebSocketOptions
    {
        KeepAliveInterval = TimeSpan.FromSeconds(120)
    };
    app.UseWebSockets(webSocketOptions);

    app.MapControllers();

    app.MapGrpcService<GreeterService>();

    app.Run();
    return 0;
}
catch (Exception exception)
{
    Log.Fatal(exception, "异常退出...");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}
