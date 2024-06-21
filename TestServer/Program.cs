using System.Diagnostics;
using System.Reflection;
using IP2Region.Net.Abstractions;
using IP2Region.Net.XDB;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Sinks.OpenTelemetry;
using Serilog.Sinks.SystemConsole.Themes;
using TestServer.Extensions;
using TestServer.Service;
using TestServer.Tools;

Log.Logger = new LoggerConfiguration()
    .AddDefaultLogConfig()
    .CreateBootstrapLogger();

Log.Information("日志初始化完成,正在启动服务...");

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Configuration.AddUserSecrets(typeof(Program).Assembly);
    var enableOpentelemetry = builder.Configuration.GetValue<bool>("EnableOpenTelemetry", false);

    builder.Host.UseSerilog((serviceProvider, loggerConfiguration) =>
    {
        loggerConfiguration.AddCustomLogConfig(builder.Configuration);
        if (enableOpentelemetry)
        {
            loggerConfiguration.WriteTo.OpenTelemetry(builder.Configuration["OC_Endpoint"] ??
                                                       throw new InvalidOperationException("必须配置open telemetry的collector地址"));
            // opentelemetry采集
            builder.AddMyOpenTelemetry();
        }
    });

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

    app.UseSwagger();
    app.UseSwaggerUI(u => { u.SwaggerEndpoint("/swagger/V1/swagger.json", "V1"); });

    if (enableOpentelemetry)
    {
        app.UseOpenTelemetryPrometheusScrapingEndpoint();
    }

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
