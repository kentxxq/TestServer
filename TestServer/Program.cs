using System.Reflection;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Sinks.SystemConsole.Themes;

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

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseAuthorization();

    var webSocketOptions = new WebSocketOptions
    {
        KeepAliveInterval = TimeSpan.FromSeconds(120),
    };
    app.UseWebSockets(webSocketOptions);

    app.MapControllers();

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


