using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;

namespace TestServer.Controllers;

/// <summary>websocket控制器</summary>
[ApiExplorerSettings(GroupName = "V1")]
[ApiController]
[Route("[controller]/[action]")]
public class WebSocketController : ControllerBase
{
    private readonly ILogger<WebSocketController> _logger;

    /// <inheritdoc />
    public WebSocketController(ILogger<WebSocketController> logger)
    {
        _logger = logger;
    }

    /// <summary>连接websocket</summary>
    [HttpGet]
    public async Task<string> Echo()
    {
        _logger.LogInformation("请求到了ws");
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var websocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            try
            {
                await EchoData(websocket);
            }
            catch (Exception e)
            {
                _logger.LogWarning("连接断开:{Reason}", e.Message);
            }
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }

        return "请求完成";
    }

    private static async Task EchoData(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        while (!result.CloseStatus.HasValue)
        {
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, false,
                CancellationToken.None);
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
    }
}
