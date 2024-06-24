using TestServer.Tools;
using TestServer.Tools.Ip;
using TestServer.Tools.Ip.Ip2Region;
using TestServer.Tools.Ip.IpApi;

namespace TestServer.Service;

/// <summary>ip服务</summary>
public class IpService
{
    private readonly GlobalVar _globalVar;
    private readonly ILogger<IpService> _logger;
    private readonly List<string> ChinaStrings = new() { "中国", "CHINA", "china", "CN", "cn" };


    /// <summary>依赖注入</summary>
    /// <param name="logger"></param>
    /// <param name="globalVar"></param>
    public IpService(ILogger<IpService> logger, GlobalVar globalVar)
    {
        _logger = logger;
        _globalVar = globalVar;
    }

    /// <summary>获取ip信息</summary>
    /// <param name="ip"></param>
    /// <returns></returns>
    public async Task<IpServiceModel> GetIpInfo(string ip)
    {
        IpServiceModel result;
        if (DateTime.Now.Subtract(_globalVar.IpApiErrorTime) < TimeSpan.FromHours(1))
        {
            _logger.LogWarning($"从{_globalVar.IpApiErrorTime:yyyy-MM-dd HH:mm:ss}开始1小时内不再尝试请求ip-api.com");
            result = Ip2RegionTool.GetIpInfo(ip);
        }
        else
        {
            try
            {
                result = await IpApiTool.GetIpInfo(ip);
                _logger.LogInformation("使用ip-api的验证结果");
                return result;
            }
            catch (Exception e)
            {
                _globalVar.IpApiErrorTime = DateTime.Now;
                _logger.LogWarning("请求ip-api遇到网络问题:{EMessage}，改用ip2region", e.Message);
                result = Ip2RegionTool.GetIpInfo(ip);
            }
        }

        return result;
    }


    /// <summary>特定ip是否在国内</summary>
    /// <param name="ip"></param>
    /// <returns></returns>
    public async Task<bool> InChina(string ip)
    {
        var result = await GetIpInfo(ip);
        return ChinaStrings.Contains(result.Country) && result.Status == IpServiceQueryStatus.success;
    }
}
