using Bogus;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using Products.Api.DataAccess.Entities;
using Products.Api.IntegrationTests.Configuration;
using Products.Api.Mappers;
using Products.Api.Models.Enums;
using Products.Api.Models.Responses;
using System.Net;
using System.Net.Http.Json;

namespace Products.Api.IntegrationTests.Controllers
{
    public class ProductsControllerTests : IDisposable
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public ProductsControllerTests()
        {
            _factory = new CustomWebApplicationFactory();
            _client = _factory.CreateClient();            
        }

        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [Fact]
        public async Task Get_ReturnsAllProducts()
        {
            var mockProducts = GenerateMockProducts();
            _factory.MockProductsRepository.Setup(m => m.GetAsync()).ReturnsAsync([.. mockProducts]).Verifiable();

            var response = await _client.GetAsync("api/products");

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = JsonConvert.DeserializeObject<IEnumerable<ProductResponse>>(await response.Content.ReadAsStringAsync());

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(mockProducts);
            _factory.MockProductsRepository.VerifyAll();
        }

        [Fact]
        public async Task Get_ReturnsEmptyListWhenThereAreNoProducts()
        {
            var mockProducts = new List<ProductEntity>();
            _factory.MockProductsRepository.Setup(m => m.GetAsync()).ReturnsAsync([.. mockProducts]).Verifiable();

            var response = await _client.GetAsync("api/products");

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = JsonConvert.DeserializeObject<IEnumerable<ProductResponse>>(await response.Content.ReadAsStringAsync());

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(mockProducts);
            _factory.MockProductsRepository.VerifyAll();
        }

        [Fact]
        public async Task Get_ReturnsProductById()
        {
            var mockProducts = GenerateMockProducts();
            var mockProduct = mockProducts.First();

            _factory.MockProductsRepository.Setup(m => m.GetAsync(mockProduct.Id)).ReturnsAsync(mockProduct).Verifiable();

            var response = await _client.GetAsync($"api/products/{mockProduct.Id}");

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = JsonConvert.DeserializeObject<ProductResponse>(await response.Content.ReadAsStringAsync());

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(mockProduct);
            _factory.MockProductsRepository.VerifyAll();
        }

        [Fact]
        public async Task Get_ReturnsNotFoundWhenProductDoesNotExist()
        {
            var mockProducts = GenerateMockProducts();

            _factory.MockProductsRepository.Setup(m => m.GetAsync(-1)).ReturnsAsync(default(ProductEntity)).Verifiable();

            var response = await _client.GetAsync($"api/products/-1");

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            _factory.MockProductsRepository.VerifyAll();
        }

        [Fact]
        public async Task Get_ReturnsProductsByColour()
        {
            var colour = new Faker().PickRandom<ColourEnum>();

            var mockProducts = GenerateMockProducts(colour);

            _factory.MockProductsRepository.Setup(m => m.GetAsync(colour)).ReturnsAsync([.. mockProducts]).Verifiable();

            var response = await _client.GetAsync($"api/products/{colour}");

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = JsonConvert.DeserializeObject<IEnumerable<ProductResponse>>(await response.Content.ReadAsStringAsync());

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(mockProducts);
            _factory.MockProductsRepository.VerifyAll();
        }

        [Fact]
        public async Task Put_AddsProduct()
        {
            var mockProducts = GenerateMockProducts();
            var mockProduct = mockProducts.First();

            _factory.MockProductsRepository.Setup(m => m.AddAsync(It.IsAny<ProductEntity>())).ReturnsAsync(1).Verifiable();

            var response = await _client.PutAsync("api/products/", JsonContent.Create(EntityToResponseMapper.ToProduct(mockProduct)));

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var result = JsonConvert.DeserializeObject<ProductResponse>(await response.Content.ReadAsStringAsync());

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(mockProduct);
            _factory.MockProductsRepository.VerifyAll();
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
