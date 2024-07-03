using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Pyroscope.OpenTelemetry;

namespace TestServer.Extensions;

/// <summary>OpenTelemetry-拓展方法</summary>
public static class MyOpenTelemetryExtension
{
    private const string AppName = ThisAssembly.Project.AssemblyName;
    private const string AppVersion = ThisAssembly.Info.Version;
    private static string _ocEndpoint = string.Empty;

    /// <summary>添加OpenTelemetry</summary>
    /// <param name="builder"></param>
    /// <param name="instanceId"></param>
    /// <returns></returns>
    public static void AddMyOpenTelemetry(this WebApplicationBuilder builder,string instanceId)
    {
        // 不要开代理!!!!!!
        // 不要开代理!!!!!!
        // 不要开代理!!!!!!
        _ocEndpoint = builder.Configuration["OC_Endpoint"] ??
                      throw new InvalidOperationException("必须配置open telemetry的collector地址");

        // 统一配置名称,版本,连接地址
        var openTelemetryBuilder = builder.Services.AddOpenTelemetry()
            .UseOtlpExporter(OtlpExportProtocol.Grpc, new Uri(_ocEndpoint))
            .ConfigureResource(resourceBuilder => resourceBuilder.AddService(AppName, serviceVersion: AppVersion,serviceInstanceId:instanceId));

        // 日志让serilog自己发送
        // openTelemetryBuilder.WithLogging();

        // 指标
        openTelemetryBuilder
            .WithMetrics(meterProviderBuilder => meterProviderBuilder
                // .AddConsoleExporter((options, readerOptions) =>
                // {
                //     readerOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 1000;
                // })
                // debug用
                .AddPrometheusExporter()
                // 默认60秒导出一次,但无法和UseOtlpExporter兼容 https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/src/OpenTelemetry.Exporter.OpenTelemetryProtocol/README.md#enable-metric-exporter
                // .AddOtlpExporter((options, readerOptions) =>
                // {
                //     readerOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 1000;
                // })
                .AddMeter(AppName)
                .AddProcessInstrumentation()
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddRuntimeInstrumentation()
                .AddEventCountersInstrumentation(o =>
                {
                    o.RefreshIntervalSecs = 1;
                    // https://learn.microsoft.com/en-us/dotnet/core/diagnostics/available-counters
                    // o.AddEventSources("System.Runtime"); 有RuntimeInstrumentation和AspNetCoreInstrumentation就够了
                    // o.AddEventSources("Microsoft.AspNetCore.Hosting");
                    // o.AddEventSources("Microsoft.AspNetCore.Http.Connections"); 没看到输出
                    // o.AddEventSources("Microsoft-AspNetCore-Server-Kestrel");
                    // o.AddEventSources("System.Net.Http");
                    o.AddEventSources("System.Net.NameResolution");
                    // o.AddEventSources("System.Net.Security"); 主要是ssl信息，挂在代理后面不需要这个
                    o.AddEventSources("System.Net.Sockets");
                })
            );

        // 链路追踪
        openTelemetryBuilder.WithTracing(tracerProviderBuilder =>
                tracerProviderBuilder.AddAspNetCoreInstrumentation(o =>
                    {
                        o.Filter = r => r.Request.Path != "/healthz";
                    })
                    .AddGrpcClientInstrumentation(o =>
                    {
                        // grpc基于http,如果为true,那么http就可以检测到grpc的traceid,产生一些activity.所以默认false关掉它.
                        o.SuppressDownstreamInstrumentation = false;
                    })
                    .AddHttpClientInstrumentation(o =>
                    {
                        o.FilterHttpRequestMessage = r => r.RequestUri?.PathAndQuery != "/healthz";
                    })
                    .AddQuartzInstrumentation()
                    .AddProcessor(new PyroscopeSpanProcessor())
            // 默认 ParentBased(root=AlwaysOn) 全部采集
            // .SetSampler(new TraceIdRatioBasedSampler(0.1))  // 采集1/10的指标
            // debug用
            // .AddConsoleExporter()
        );
    }

    // private static void AddLogging(WebApplicationBuilder builder)
    // {
    //     builder.Logging.AddOpenTelemetry(lo =>
    //     {
    //         // 包含attributes字段，里面有RequestUrl，ConnectionId等信息
    //         lo.IncludeScopes = true;
    //         // var resourceBuilder = ResourceBuilder.CreateDefault().AddService(AppName, serviceVersion: AppVersion);
    //         // lo.SetResourceBuilder(resourceBuilder);
    //         lo.AddOtlpExporter(options =>
    //         {
    //             options.Endpoint = new Uri(_ocEndpoint);
    //             options.Protocol = OtlpExportProtocol.Grpc;
    //         });
    //     });
    // }
}
