using AutoMapper;
using Microsoft.Extensions.Logging;
using POSSystem.Core.Entities;
using POSSystem.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace POSSystem.Services.Services
{
    public interface IItemsService
    {
        Task<GetItemDto> GetByIdAsync(int id);
        Task<Item> GetEntityByIdAsync(int id);
        Task<IEnumerable<GetItemDto>> GetAllAsync();
        Task AddAsync(AddItemDto entity);
        void Update(Item entity);
        void Remove(Item entity);
        void Remove(int id);
        Task DecreaseItemQuantity(int itemId, int quantity);

    }
    public class ItemsService : IItemsService
    {
        private readonly ILogger<ItemsService> _logger;
        private readonly IRepository<Item> _repo;
        private readonly IMapper _mapper;


        public ItemsService(ILogger<ItemsService> logger, IRepository<Item> repo, IMapper mapper)
        {
            _logger = logger;
            _repo = repo;
            _mapper = mapper;
        }

        public async Task AddAsync(AddItemDto model)
        {
            if (model == null)
            {
                _logger.LogWarning("Attempted to add null item");
                throw new ArgumentNullException(nameof(model));
            }

            try
            {
                _logger.LogInformation("Adding new item {@Item}", model);
                var entity = _mapper.Map<Item>(model);
                _repo.AddAsync(entity);
                _repo.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public async Task<IEnumerable<GetItemDto>?> GetAllAsync()
        {
            try
            {
                _logger.LogWarning("Attempted to GetAllAsync");

                var items = await _repo.GetAllAsync(i => i.Category);
                return _mapper.Map<IEnumerable<GetItemDto>>(items) ?? Enumerable.Empty<GetItemDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                throw ex;
            }
        }

        public async Task<GetItemDto?> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _repo.GetByIdAsync(id, i => i.Category);

                if (entity == null)
                    throw new KeyNotFoundException("Attempted to request item that doesnt exists");

                var model = _mapper.Map<GetItemDto>(entity);
                return model;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return null;
            }
        }

        public async Task<Item?> GetEntityByIdAsync(int id)
        {
            try
            {
                var entity = await _repo.GetByIdAsync(id, i => i.Category);

                if (entity == null)
                    throw new KeyNotFoundException("Attempted to request item that doesnt exists");

                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return null;
            }
        }

        public async void Remove(Item entity)
        {
            if (entity == null)
            {
                _logger.LogWarning("Attempted to Remove null item");
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                _repo.Remove(entity);
                _repo.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public async void Remove(int id)
        {
            var entity = await GetEntityByIdAsync(id);

            Remove(entity);
        }

        public async void Update(Item entity)
        {
            var old = await _repo.GetByIdAsync(entity.Id);

            if (old == null)
                throw new KeyNotFoundException("Attempted to update item that doesnt exists");

            try
            {
                _repo.Update(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public async Task DecreaseItemQuantity(int itemId, int quantity)
        {
            var item = await GetEntityByIdAsync(itemId);

            item.Quantity -= quantity;
            _repo.Update(item);

        }
    }
}
