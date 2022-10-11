using Grpc.Core;
using GrpcGreeter;

namespace TestServer.Service;

/// <summary>
/// 演示greeter-GRPC
/// </summary>
public class GreeterService:Greeter.GreeterBase
{
    /// <summary>
    /// 响应hello请求
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        return Task.FromResult(new HelloReply
        {
            Message = $"Hello {request.Name} from GreeterService-Server"
        });
    }
}