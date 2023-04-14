using IP2Region.Net.XDB;
using TestServer.Tools.Ip;
using TestServer.Tools.Ip.IpApi;

namespace TestServer.Service;

/// <summary>
/// ip服务
/// </summary>
public class IpService
{
    private readonly ISearcher _searcher;
    private readonly ILogger<IpService> _logger;
    private static readonly IpServiceModel QueryFailed = new IpServiceModel { Status = "failed" };

    /// <summary>
    /// 依赖注入
    /// </summary>
    /// <param name="searcher"></param>
    /// <param name="logger"></param>
    public IpService(ISearcher searcher, ILogger<IpService> logger)
    {
        _searcher = searcher;
        _logger = logger;
    }

    /// <summary>
    /// 获取ip信息
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
            var data = _searcher.Search(ip);
            if (string.IsNullOrEmpty(data))
            {
                _logger.LogWarning("ip2region没有查询到{IP}的信息", ip);
                return QueryFailed;
            }
            var dataList = data.Split("|");
            var result = new IpServiceModel
            {
                Status = "success",
                Country = dataList[0],
                RegionName = dataList[2],
                Isp = dataList[4],
                City = dataList[3]
            };
            return result;
        }
        catch(Exception e)
        {
            _logger.LogError("查询{IP}信息失败：{EMessage}", ip, e.Message);
            return QueryFailed;
        }
    }
}