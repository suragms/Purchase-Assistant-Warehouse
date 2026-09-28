using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseAssistant.Application.DTOs.Catalog;
using PurchaseAssistant.Application.Interfaces;

namespace PurchaseAssistant.Web.Controllers
{
    [ApiController]
    [Route("api/v1/catalog/suppliers")]
    [Authorize]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SuppliersController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet]
        [Authorize(Policy = "RequireSupplierView")]
        public async Task<ActionResult<List<SupplierDto>>> GetAll(CancellationToken cancellationToken = default)
        {
            return Ok(await _supplierService.GetAllAsync(cancellationToken));
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "RequireSupplierView")]
        public async Task<ActionResult<SupplierDto>> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            var supplier = await _supplierService.GetByIdAsync(id, cancellationToken);
            return supplier != null ? Ok(supplier) : NotFound();
        }

        [HttpPost]
        [Authorize(Policy = "RequireSupplierCreate")]
        public async Task<ActionResult<SupplierDto>> Create([FromBody] SupplierDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _supplierService.CreateAsync(dto, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex) when (ex.Message == "SUPPLIER_EXISTS")
            {
                return Conflict(new { error = "SUPPLIER_EXISTS" });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "RequireSupplierEdit")]
        public async Task<ActionResult<SupplierDto>> Update(Guid id, [FromBody] SupplierDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                return Ok(await _supplierService.UpdateAsync(id, dto, cancellationToken));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireSupplierDelete")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                await _supplierService.DeleteAsync(id, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex) when (ex.Message == "SUPPLIER_IN_USE")
            {
                return Conflict(new { error = "SUPPLIER_IN_USE" });
            }
        }
    }
}
