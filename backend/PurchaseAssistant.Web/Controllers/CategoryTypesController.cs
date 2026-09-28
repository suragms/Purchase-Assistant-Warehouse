using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseAssistant.Application.DTOs.Catalog;
using PurchaseAssistant.Application.Interfaces;

namespace PurchaseAssistant.Web.Controllers
{
    [ApiController]
    [Route("api/v1/catalog/categories/{categoryId}/types")]
    [Authorize]
    public class CategoryTypesController : ControllerBase
    {
        private readonly ICategoryTypeService _typeService;

        public CategoryTypesController(ICategoryTypeService typeService)
        {
            _typeService = typeService;
        }

        [HttpGet]
        [Authorize(Policy = "RequireCatalogView")]
        public async Task<ActionResult<List<CategoryTypeDto>>> GetByCategoryId(Guid categoryId, CancellationToken cancellationToken = default)
        {
            return Ok(await _typeService.GetByCategoryIdAsync(categoryId, cancellationToken));
        }

        [HttpPost]
        [Authorize(Policy = "RequireCatalogCreate")]
        public async Task<ActionResult<CategoryTypeDto>> Create(Guid categoryId, [FromBody] string name, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _typeService.CreateAsync(categoryId, name, cancellationToken);
                return Created($"/api/v1/catalog/categories/{categoryId}/types/{result.Id}", result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex) when (ex.Message == "CATEGORY_TYPE_EXISTS")
            {
                return Conflict(new { error = "CATEGORY_TYPE_EXISTS" });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "RequireCatalogEdit")]
        public async Task<ActionResult<CategoryTypeDto>> Update(Guid categoryId, Guid id, [FromBody] string name, CancellationToken cancellationToken = default)
        {
            try
            {
                return Ok(await _typeService.UpdateAsync(id, name, cancellationToken));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireCatalogArchive")]
        public async Task<IActionResult> Delete(Guid categoryId, Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                await _typeService.DeleteAsync(id, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex) when (ex.Message == "CATEGORY_TYPE_IN_USE")
            {
                return Conflict(new { error = "CATEGORY_TYPE_IN_USE" });
            }
        }
    }
}
