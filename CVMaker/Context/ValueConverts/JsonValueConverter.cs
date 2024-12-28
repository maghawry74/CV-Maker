using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CVMaker.Context.ValueConverts;
public class JsonValueConverter<T> : ValueConverter<T, string>
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };
    public JsonValueConverter() : base(
        v => JsonSerializer.Serialize(v,Options),
        v => JsonSerializer.Deserialize<T>(v, Options)!)
    { }
}
