using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Products.Api.Controllers;
using Products.Api.DataAccess.Entities;
using Products.Api.DataAccess.Repositories;
using Products.Api.Mappers;
using Products.Api.Models.Enums;

namespace Projects.Api.UnitTests.Controllers
{
    public class ProductsControllerTests
    {
        private readonly Mock<ILogger<ProductsController>> _mockLogger;
        private readonly Mock<IProductsRepository> _mockProductsRepository;
        private readonly ProductsController _productsController;

        public ProductsControllerTests()
        {
            _mockLogger = new Mock<ILogger<ProductsController>>();
            _mockProductsRepository = new Mock<IProductsRepository>();
            _productsController = new ProductsController(_mockLogger.Object, _mockProductsRepository.Object);
        }

        [Fact]
        public async Task Get_ReturnsAllProducts()
        {
            var mockProducts = GenerateMockProducts();
            _mockProductsRepository.Setup(m => m.GetAsync()).ReturnsAsync([.. mockProducts]).Verifiable();

            var result = (await _productsController.GetAsync()).Result as OkObjectResult;

            result.Should().NotBeNull();
            result.Value.Should().NotBeNull();
            result.Value.Should().BeEquivalentTo(mockProducts);
            _mockProductsRepository.VerifyAll();
        }

        [Fact]
        public async Task Get_ReturnsEmptyListWhenThereAreNoProducts()
        {
            var mockProducts = new List<ProductEntity>();
            _mockProductsRepository.Setup(m => m.GetAsync()).ReturnsAsync([.. mockProducts]).Verifiable();

            var result = (await _productsController.GetAsync()).Result as OkObjectResult;

            result.Should().NotBeNull();
            result.Value.Should().NotBeNull();
            result.Value.Should().BeEquivalentTo(mockProducts);
            _mockProductsRepository.VerifyAll();
        }

        [Fact]
        public async Task Get_ReturnsProductById()
        {
            var mockProducts = GenerateMockProducts();
            var mockProduct = mockProducts.First();

            _mockProductsRepository.Setup(m => m.GetAsync(mockProduct.Id)).ReturnsAsync(mockProduct).Verifiable();

            var result = (await _productsController.GetAsync(mockProduct.Id)).Result as OkObjectResult;

            result.Should().NotBeNull();
            result.Value.Should().NotBeNull();
            result.Value.Should().BeEquivalentTo(mockProduct);
            _mockProductsRepository.VerifyAll();
        }

        [Fact]
        public async Task Get_ReturnsNotFoundWhenProductDoesNotExist()
        {
            var mockProducts = GenerateMockProducts();

            _mockProductsRepository.Setup(m => m.GetAsync(-1)).ReturnsAsync(default(ProductEntity)).Verifiable();

            var result = (await _productsController.GetAsync(-1)).Result as NotFoundResult;

            result.Should().NotBeNull();
            _mockProductsRepository.VerifyAll();
        }

        [Fact]
        public async Task Get_ReturnsProductsByColour()
        {
            var colour = new Faker().PickRandom<ColourEnum>();

            var mockProducts = GenerateMockProducts(colour);

            _mockProductsRepository.Setup(m => m.GetAsync(colour)).ReturnsAsync([.. mockProducts]).Verifiable() ;

            var result = (await _productsController.GetAsync(colour)).Result as OkObjectResult;

            result.Should().NotBeNull();
            result.Value.Should().NotBeNull();
            result.Value.Should().BeEquivalentTo(mockProducts);
            _mockProductsRepository.VerifyAll();
        }

        [Fact]
        public async Task Put_AddsProduct()
        {
            var mockProducts = GenerateMockProducts();
            var mockProduct = mockProducts.First();

            _mockProductsRepository.Setup(m => m.AddAsync(It.IsAny<ProductEntity>())).ReturnsAsync(1).Verifiable();

            var result = (await _productsController.PutAsync(EntityToResponseMapper.ToProduct(mockProduct))) as CreatedAtRouteResult;

            result.Should().NotBeNull();
            result.Value.Should().NotBeNull();
            result.Value.Should().BeEquivalentTo(mockProduct);
            _mockProductsRepository.VerifyAll();
        }

        private static List<ProductEntity> GenerateMockProducts(ColourEnum? colour = null)
        {
            return new Faker<ProductEntity>()
                .RuleFor(p => p.Id, f => f.Random.Int(min: 0))
                .RuleFor(p => p.Name, f => f.Random.Word())
                .RuleFor(p => p.Colour, f => colour ?? f.PickRandom<ColourEnum>())
                .Generate(5);
        }
    }
}
