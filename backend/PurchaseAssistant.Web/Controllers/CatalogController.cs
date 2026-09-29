using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseAssistant.Application.DTOs.Catalog;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Contracts.Responses;

namespace PurchaseAssistant.Web.Controllers
{
    [ApiController]
    [Route("api/v1/catalog/items")]
    [Authorize]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogService _catalogService;

        public CatalogController(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        [HttpGet]
        [Authorize(Policy = "RequireCatalogView")]
        public async Task<ActionResult<PaginatedResult<CatalogItemDto>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] string? search = null,
            [FromQuery] Guid? categoryId = null,
            CancellationToken cancellationToken = default)
        {
            return Ok(await _catalogService.GetAllAsync(page, pageSize, search, categoryId, cancellationToken));
        }

        [HttpGet("duplicates")]
        [Authorize(Policy = "RequireCatalogView")]
        public async Task<ActionResult<ApiResponse<List<DuplicateCandidateDto>>>> GetDuplicateCandidates([FromQuery] int? minSimilarity = 70)
        {
            var duplicates = await _catalogService.GetDuplicateCandidatesAsync(minSimilarity);
            return Ok(new ApiResponse<List<DuplicateCandidateDto>>(duplicates));
        }

        [HttpGet("by-barcode/{barcode}")]
        [Authorize(Policy = "RequireCatalogView")]
        public async Task<ActionResult<CatalogItemDto>> GetByBarcode(string barcode)
        {
            var item = await _catalogService.GetByBarcodeAsync(barcode);
            if (item == null)
            {
                return NotFound(new { error = "BARCODE_NOT_FOUND" });
            }
            return Ok(item);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "RequireCatalogView")]
        public async Task<ActionResult<CatalogItemDetailDto>> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            var item = await _catalogService.GetByIdAsync(id, cancellationToken);
            return item != null ? Ok(item) : NotFound();
        }

        [HttpPost]
        [Authorize(Policy = "RequireCatalogCreate")]
        public async Task<ActionResult<CatalogItemDto>> Create([FromBody] CatalogItemDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _catalogService.CreateAsync(dto, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex) when (ex.Message == "DUPLICATE_ITEM_CODE_OR_BARCODE")
            {
                return Conflict(new { error = "DUPLICATE_ITEM_CODE_OR_BARCODE" });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "RequireCatalogEdit")]
        public async Task<ActionResult<CatalogItemDto>> Update(Guid id, [FromBody] CatalogItemDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                return Ok(await _catalogService.UpdateAsync(id, dto, cancellationToken));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex) when (ex.Message == "CATALOG_ITEM_VERSION_CONFLICT")
            {
                return Conflict(new { error = "CATALOG_ITEM_VERSION_CONFLICT" });
            }
            catch (InvalidOperationException ex) when (ex.Message == "DUPLICATE_ITEM_CODE_OR_BARCODE")
            {
                return Conflict(new { error = "DUPLICATE_ITEM_CODE_OR_BARCODE" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireCatalogArchive")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                await _catalogService.DeleteAsync(id, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
