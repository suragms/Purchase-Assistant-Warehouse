using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseAssistant.Application.DTOs.Catalog;
using PurchaseAssistant.Application.DTOs.Purchase;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PurchaseAssistant.Web.Controllers
{
    [ApiController]
    [Route("api/v1/purchases")]
    public class PurchaseController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;

        public PurchaseController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        [HttpGet]
        [Authorize(Policy = "RequirePurchaseView")]
        public async Task<ActionResult<PaginatedResult<PurchaseOrderDto>>> GetPurchaseOrders(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] string? search = null,
            [FromQuery] PurchaseStatus? status = null,
            [FromQuery] Guid? supplierId = null)
        {
            var result = await _purchaseService.GetPurchaseOrdersAsync(page, pageSize, search, status, supplierId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "RequirePurchaseView")]
        public async Task<ActionResult<PurchaseOrderDto>> GetPurchaseOrder(Guid id)
        {
            try
            {
                var result = await _purchaseService.GetPurchaseOrderByIdAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { error = "PURCHASE_ORDER_NOT_FOUND" });
            }
        }

        [HttpPost]
        [Authorize(Policy = "RequirePurchaseCreate")]
        public async Task<ActionResult<PurchaseOrderDto>> CreatePurchaseOrder([FromBody] UpsertPurchaseOrderDto dto)
        {
            try
            {
                var result = await _purchaseService.CreatePurchaseOrderAsync(dto);
                return CreatedAtAction(nameof(GetPurchaseOrder), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "RequirePurchaseEdit")]
        public async Task<ActionResult<PurchaseOrderDto>> UpdatePurchaseOrder(Guid id, [FromBody] UpsertPurchaseOrderDto dto)
        {
            try
            {
                var result = await _purchaseService.UpdatePurchaseOrderAsync(id, dto);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { error = "PURCHASE_ORDER_NOT_FOUND" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "RequirePurchaseDelete")]
        public async Task<IActionResult> DeletePurchaseOrder(Guid id)
        {
            try
            {
                await _purchaseService.DeletePurchaseOrderAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { error = "PURCHASE_ORDER_NOT_FOUND" });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        [HttpPost("{id}/status")]
        [Authorize(Policy = "RequirePurchaseEdit")]
        public async Task<ActionResult<PurchaseOrderDto>> UpdateStatus(Guid id, [FromBody] UpdateStatusDto dto)
        {
            try
            {
                var result = await _purchaseService.UpdateStatusAsync(id, dto.Status);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { error = "PURCHASE_ORDER_NOT_FOUND" });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        [HttpPost("{id}/receive")]
        [Authorize(Policy = "RequirePurchaseVerify")]
        public async Task<ActionResult<PurchaseOrderDto>> ReceiveItems(Guid id, [FromBody] ReceivePurchaseDto dto)
        {
            try
            {
                var result = await _purchaseService.ReceiveItemsAsync(id, dto);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { error = "PURCHASE_ORDER_NOT_FOUND" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }
    }

    public class UpdateStatusDto
    {
        public PurchaseStatus Status { get; set; }
    }
}
