using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseAssistant.Application.DTOs.Catalog;
using PurchaseAssistant.Application.Interfaces;

namespace PurchaseAssistant.Web.Controllers
{
    [ApiController]
    [Route("api/v1/catalog/search")]
    [Authorize]
    public class SearchController : ControllerBase
    {
        private readonly IGlobalSearchService _searchService;

        public SearchController(IGlobalSearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet]
        public async Task<ActionResult<GlobalSearchResponseDto>> Search([FromQuery] string q, CancellationToken cancellationToken = default)
        {
            return Ok(await _searchService.SearchAsync(q, cancellationToken));
        }
    }
}
