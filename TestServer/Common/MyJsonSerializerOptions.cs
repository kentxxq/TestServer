using System.Text.Encodings.Web;
using System.Text.Json;

namespace TestServer.Common;

public static class MyJsonSerializerOptions
{
    public static JsonSerializerOptions Default = new JsonSerializerOptions
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
}
