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
    /// ������� �������
    /// </summary>
    /// <param name="request">���������� ��� �������� ��������.</param>
    /// <returns></returns>
    /// <response code="201">Success</response>
    /// <response code="400">Bad Request (������ ������� ������)</response>
    /// <response code="500">Internal Server Error</response>
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateOrUpdateProductDto request)
    {
        _logger.LogInformation("Try create product with title: {title}", request.ProductTitle);
        var created = await _productService.CreateProductAsync(request);
        return CreatedAtAction(nameof(GetAsync), new { id = created.Id }, created);
    }

    /// <summary>
    /// �������� ���������� �� ��������
    /// </summary>
    /// <param name="id">������������� ��������</param>
    /// <returns>�������</returns>
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
    /// �������� ��� ��������
    /// </summary>
    /// <returns>������ ���������</returns>
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
    /// �������� �������
    /// </summary>
    /// <param name="id">������������� ��������</param>
    /// <param name="request">���������� ��� ����������.</param>
    /// <returns>���������� �������</returns>
    /// <response code="200">Success</response>
    /// <response code="404">Not Found</response>
    /// <response code="422">Bad Request (������ ������� ������)</response>
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
    /// ������� �������
    /// </summary>
    /// <param name="id">������������� ��������</param>
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

