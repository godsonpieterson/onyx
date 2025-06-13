using Products.Api.DataAccess.Entities;
using Products.Api.Models.Responses;

namespace Products.Api.Mappers
{
    public static class ResponseToEntityMapper
    {
        public static ProductEntity ToProductEntity(ProductResponse product)
        {
            return new ProductEntity()
            {
                Id = product.Id,
                Name = product.Name,
                Colour = product.Colour
            };
        }

        public static IEnumerable<ProductEntity> ToProductEntities(IEnumerable<ProductResponse> products)
        {
            return products.Select(ToProductEntity).ToList();
        }
    }
}
