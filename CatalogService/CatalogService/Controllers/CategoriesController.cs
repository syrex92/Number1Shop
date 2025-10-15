using CatalogService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CatalogService.Controllers;

/// <summary>
/// ��������� ���������
/// </summary>
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
    /// �������� �������� �� ���������
    /// </summary>
    /// <param name="id">������������� ���������</param>
    /// <param name="page">����� ��������</param>
    /// <param name="pageSize">������ ��������</param>
    /// <returns>�������� ���������</returns>
    /// <response code="200">Success</response>
    /// <response code="500">Internal Server Error</response>
    [HttpGet("{id:guid}")]
    [ActionName(nameof(GetAsync))]
    public async Task<IActionResult> GetAsync(Guid id, int? page = null, int? pageSize = null)
    {
        _logger.LogInformation("Try get products by category with id: {id}", id);
        var product = await _categoriesService.GetProductsByCategoryIdAsync(id, page, pageSize);

        if (product == null)
        {
            return NotFound("Category with id not found");
        }

        return Ok(product);
    }

    /// <summary>
    /// �������� ��� ���������
    /// </summary>
    /// <returns>������ ��������� � ����������</returns>
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

