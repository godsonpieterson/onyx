using Newtonsoft.Json;
using Products.Api.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Products.Api.Models.Responses
{
    public record ProductResponse
    {
        [JsonProperty(Required = Required.Always)]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [JsonProperty(Required = Required.Always)]
        public required string Name { get; set; }

        [JsonProperty(Required = Required.Always)]
        public required ColourEnum Colour { get; set; }
    }
}
