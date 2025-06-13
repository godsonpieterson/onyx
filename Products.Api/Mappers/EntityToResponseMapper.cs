using Products.Api.DataAccess.Entities;
using Products.Api.Models.Responses;

namespace Products.Api.Mappers
{
    public static class EntityToResponseMapper
    {
        public static ProductResponse ToProduct(ProductEntity productEntity)
        {
            return new ProductResponse()
            {
                Id = productEntity.Id,
                Name = productEntity.Name,
                Colour = productEntity.Colour
            };
        }

        public static IEnumerable<ProductResponse> ToProducts(IEnumerable<ProductEntity> productEntities)
        {
            return productEntities.Select(ToProduct).ToList();
        }
    }
}
