using FurnitureStore.Data;
using FurnitureStore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        public readonly ApplicationDbContext _db;
        public ProductController(ApplicationDbContext _context)
        {
            _db = _context;
        }


        [HttpGet("GetCategory/{CategoryId}")]
        public async Task<IActionResult> CategoryFilter(int CategoryId)
        {
            var CategoriesItem = await _db.Product
                .Where(ci => ci.ProductCategoryId == CategoryId)
                .ToListAsync();
            return Ok(CategoriesItem);
        }
        

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _db.Product.ToListAsync();

          
            return Ok(products);
        }
        [HttpGet("{id}")]
       
        public async Task<IActionResult> GetDetails(int id)
        {
            var item = await _db.Product.FirstOrDefaultAsync(x => x.Id == id);

            if(item == null)
            {
                return BadRequest();
            }
            return Ok(item);
        } 

        [HttpPost]
        public async Task<IActionResult> Add(Product product)
        {
            if(product == null)
            {
                return BadRequest();
            }

             _db.Product.Add(product);
             await _db.SaveChangesAsync();
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Edit (Product product)
        {

            var item = await _db.Product.FindAsync(product.Id);

            if (item == null)
            {
                return BadRequest();
            }
            _db.Product.Update(item);
            await _db.SaveChangesAsync();
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.Product.FirstOrDefaultAsync(p => p.Id == id);
            if(item == null)
            {
                return BadRequest();
            }
            _db.Product.Remove(item);
            await _db.SaveChangesAsync();
            return Ok();
            
        }
    }
}
