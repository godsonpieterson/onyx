using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Products.Api.DataAccess.Repositories;
using Products.Api.Mappers;
using Products.Api.Models.Enums;
using Products.Api.Models.Responses;

namespace Products.Api.Controllers
{
    [Route("api/products")]
    [ApiController]
    [Authorize]
    public class ProductsController(ILogger<ProductsController> logger, IProductsRepository productsRepository) : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger = logger;
        private readonly IProductsRepository _productsRepository = productsRepository;

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProductResponse>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<ProductResponse>>> GetAsync()
        {
            _logger.LogInformation("Getting all products");

            var productEntities = await _productsRepository.GetAsync();

            return Ok(EntityToResponseMapper.ToProducts(productEntities));
        }

        [HttpGet]
        [Route("{colour}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProductResponse>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<ProductResponse>>> GetAsync(ColourEnum colour)
        {
            _logger.LogInformation("Getting products by colour: {colour}", colour);

            var productEntities = await _productsRepository.GetAsync(colour);

            return Ok(EntityToResponseMapper.ToProducts(productEntities));
        }

        [HttpGet]
        [Route("{id:int}", Name = nameof(GetAsync))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ProductResponse>> GetAsync(int id)
        {
            _logger.LogInformation("Getting product by id: {id}", id);

            var productEntity = await _productsRepository.GetAsync(id);

            return (productEntity == null)
                ? NotFound()
                : Ok(EntityToResponseMapper.ToProduct(productEntity));
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> PutAsync(ProductResponse product)
        {
            _logger.LogInformation("Creating product: {product}", product);

            var productEntity = ResponseToEntityMapper.ToProductEntity(product);
            await _productsRepository.AddAsync(productEntity);

            return new CreatedAtRouteResult(nameof(GetAsync), new { product.Id }, product);
        }
    }
}
