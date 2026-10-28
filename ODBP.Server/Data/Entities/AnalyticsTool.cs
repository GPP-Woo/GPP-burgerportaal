using System.Text.Json.Serialization;

namespace ODBP.Data.Entities
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AnalyticsTool
    {
        None,
        Matomo
    }
}
