using System.Net;
using System.Net.Sockets;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using TestServer.Common;

namespace TestServer.Extensions;

public static class LogExtensions
{
    public static string DefaultLogTemplate =
        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}|{Level:u3}|{SourceContext}|{IP}|{MachineName}|{ThreadName}|{ThreadId}|{Message:lj}{Exception}{NewLine}";

    private static LoggerConfiguration AddCommonConfig(this LoggerConfiguration loggerConfiguration)
    {
        var ip = GetLocalIP();
        return loggerConfiguration
            .MinimumLevel.Is(LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.WithProperty("AppName", ThisAssembly.Project.AssemblyName)
            .Enrich.WithMachineName()
            .Enrich.WithProperty("IP",ip.ToString())
            .Enrich.When(logEvent => !logEvent.Properties.ContainsKey("SourceContext"),
                enrichmentConfig => enrichmentConfig.WithProperty("SourceContext", "SourceContext"))
            .Enrich.When(logEvent => !logEvent.Properties.ContainsKey("ThreadName"),
                enrichmentConfig => enrichmentConfig.WithProperty("ThreadName", "ThreadName"))
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .Enrich.WithThreadName();
    }

    public static LoggerConfiguration AddDefaultLogConfig(this LoggerConfiguration loggerConfiguration)
    {
        return loggerConfiguration
            .AddCommonConfig()
            .WriteTo.Async(l => l.File(
                path: $"{ThisAssembly.Project.AssemblyName}-.log",
                formatter: MyJsonFormatter.Formatter,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 1))
            .WriteTo.Async(l =>
                l.Console(
                    outputTemplate:DefaultLogTemplate,
                    theme: AnsiConsoleTheme.Code));
    }

    public static LoggerConfiguration AddCustomLogConfig(this LoggerConfiguration loggerConfiguration,
        IConfiguration configuration)
    {
        return loggerConfiguration
            .AddCommonConfig()
            .WriteTo.Async(l => l.File(
                path: configuration["KLog:File:Path"] ?? $"{ThisAssembly.Project.AssemblyName}-.log",
                formatter: MyJsonFormatter.Formatter,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: configuration.GetValue("KLog:File:RetainedFileCountLimit", 1))
            )
            .WriteTo.Async(l =>
                l.Console(
                    outputTemplate: configuration["KLog:Console:OutputTemplate"] ??DefaultLogTemplate,
                    theme: AnsiConsoleTheme.Code)
            );
    }

    public static LoggerConfiguration AddMyOpenTelemetry(this LoggerConfiguration loggerConfiguration,
        IConfiguration configuration)
    {
        return loggerConfiguration.WriteTo.OpenTelemetry(options =>
            {
                options.Endpoint = configuration["OC_Endpoint"] ??
                                   throw new InvalidOperationException("必须配置open telemetry的collector地址");
                options.ResourceAttributes["service.name"] = ThisAssembly.Project.AssemblyName;
                options.ResourceAttributes["job"] = ThisAssembly.Project.AssemblyName;
            }
        );
    }


    /// <summary>
    /// 获取本机ipv4内网ip<br />
    /// 如果网络不可用返回127.0.0.1<br />
    /// 如果之前网络可用，可能会返回之前保留下来的ip地址
    /// </summary>
    /// <returns></returns>
    public static IPAddress GetLocalIP()
    {
        try
        {
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
            socket.Connect("223.5.5.5", 53);
            var endPoint = socket.LocalEndPoint as IPEndPoint;
            endPoint ??= new IPEndPoint(IPAddress.Loopback, 0);
            return endPoint.Address;
        }
        catch (Exception)
        {
            return IPAddress.Loopback;
        }
    }
}
