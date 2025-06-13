using Microsoft.EntityFrameworkCore;
using Products.Api.DataAccess.Entities;
using Products.Api.Models.Enums;

namespace Products.Api.DataAccess.Repositories
{
    public class ProductsRepository(ProductsDbContext dbContext) : IProductsRepository
    {
        private readonly ProductsDbContext _dbContext = dbContext;

        public async Task<List<ProductEntity>> GetAsync()
        {
            return await _dbContext.Products.ToListAsync();
        }

        public async Task<ProductEntity?> GetAsync(int id)
        {
            return await _dbContext.Products.Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<ProductEntity>> GetAsync(ColourEnum colour)
        {
            return await _dbContext.Products.Where(p => p.Colour == colour).ToListAsync();
        }

        public async Task<int> AddAsync(ProductEntity product)
        {
            await _dbContext.Products.AddAsync(product);
            return await _dbContext.SaveChangesAsync();
        }
    }
}
