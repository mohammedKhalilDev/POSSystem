using Microsoft.AspNetCore.Mvc;
using POSSystem.Core.Entities;
using POSSystem.Services.Services;

namespace POSSystem.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IItemsService _itemService;
        public ItemsController(IItemsService itemService)
        {
            _itemService = itemService;
        }

        [HttpGet("GetAll")]
        public async Task<IEnumerable<GetItemDto>> GetAll()
        {
            return await _itemService.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<GetItemDto> Get(int id)
        {
            return await _itemService.GetByIdAsync(id);
        }

        // POST api/<ItemsController>
        [HttpPost]
        public async Task Add([FromBody] AddItemDto value)
        {
            await _itemService.AddAsync(value);
        }

        // PUT api/<ItemsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ItemsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _itemService.Remove(id);
        }
    }
}
