using AutoMapper;
using Microsoft.Extensions.Logging;
using POSSystem.Core.Entities;
using POSSystem.Core.Messages;
using POSSystem.Core.Repository;
using POSSystem.Infrastructure.RabbitMQ.Interfaces;
using POSSystem.Services.Messaging.Emailing;
using POSSystem.Services.Messaging.ItemStock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSSystem.Services.Services
{
    public interface IInvoiceService
    {
        Task<InvoiceDetailedDto> GetByIdAsync(int id);
        Task<Invoice> GetEntityByIdAsync(int id);
        Task<IEnumerable<InvoiceListDto>> GetAllAsync();
        Task<int> AddAsync(InvoiceCreateDto dto);
        Task UpdateAsync(int id, InvoiceUpdateDto dto);
        Task RemoveAsync(int id);
        Task<decimal> CalculateInvoiceTotal(IEnumerable<InvoiceDetailCreateDto> items);
    }

    public class InvoiceService : IInvoiceService
    {
        private readonly ILogger<InvoiceService> _logger;
        private readonly IRepository<Invoice> _invoiceRepo;
        private readonly IRepository<InvoiceDetail> _invoiceDetailRepo;
        private readonly IRepository<Item> _itemRepo;
        private readonly IMapper _mapper;
        private readonly IItemStockPublisher _itemStockPublisher;
        private readonly IEmailPublisher _emailPublisher;

        public InvoiceService(
            ILogger<InvoiceService> logger,
            IRepository<Invoice> invoiceRepo,
            IRepository<InvoiceDetail> invoiceDetailRepo,
            IRepository<Item> itemRepo,
            IMapper mapper,
            IItemStockPublisher itemStockPublisher,
            IEmailPublisher emailPublisher)
        {
            _logger = logger;
            _invoiceRepo = invoiceRepo;
            _invoiceDetailRepo = invoiceDetailRepo;
            _itemRepo = itemRepo;
            _mapper = mapper;
            _itemStockPublisher = itemStockPublisher;
            _emailPublisher = emailPublisher;
        }

        public async Task<int> AddAsync(InvoiceCreateDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Attempted to add null invoice");
                throw new ArgumentNullException(nameof(dto));
            }

            try
            {
                var total = await CalculateInvoiceTotal(dto.Items);

                var invoice = _mapper.Map<Invoice>(dto);
                invoice.Total = total;

                await _invoiceRepo.AddAsync(invoice);
                await _invoiceRepo.SaveChangesAsync();

                // Create invoice details
                foreach (var itemDto in dto.Items)
                {
                    var item = await _itemRepo.GetByIdAsync(itemDto.ItemId);
                    if (item == null)
                    {
                        throw new KeyNotFoundException($"Item with ID {itemDto.ItemId} not found");
                    }

                    var detail = new InvoiceDetail
                    {
                        InvoiceId = invoice.Id,
                        ItemId = itemDto.ItemId,
                        Quantity = itemDto.Quantity,
                        Total = item.Price * itemDto.Quantity
                    };

                    await _invoiceDetailRepo.AddAsync(detail);


                    _itemStockPublisher.Publish(new SaleItemMessage { ItemId = detail.ItemId, QuantitySold = detail.Quantity });
                }

                await _invoiceDetailRepo.SaveChangesAsync();

                _emailPublisher.Publish(new SendEmailMessage { OrderId = invoice.Id });

                _logger.LogInformation("Created new invoice with ID {InvoiceId}", invoice.Id);
                return invoice.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating invoice");
                throw;
            }
        }

        public async Task<IEnumerable<InvoiceListDto>> GetAllAsync()
        {
            try
            {
                var invoices = await _invoiceRepo.GetAllAsync();
                return _mapper.Map<IEnumerable<InvoiceListDto>>(invoices) ?? Enumerable.Empty<InvoiceListDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all invoices");
                throw;
            }
        }

        public async Task<InvoiceDetailedDto> GetByIdAsync(int id)
        {
            try
            {
                var invoice = await _invoiceRepo.GetByIdAsync(id,
                    i => i.InvoiceDetails,
                    i => i.InvoiceDetails.Select(d => d.Item));

                if (invoice == null)
                {
                    throw new KeyNotFoundException($"Invoice with ID {id} not found");
                }

                var dto = _mapper.Map<InvoiceDetailedDto>(invoice);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting invoice by ID {InvoiceId}", id);
                throw;
            }
        }

        public async Task<Invoice> GetEntityByIdAsync(int id)
        {
            try
            {
                var invoice = await _invoiceRepo.GetByIdAsync(id,
                    i => i.InvoiceDetails,
                    i => i.InvoiceDetails.Select(d => d.Item));

                if (invoice == null)
                {
                    throw new KeyNotFoundException($"Invoice with ID {id} not found");
                }

                return invoice;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting invoice entity by ID {InvoiceId}", id);
                throw;
            }
        }

        public async Task RemoveAsync(int id)
        {
            try
            {
                var invoice = await GetEntityByIdAsync(id);

                var details = (await _invoiceDetailRepo.GetAllAsync())
                    .Where(d => d.InvoiceId == id)
                    .ToList();

                await _invoiceDetailRepo.RemoveRange(details);

                // Remove the invoice
                await _invoiceRepo.Remove(invoice);

                await _invoiceRepo.SaveChangesAsync();
                _logger.LogInformation("Removed invoice with ID {InvoiceId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing invoice with ID {InvoiceId}", id);
                throw;
            }
        }

        public async Task UpdateAsync(int id, InvoiceUpdateDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            try
            {
                var existingInvoice = await GetEntityByIdAsync(id);

                _mapper.Map(dto, existingInvoice);

                existingInvoice.Total = await CalculateInvoiceTotal(dto.Items);

                // Get existing details
                var existingDetails = (await _invoiceDetailRepo.GetAllAsync())
                    .Where(d => d.InvoiceId == id)
                    .ToList();

                // Process details
                foreach (var itemDto in dto.Items)
                {
                    var item = await _itemRepo.GetByIdAsync(itemDto.ItemId);
                    if (item == null)
                    {
                        throw new KeyNotFoundException($"Item with ID {itemDto.ItemId} not found");
                    }

                    var existingDetail = existingDetails.FirstOrDefault(ed => ed.Id == itemDto.Id);

                    if (existingDetail != null)
                    {
                        // Update existing detail
                        existingDetail.ItemId = itemDto.ItemId;
                        existingDetail.Quantity = itemDto.Quantity;
                        existingDetail.Total = item.Price * itemDto.Quantity;
                        await _invoiceDetailRepo.Update(existingDetail);
                    }
                    else
                    {
                        // Add new detail
                        var newDetail = new InvoiceDetail
                        {
                            InvoiceId = id,
                            ItemId = itemDto.ItemId,
                            Quantity = itemDto.Quantity,
                            Total = item.Price * itemDto.Quantity
                        };
                        await _invoiceDetailRepo.AddAsync(newDetail);
                    }
                }

                // Remove details not in the DTO
                var detailsToRemove = existingDetails
                    .Where(ed => !dto.Items.Any(idto => idto.Id == ed.Id));

                await _invoiceDetailRepo.RemoveRange(detailsToRemove);


                await _invoiceRepo.SaveChangesAsync();
                _logger.LogInformation("Updated invoice with ID {InvoiceId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating invoice with ID {InvoiceId}", id);
                throw;
            }
        }

        public async Task<decimal> CalculateInvoiceTotal(IEnumerable<InvoiceDetailCreateDto> items)
        {
            decimal total = 0;

            foreach (var item in items)
            {
                var itemEntity = await _itemRepo.GetByIdAsync(item.ItemId);
                if (itemEntity == null)
                {
                    throw new KeyNotFoundException($"Item with ID {item.ItemId} not found");
                }

                total += itemEntity.Price * item.Quantity;
            }

            return total;
        }

        public async Task<decimal> CalculateInvoiceTotal(IEnumerable<InvoiceDetailUpdateDto> items)
        {
            decimal total = 0;

            foreach (var item in items)
            {
                var itemEntity = await _itemRepo.GetByIdAsync(item.ItemId);
                if (itemEntity == null)
                {
                    throw new KeyNotFoundException($"Item with ID {item.ItemId} not found");
                }

                total += itemEntity.Price * item.Quantity;
            }

            return total;
        }
    }
}
