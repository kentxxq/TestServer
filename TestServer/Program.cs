using System.Reflection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Sinks.SystemConsole.Themes;
using TestServer.Extensions;
using TestServer.Service;

var logTemplate = "{Timestamp:HH:mm:ss}|{Level:u3}|{SourceContext}|{Message:lj}{Exception}{NewLine}";

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: logTemplate, theme: AnsiConsoleTheme.Code)
    .WriteTo.File(path: $"{Assembly.GetEntryAssembly()?.GetName().Name}-.log", formatter: new JsonFormatter(),
        rollingInterval: RollingInterval.Day, retainedFileCountLimit: 1)
    .CreateLogger();
Log.Information("启动中...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddGrpc();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddMySwagger();
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        // 允许非本机ip转发 options.KnownProxies.Add(IPAddress.Parse("127.0.10.1"));
        // 允许非规范header头 options.ForwardedForHeaderName = "X-Forwarded-For-My-Custom-Header-Name";
    });

    var app = builder.Build();
    
    #region 生命周期

    app.Lifetime.ApplicationStarted.Register(() =>
    {
        Log.Information("ApplicationStarted:启动完成");
    });
    app.Lifetime.ApplicationStopping.Register(() =>
    {
        // shutdown会停止，直到下面的语句执行完成
        Log.Warning("ApplicationStopping:正在关闭");
    });
    app.Lifetime.ApplicationStopped.Register(() =>
    {
        Log.Warning("ApplicationStopped:应用已停止");
    });

    #endregion

    // Configure the HTTP request pipeline.
    app.UseForwardedHeaders();

    app.UseSwagger();
    app.UseSwaggerUI(u =>
    {
        u.SwaggerEndpoint("/swagger/V1/swagger.json", "V1");
    });

    app.UseRouting();
    app.UseAuthorization();

    var webSocketOptions = new WebSocketOptions
    {
        KeepAliveInterval = TimeSpan.FromSeconds(120),
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


