using FurnitureStore.Data;
using FurnitureStore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        public readonly ApplicationDbContext _db;
        public ClientController(ApplicationDbContext _context)
        {
            _db = _context;
        }

        [HttpGet]
        public async Task<IEnumerable<Client>> Get()
        {
          return await _db.Client.ToListAsync();               
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var client = await _db.Client.FirstOrDefaultAsync(c => c.Id == id); 
            
            if(client == null)
            {
                return BadRequest();
            }
            return Ok(client);
        }
        [HttpPost]
        public async Task<IActionResult> Add(Client client)
        {
          await  _db.Client.AddAsync(client);
          await  _db.SaveChangesAsync();
           return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Edit(Client client)
        {
             _db.Client.Update(client);
            await _db.SaveChangesAsync();
            return NoContent();

        }
        [HttpDelete]
        public async Task<IActionResult> Delete(Client client)
        {
            var item = await _db.Client.FindAsync(client.Id);
            if(item == null)
            {
                return NotFound();
            }
           _db.Client.Remove(item);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}
