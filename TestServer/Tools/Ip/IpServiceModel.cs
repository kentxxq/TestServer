using System.Text.Json.Serialization;
using TestServer.Service;

namespace TestServer.Tools.Ip;

/// <summary>
///     ip服务模型
/// </summary>
public class IpServiceModel
{
    /// <summary>
    ///     查询状态
    /// </summary>
    [JsonPropertyName("status")]
    public IpServiceQueryStatus Status { get; set; }

    /// <summary>
    ///     ip
    /// </summary>
    [JsonPropertyName("ip")]
    public string IP { get; set; } = null!;

    /// <summary>
    ///     国家名称
    /// </summary>
    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;

    /// <summary>
    ///     地区名称
    /// </summary>
    [JsonPropertyName("regionName")]
    public string RegionName { get; set; } = string.Empty;

    /// <summary>
    ///     城市
    /// </summary>
    [JsonPropertyName("city")]
    public string City { get; set; } = string.Empty;

    /// <summary>
    ///     运营商
    /// </summary>
    [JsonPropertyName("isp")]
    public string Isp { get; set; } = string.Empty;
}