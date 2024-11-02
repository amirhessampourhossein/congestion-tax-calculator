using Newtonsoft.Json;

namespace CongestionTaxCalculator.Services.Json;

public static class JsonService
{
    private static readonly JsonSerializerSettings _settings;

    static JsonService()
    {
        _settings = new();
        _settings.Converters.Add(new TimeOnlyJsonConverter());
    }

    public static string Serialize<T>(T obj)
    {
        var json = JsonConvert.SerializeObject(obj, _settings);

        return json;
    }

    public static T? Deserialize<T>(string json)
    {
        var result = JsonConvert.DeserializeObject<T>(json, _settings);

        return result;
    }
}
