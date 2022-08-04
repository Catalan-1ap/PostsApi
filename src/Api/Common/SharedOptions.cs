using System.Text.Json;
using System.Text.Json.Serialization;


namespace Api.Common;


public static class SharedOptions
{
    public static readonly Action<JsonSerializerOptions> SerializerOptions = x =>
    {
        x.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        x.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        x.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    };
}