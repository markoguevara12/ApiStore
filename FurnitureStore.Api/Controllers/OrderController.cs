using FurnitureStore.Data;
using FurnitureStore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        public readonly ApplicationDbContext _db;
        public OrderController(ApplicationDbContext _context)
        {
            _db = _context;
        }
        [HttpGet]
        public  async Task<IActionResult> Get()
        {
            var orderDetails = await _db.Order.Include(od => od.OrderDetails).ToListAsync();

            return Ok(orderDetails);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult>GetDetail(int id)
        {
            var order = await _db.Order.Include(od => od.OrderDetails).FirstOrDefaultAsync(o => o.Id == id);
            if(order == null)
            {
                return BadRequest("No se encontro la orden");
            }
            return Ok(order);
        }
        [HttpPost]
        public async Task<IActionResult>Add(Order order)
        {
            if(order.OrderDetails == null)
            {
                return BadRequest("The order must have at least one detail");
            }

            await _db.Order.AddAsync(order);
            await _db.OrderDetails.AddRangeAsync(order.OrderDetails);

            await _db.SaveChangesAsync();
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Edit(Order order)
        {
            if (order == null) return NotFound(); //Order no puede estar vacio
            if (order.Id <= 0) return NotFound();// El Id de la orden no puede ser cero o menor que cero

            var existingOrder = await _db.Order
                .Include(o => o.OrderDetails) //buscamos la ordern en la base de datos e incluimos el orderDetails
                .FirstOrDefaultAsync(o => o.Id == order.Id);

            if (existingOrder == null) return NotFound(); //verificamos que no venga vacio el objeto

            existingOrder.OrderNumber = order.OrderNumber; 
            existingOrder.OrderDate = order.OrderDate;     //Actualizamos los datos con los nuevos datos que estamos pasando por parametros
            existingOrder.DeliveryDate = order.DeliveryDate;
            existingOrder.ClientId = order.ClientId;
           
            _db.OrderDetails.RemoveRange(existingOrder.OrderDetails); //Se eliminan todos los detalles viejos para luego reemplazarlos por los nuevos

             _db.Order.Update(existingOrder); 
             _db.OrderDetails.AddRange(order.OrderDetails); 
            await _db.SaveChangesAsync(); 
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Order order)
        {
            if (order == null) return NotFound();

            var existingOrder = await _db.Order.Include(o => o.OrderDetails).FirstOrDefaultAsync(o => o.Id == order.Id);
            if (existingOrder == null) return NotFound();

            _db.OrderDetails.RemoveRange(existingOrder.OrderDetails);
            _db.Order.Remove(existingOrder);            
            await _db.SaveChangesAsync();
            return Ok();
        }
        
    }
}
