using Microsoft.EntityFrameworkCore;
using Products.Api.DataAccess.Entities;

namespace Products.Api.DataAccess.Repositories
{
    public class ProductsDbContext(DbContextOptions<ProductsDbContext> options) : DbContext(options)
    {
        public DbSet<ProductEntity> Products { get; set; }
    }
}
