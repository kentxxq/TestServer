using System.Text.Json.Serialization;

namespace TestServer.Tools.Ip.IpApi;

/// <summary>ip-api的返回模型</summary>
public class IpApiModel
{
    /// <summary>查询的ip</summary>
    [JsonPropertyName("query")]
    public string Query { get; set; } = null!;

    /// <summary>查询状态</summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = null!;

    /// <summary>国家名称</summary>
    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;

    /// <summary>国家短码</summary>
    [JsonPropertyName("countryCode")]
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>地区短码</summary>
    [JsonPropertyName("region")]
    public string Region { get; set; } = string.Empty;

    /// <summary>地区名称</summary>
    [JsonPropertyName("regionName")]
    public string RegionName { get; set; } = string.Empty;

    /// <summary>城市</summary>
    [JsonPropertyName("city")]
    public string City { get; set; } = string.Empty;

    /// <summary>邮政编码</summary>
    [JsonPropertyName("zip")]
    public string Zip { get; set; } = string.Empty;

    /// <summary>维度</summary>
    [JsonPropertyName("lat")]
    public double Lat { get; set; } = 0;

    /// <summary>经度</summary>
    [JsonPropertyName("lon")]
    public double Lon { get; set; } = 0;

    /// <summary>时区</summary>
    [JsonPropertyName("timezone")]
    public string Timezone { get; set; } = string.Empty;

    /// <summary>运营商</summary>
    [JsonPropertyName("isp")]
    public string Isp { get; set; } = string.Empty;

    /// <summary>机构</summary>
    [JsonPropertyName("org")]
    public string Org { get; set; } = string.Empty;

    /// <summary>AS机构名称</summary>
    [JsonPropertyName("as")]
    public string As { get; set; } = string.Empty;
}
