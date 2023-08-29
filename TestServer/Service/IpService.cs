using TestServer.Tools.Ip;
using TestServer.Tools.Ip.Ip2Region;
using TestServer.Tools.Ip.IpApi;

namespace TestServer.Service;

/// <summary>
///     ip服务
/// </summary>
public class IpService
{
    private readonly ILogger<IpService> _logger;
    private readonly List<string> ChinaStrings = new() { "中国", "CHINA", "china", "CN", "cn" };


    /// <summary>
    ///     依赖注入
    /// </summary>
    /// <param name="logger"></param>
    public IpService(ILogger<IpService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    ///     获取ip信息
    /// </summary>
    /// <param name="ip"></param>
    /// <returns></returns>
    public async Task<IpServiceModel> GetIpInfo(string ip)
    {
        try
        {
            var result = await IpApiTool.GetIpInfo(ip);
            return result;
        }
        catch (HttpRequestException e)
        {
            _logger.LogWarning("请求ip-api遇到网络问题:{EMessage}，改用ip2region", e.Message);
            var result = Ip2RegionTool.GetIpInfo(ip);
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError("查询{IP}信息失败：{EMessage}", ip, e.Message);
            return new IpServiceModel { Status = IpServiceQueryStatus.fail, IP = ip };
        }
    }


    /// <summary>
    ///     特定ip是否在国内
    /// </summary>
    /// <param name="ip"></param>
    /// <returns></returns>
    public async Task<bool> InChina(string ip)
    {
        var result = await GetIpInfo(ip);
        return ChinaStrings.Contains(result.Country) && result.Status == IpServiceQueryStatus.success;
    }
}