using Products.Api.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Products.Api.DataAccess.Entities
{
    public class ProductEntity
    {
        [Key]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        public required string Name { get; set; }

        public ColourEnum Colour { get; set; }
    }
}
