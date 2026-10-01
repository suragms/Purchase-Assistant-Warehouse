using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseAssistant.Application.DTOs.Catalog;
using PurchaseAssistant.Application.Interfaces;

namespace PurchaseAssistant.Web.Controllers
{
    [ApiController]
    [Route("api/v1/catalog/search")]
    [Authorize]
    [Authorize(Policy = "RequireCatalogView")]
    public class SearchController : ControllerBase
    {
        private readonly IGlobalSearchService _searchService;
        private readonly IAuthorizationService _authorization;

        public SearchController(IGlobalSearchService searchService, IAuthorizationService authorization)
        {
            _searchService = searchService;
            _authorization = authorization;
        }

        [HttpGet]
        public async Task<ActionResult<GlobalSearchResponseDto>> Search([FromQuery] string q, CancellationToken cancellationToken = default)
        {
            var result = await _searchService.SearchAsync(q, cancellationToken);
            if (!(await _authorization.AuthorizeAsync(User, "RequireSupplierView")).Succeeded) result.Suppliers.Clear();
            if (!(await _authorization.AuthorizeAsync(User, "RequireBrokerView")).Succeeded) result.Brokers.Clear();
            return Ok(result);
        }
    }
}
