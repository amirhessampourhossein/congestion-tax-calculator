using Newtonsoft.Json;

namespace CongestionTaxCalculator.Services.Json;

public class TimeOnlyJsonConverter : JsonConverter<TimeOnly>
{
    private const string TimeFormat = "HH:mm";

    public override TimeOnly ReadJson(
        JsonReader reader,
        Type objectType,
        TimeOnly existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        return TimeOnly.ParseExact(reader.Value?.ToString()!, TimeFormat);
    }

    public override void WriteJson(
        JsonWriter writer,
        TimeOnly value,
        JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString(TimeFormat));
    }
}
