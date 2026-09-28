using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseAssistant.Application.DTOs.Catalog;
using PurchaseAssistant.Domain.Constants;
using PurchaseAssistant.Infrastructure.Services;

namespace PurchaseAssistant.Web.Controllers
{
    [ApiController]
    [Route("api/v1/catalog/categories")]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [Authorize(Policy = "RequireCatalogView")] // Assuming policy mapping based on Phase 2
        public async Task<ActionResult<List<CategoryDto>>> GetAll()
        {
            return Ok(await _categoryService.GetAllAsync());
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "RequireCatalogView")]
        public async Task<ActionResult<CategoryDto>> GetById(Guid id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            return category != null ? Ok(category) : NotFound();
        }

        [HttpPost]
        [Authorize(Policy = "RequireCatalogCreate")]
        public async Task<ActionResult<CategoryDto>> Create(string name)
        {
            try
            {
                var result = await _categoryService.CreateAsync(name);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex) when (ex.Message == "CATEGORY_EXISTS")
            {
                return Conflict(new { error = "CATEGORY_EXISTS" });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "RequireCatalogEdit")]
        public async Task<ActionResult<CategoryDto>> Update(Guid id, string name)
        {
            try
            {
                return Ok(await _categoryService.UpdateAsync(id, name));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireCatalogArchive")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _categoryService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex) when (ex.Message == "CATEGORY_IN_USE")
            {
                return Conflict(new { error = "CATEGORY_IN_USE" });
            }
        }
    }
}
