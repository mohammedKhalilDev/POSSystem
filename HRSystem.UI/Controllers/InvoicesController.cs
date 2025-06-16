using Microsoft.AspNetCore.Mvc;
using POSSystem.Core.Entities;
using POSSystem.Services.Services;

namespace POSSystem.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<InvoicesController> _logger;

        public InvoicesController(
            IInvoiceService invoiceService,
            ILogger<InvoicesController> logger)
        {
            _invoiceService = invoiceService;
            _logger = logger;
        }

        /// <summary>
        /// Get all invoices
        /// </summary>
        /// <returns>List of invoices</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<InvoiceListDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var invoices = await _invoiceService.GetAllAsync();
                return Ok(invoices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all invoices");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving invoices");
            }
        }

        /// <summary>
        /// Get a specific invoice by ID
        /// </summary>
        /// <param name="id">Invoice ID</param>
        /// <returns>Invoice details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InvoiceDetailedDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var invoice = await _invoiceService.GetByIdAsync(id);
                return invoice == null ? NotFound() : Ok(invoice);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Invoice not found with ID: {InvoiceId}", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting invoice with ID: {InvoiceId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving invoice");
            }
        }

        /// <summary>
        /// Create a new invoice
        /// </summary>
        /// <param name="dto">Invoice data</param>
        /// <returns>Created invoice ID</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] InvoiceCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for invoice creation");
                return BadRequest(ModelState);
            }

            try
            {
                var invoiceId = await _invoiceService.AddAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = invoiceId }, new { Id = invoiceId });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Item not found during invoice creation");
                return BadRequest("One or more items not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating invoice");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating invoice");
            }
        }

        /// <summary>
        /// Update an existing invoice
        /// </summary>
        /// <param name="id">Invoice ID</param>
        /// <param name="dto">Updated invoice data</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] InvoiceUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for invoice update");
                return BadRequest(ModelState);
            }

            if (id != dto.Id)
            {
                _logger.LogWarning("ID mismatch in invoice update");
                return BadRequest("ID mismatch");
            }

            try
            {
                await _invoiceService.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Invoice or item not found during update");
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating invoice with ID: {InvoiceId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating invoice");
            }
        }

        /// <summary>
        /// Delete an invoice
        /// </summary>
        /// <param name="id">Invoice ID</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _invoiceService.RemoveAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Invoice not found for deletion with ID: {InvoiceId}", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting invoice with ID: {InvoiceId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting invoice");
            }
        }

        /// <summary>
        /// Calculate invoice total without saving
        /// </summary>
        /// <param name="items">List of invoice items</param>
        /// <returns>Calculated total</returns>
        [HttpPost("calculate-total")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(decimal))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CalculateTotal([FromBody] IEnumerable<InvoiceDetailCreateDto> items)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for total calculation");
                return BadRequest(ModelState);
            }

            try
            {
                var total = await _invoiceService.CalculateInvoiceTotal(items);
                return Ok(total);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Item not found during total calculation");
                return BadRequest("One or more items not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating invoice total");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error calculating total");
            }
        }
    }
}