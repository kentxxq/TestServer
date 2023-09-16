using System.Text.Encodings.Web;
using System.Text.Json;

namespace TestServer.Tools;

/// <summary>静态数据</summary>
public static class StaticData
{
    /// <summary>友好打印</summary>
    public static readonly JsonSerializerOptions PrettyPrintJsonSerializerOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
}
