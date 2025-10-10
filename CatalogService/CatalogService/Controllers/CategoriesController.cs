using CatalogService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ILogger<CategoriesController> _logger;
    private readonly ICategoriesService _categoriesService;

    public CategoriesController(ICategoriesService categoriesService, ILogger<CategoriesController> logger)
    {
        _logger = logger;
        _categoriesService = categoriesService;
    }

    /// <summary>
    /// Получить продукты по категории
    /// </summary>
    /// <param name="id">Идентификатор категории</param>
    /// <returns>Продукты категории</returns>
    /// <response code="200">Success</response>
    /// <response code="500">Internal Server Error</response>
    [HttpGet("{id:guid}")]
    [ActionName(nameof(GetAsync))]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        _logger.LogInformation("Try get products by category with id: {id}", id);
        var product = await _categoriesService.GetProductsByCategoryIdAsync(id);

        if (product == null)
        {
            return NotFound("Category with id not found");
        }

        return Ok(product);
    }

    /// <summary>
    /// Получить все категории
    /// </summary>
    /// <returns>Список категорий с продуктами</returns>
    /// <response code="200">Success</response>
    /// <response code="500">Internal Server Error</response>
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        _logger.LogInformation("Try get categories");
        var products = await _categoriesService.GetAllCategoriesAsync();
        return Ok(products);
    }
}

