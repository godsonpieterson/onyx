using System.Text.Json.Serialization;

namespace Products.Api.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ColourEnum
    {
        Red = 0,
        Blue = 1,
        Green = 2,
        Yellow = 3,
        White = 4,
        Black = 5
    }
}
