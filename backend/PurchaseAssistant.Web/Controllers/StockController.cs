using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseAssistant.Application.DTOs.Catalog;
using PurchaseAssistant.Application.DTOs.Stock;
using PurchaseAssistant.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace PurchaseAssistant.Web.Controllers
{
    [ApiController]
    [Route("api/v1/stock")]
    public class StockController : ControllerBase
    {
        private readonly IStockService _stockService;

        public StockController(IStockService stockService)
        {
            _stockService = stockService;
        }

        [HttpGet]
        [Authorize(Policy = "RequireStockView")]
        public async Task<ActionResult<PaginatedResult<StockItemDto>>> GetStockItems(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] string? search = null)
        {
            var result = await _stockService.GetStockItemsAsync(page, pageSize, search, false, false);
            return Ok(result);
        }

        [HttpGet("low-stock")]
        [Authorize(Policy = "RequireStockView")]
        public async Task<ActionResult<PaginatedResult<StockItemDto>>> GetLowStockItems(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] string? search = null)
        {
            var result = await _stockService.GetStockItemsAsync(page, pageSize, search, true, false);
            return Ok(result);
        }

        [HttpGet("out-of-stock")]
        [Authorize(Policy = "RequireStockView")]
        public async Task<ActionResult<PaginatedResult<StockItemDto>>> GetOutOfStockItems(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] string? search = null)
        {
            var result = await _stockService.GetStockItemsAsync(page, pageSize, search, false, true);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "RequireStockView")]
        public async Task<ActionResult<StockItemDto>> GetStockDetail(Guid id)
        {
            try
            {
                var result = await _stockService.GetStockDetailAsync(id);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { error = "STOCK_ITEM_NOT_FOUND" });
            }
        }

        [HttpGet("{id}/activity")]
        [Authorize(Policy = "RequireStockView")]
        public async Task<ActionResult<PaginatedResult<StockMovementDto>>> GetItemActivity(
            Guid id,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                var result = await _stockService.GetItemActivityAsync(id, page, pageSize);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { error = "STOCK_ITEM_NOT_FOUND" });
            }
        }

        [HttpPost("{id}/adjust")]
        [Authorize(Policy = "RequireStockAdjust")]
        public async Task<ActionResult<StockItemDto>> AdjustStock(Guid id, [FromBody] AdjustStockRequestDto request)
        {
            try
            {
                var result = await _stockService.AdjustStockAsync(id, request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { error = "STOCK_ITEM_NOT_FOUND" });
            }
            catch (InvalidOperationException ex) when (ex.Message == "STOCK_VERSION_CONFLICT")
            {
                return Conflict(new { error = "STOCK_VERSION_CONFLICT" });
            }
            catch (InvalidOperationException ex) when (ex.Message == "INSUFFICIENT_STOCK")
            {
                return Conflict(new { error = "INSUFFICIENT_STOCK" });
            }
        }

        [HttpPost("{id}/physical")]
        [Authorize(Policy = "RequireStockPhysical")]
        public async Task<ActionResult<StockItemDto>> UpdatePhysicalStock(Guid id, [FromBody] UpdatePhysicalStockRequestDto request)
        {
            try
            {
                var result = await _stockService.UpdatePhysicalStockAsync(id, request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { error = "STOCK_ITEM_NOT_FOUND" });
            }
            catch (InvalidOperationException ex) when (ex.Message == "STOCK_VERSION_CONFLICT")
            {
                return Conflict(new { error = "STOCK_VERSION_CONFLICT" });
            }
        }

        [HttpPost("{id}/reconcile")]
        [Authorize(Policy = "RequireStockSystem")]
        public async Task<ActionResult<StockItemDto>> ReconcileStock(Guid id, [FromBody] ReconcileStockRequestDto request)
        {
            try
            {
                var result = await _stockService.ReconcileStockAsync(id, request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { error = "STOCK_ITEM_NOT_FOUND" });
            }
            catch (InvalidOperationException ex) when (ex.Message == "STOCK_VERSION_CONFLICT")
            {
                return Conflict(new { error = "STOCK_VERSION_CONFLICT" });
            }
            catch (InvalidOperationException ex) when (ex.Message == "RECONCILE_NO_VARIANCE")
            {
                return Conflict(new { error = "RECONCILE_NO_VARIANCE" });
            }
            catch (InvalidOperationException ex) when (ex.Message == "INSUFFICIENT_STOCK")
            {
                return Conflict(new { error = "INSUFFICIENT_STOCK" });
            }
        }
    }
}
