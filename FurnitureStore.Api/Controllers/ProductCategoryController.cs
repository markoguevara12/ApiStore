using FurnitureStore.Data;
using FurnitureStore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FurnitureStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCategoryController : ControllerBase
    {
        public readonly ApplicationDbContext _db;
        public ProductCategoryController(ApplicationDbContext _context)
        {
            _db = _context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllProductCategory()
        {
            var lista = await _db.ProductCategory.ToListAsync();
            return Ok(lista);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductCategory(int id)
        {
          var item = await _db.ProductCategory.FirstOrDefaultAsync(i => i.Id == id);
            if(item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }
        [HttpPost]
        public async Task<IActionResult> AddProductCategory(ProductCategory productCategory)
        {
            if (productCategory == null)
            {
                return BadRequest();
            }
            _db.ProductCategory.Add(productCategory);
            await _db.SaveChangesAsync();
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Edit(ProductCategory productCategory)
        {
           
            if (productCategory == null)
            {
                return BadRequest();
            }
             _db.ProductCategory.Update(productCategory);
            await _db.SaveChangesAsync();
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(ProductCategory productCategory)
        {
            var item = _db.ProductCategory.FirstOrDefault(i  => i.Id == productCategory.Id);
            if (item == null)
            {
                return NotFound();
            }
             _db.ProductCategory.Remove(item);
            await _db.SaveChangesAsync();
            return Ok();

        }

    }
}
