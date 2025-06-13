using Products.Api.DataAccess.Entities;
using Products.Api.Models.Enums;

namespace Products.Api.DataAccess.Repositories
{
    public interface IProductsRepository
    {
        Task<List<ProductEntity>> GetAsync();

        Task<ProductEntity?> GetAsync(int id);

        Task<List<ProductEntity>> GetAsync(ColourEnum colour);

        Task<int> AddAsync(ProductEntity productEntity);
    }
}
