using CatalogService.Interfaces;
using CatalogService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{

    private readonly ILogger<ProductsController> _logger;
    private readonly IProductService _productService;

    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _logger = logger;
        _productService = productService;
    }

    /// <summary>
    /// Создать продукт
    /// </summary>
    /// <param name="request">Информация для создания продукта.</param>
    /// <returns></returns>
    /// <response code="201">Success</response>
    /// <response code="400">Bad Request (Ошибка входных данных)</response>
    /// <response code="500">Internal Server Error</response>
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateOrUpdateProductDto request)
    {
        _logger.LogInformation("Try create product with title: {title}", request.ProductTitle);
        var created = await _productService.CreateProductAsync(request);
        return CreatedAtAction(nameof(GetAsync), new { id = created.Id }, created);
    }

    /// <summary>
    /// Получить информацию по продукту
    /// </summary>
    /// <param name="id">Идентификатор продукта</param>
    /// <returns>Продукт</returns>
    /// <response code="200">Success</response>
    /// <response code="404">Not found</response>
    /// <response code="500">Internal Server Error</response>
    [HttpGet("{id:guid}")]
    [ActionName(nameof(GetAsync))]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        _logger.LogInformation("Try get product with id: {id}", id);
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null) { return NotFound(); }
        return Ok(product);
    }

    /// <summary>
    /// Получить все продукты
    /// </summary>
    /// <returns>Список продуктов</returns>
    /// <response code="200">Success</response>
    /// <response code="500">Internal Server Error</response>
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        _logger.LogInformation("Try get products");
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }

    /// <summary>
    /// Обновить продукт
    /// </summary>
    /// <param name="id">Идентификатор продукта</param>
    /// <param name="request">Информация для обновления.</param>
    /// <returns>Обновлённый продукт</returns>
    /// <response code="200">Success</response>
    /// <response code="404">Not Found</response>
    /// <response code="422">Bad Request (Ошибка входных данных)</response>
    /// <response code="500">Internal Server Error</response>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductDto>> UpdateAsync(Guid id, [FromBody] CreateOrUpdateProductDto request)
    {
        _logger.LogInformation("Try update product with id: {id}", id);
        var updated = await _productService.UpdateProductAsync(id, request);
        if (updated == null) { return NotFound(); }
        return Ok(updated);
    }

    /// <summary>
    /// Удалить продукт
    /// </summary>
    /// <param name="id">Идентификатор продукта</param>
    /// <response code="204">Success</response>
    /// <response code="404">Not Found</response>
    /// <response code="500">Internal Server Error</response>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        _logger.LogInformation("Try delete product with id: {id}", id);
        var ok = await _productService.DeleteProductAsync(id);
        if (!ok) { return NotFound(); }
        return NoContent();
    }
}

