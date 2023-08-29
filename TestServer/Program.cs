using System.Reflection;
using IP2Region.Net.Abstractions;
using IP2Region.Net.XDB;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Sinks.SystemConsole.Themes;
using TestServer.Extensions;
using TestServer.Service;

// 时间、时区 | 级别 | SourceContext | 线程名称 | 线程id | 信息/异常 
// 2023-06-15 21:39:48.254 +08:00|INF|Serilog.AspNetCore.RequestLoggingMiddleware|.NET ThreadPool Worker|11|HTTP GET /Counter/Count responded 200 in 0.2160 ms
const string logTemplate =
    "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}|{Level:u3}|{SourceContext}|{ThreadName}|{ThreadId}|{Message:lj}{Exception}{NewLine}";

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.When(logEvent => !logEvent.Properties.ContainsKey("SourceContext"),
        enrichmentConfig => enrichmentConfig.WithProperty("SourceContext", "SourceContext"))
    .Enrich.When(logEvent => !logEvent.Properties.ContainsKey("ThreadName"),
        enrichmentConfig => enrichmentConfig.WithProperty("ThreadName", "ThreadName"))
    .Enrich.FromLogContext()
    .Enrich.WithThreadId()
    .Enrich.WithThreadName()
    .WriteTo.Async(l => l.File(path: $"{Assembly.GetEntryAssembly()?.GetName().Name}-.log",
        formatter: new JsonFormatter(),
        rollingInterval: RollingInterval.Day, retainedFileCountLimit: 1))
    .WriteTo.Async(l => l.Console(outputTemplate: logTemplate, theme: AnsiConsoleTheme.Code))
    // .WriteTo.Console(outputTemplate: logTemplate, theme: AnsiConsoleTheme.Code)
    // .WriteTo.File(path: $"{Assembly.GetEntryAssembly()?.GetName().Name}-.log", formatter: new JsonFormatter(),
    //     rollingInterval: RollingInterval.Day, retainedFileCountLimit: 1)
    .CreateLogger();
Log.Information("启动中...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddGrpc();
    builder.Services.AddSingleton<ISearcher>(new Searcher(CachePolicy.Content,
        Path.Combine(AppContext.BaseDirectory, "ip2region.xdb")));
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddMySwagger();
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        // 允许非本机ip转发 options.KnownProxies.Add(IPAddress.Parse("127.0.10.1"));
        // 允许非规范header头 options.ForwardedForHeaderName = "X-Forwarded-For-My-Custom-Header-Name";
    });

    builder.Services.AddTransient<IpService>();

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

    // 简化http输出
    app.UseSerilogRequestLogging();

    app.UseForwardedHeaders();

    app.UseSwagger();
    app.UseSwaggerUI(u => { u.SwaggerEndpoint("/swagger/V1/swagger.json", "V1"); });

    app.UseRouting();
    app.UseAuthorization();

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