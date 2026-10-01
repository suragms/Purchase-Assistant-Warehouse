using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseAssistant.Application.DTOs.Catalog;
using PurchaseAssistant.Application.DTOs.Purchase;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography;
using System.Text.Json;

namespace PurchaseAssistant.Web.Controllers
{
    [ApiController]
    [Route("api/v1/purchases")]
    public class PurchaseController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;
        private readonly ITimeLimitedDataProtector _previewProtector;
        private readonly IAuthorizationService _authorization;

        public PurchaseController(IPurchaseService purchaseService, ICurrentUserService currentUser, IDataProtectionProvider protection, IAuthorizationService authorization)
        {
            _purchaseService = purchaseService;
            _authorization = authorization;
            _previewProtector = protection.CreateProtector("PurchasePreview", currentUser.BusinessId?.ToString() ?? "", currentUser.UserId?.ToString() ?? "").ToTimeLimitedDataProtector();
        }

        private static string Fingerprint(UpsertPurchaseOrderDto dto)
        {
            var input = JsonSerializer.SerializeToNode(dto)!;
            input.AsObject().Remove(nameof(UpsertPurchaseOrderDto.PreviewToken));
            return Convert.ToHexString(SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(input)));
        }

        private bool HasValidPreview(UpsertPurchaseOrderDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.PreviewToken)) return false;
            try { return _previewProtector.Unprotect(dto.PreviewToken, out _) == Fingerprint(dto); }
            catch (CryptographicException) { return false; }
        }

        [HttpPost("preview")]
        [Authorize(Policy = "RequirePurchaseCreate")]
        public async Task<ActionResult<PurchasePreviewDto>> Preview([FromBody] UpsertPurchaseOrderDto dto)
        {
            try
            {
                var result = await _purchaseService.PreviewAsync(dto);
                result.PreviewToken = _previewProtector.Protect(Fingerprint(dto), TimeSpan.FromMinutes(20));
                return Ok(result);
            }
            catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
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
            if (!HasValidPreview(dto)) return Conflict(new { error = "Preview these purchase values before saving. The preview may have expired or the form changed." });
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
            if (!HasValidPreview(dto)) return Conflict(new { error = "Preview these purchase values before saving. The preview may have expired or the form changed." });
            try
            {
                if (!dto.ExpectedVersion.HasValue) return BadRequest(new { error = "Refresh this purchase before editing; expectedVersion is required." });
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
        [Authorize(Policy = "RequirePurchaseView")]
        public async Task<ActionResult<PurchaseOrderDto>> UpdateStatus(Guid id, [FromBody] UpdateStatusDto dto)
        {
            try
            {
                var policy = dto.Status switch {
                    PurchaseStatus.Verified => "RequirePurchaseVerify",
                    PurchaseStatus.Dispatched or PurchaseStatus.Arrived => "RequirePurchaseDelivery",
                    _ => "RequirePurchaseEdit"
                };
                if (!(await _authorization.AuthorizeAsync(User, policy)).Succeeded) return Forbid();
                if (!dto.ExpectedVersion.HasValue) return BadRequest(new { error = "Refresh this purchase before changing its status; expectedVersion is required." });
                var result = await _purchaseService.UpdateStatusAsync(id, dto.Status, dto.ExpectedVersion);
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

        [HttpGet("{id}/activity")]
        [Authorize(Policy = "RequirePurchaseView")]
        public async Task<ActionResult<List<PurchaseActivityDto>>> GetActivity(Guid id)
        {
            try { return Ok(await _purchaseService.GetActivityAsync(id)); }
            catch (KeyNotFoundException) { return NotFound(new { error = "PURCHASE_ORDER_NOT_FOUND" }); }
        }

        [HttpPatch("{id}/payment")]
        [Authorize(Policy = "RequirePurchaseEdit", Roles = "Owner")]
        public async Task<ActionResult<PurchaseOrderDto>> UpdatePayment(Guid id, UpdatePurchasePaymentDto dto)
        {
            if (!dto.ExpectedVersion.HasValue) return BadRequest(new { error = "Refresh this purchase before recording payment; expectedVersion is required." });
            try { return Ok(await _purchaseService.UpdatePaymentAsync(id, dto)); }
            catch (KeyNotFoundException) { return NotFound(new { error = "Purchase order not found." }); }
            catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
        }

        [HttpPost("{id}/receive")]
        [Authorize(Policy = "RequirePurchaseVerify")]
        [Authorize(Policy = "RequirePurchaseCommit")]
        public async Task<ActionResult<PurchaseOrderDto>> ReceiveItems(Guid id, [FromBody] ReceivePurchaseDto dto)
        {
            try
            {
                if (!dto.ExpectedVersion.HasValue) return BadRequest(new { error = "Refresh this purchase before receiving; expectedVersion is required." });
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
        public uint? ExpectedVersion { get; set; }
    }
}
