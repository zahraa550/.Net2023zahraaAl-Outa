using CmsShoppingCart.Infrastructure;
using CmsShoppingCart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CmsShoppingCart.Controllers
{
    public class OrderController : Controller
    {
        private readonly CmsShoppingCartContext context;

        public OrderController(CmsShoppingCartContext context)
        {
            this.context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userOrders = await context.Orders.Where(o => o.userId == userId).Include(s => s.CartItems).ToListAsync();
            return View("Index", userOrders);
        }


        [HttpGet("{id}/summary")]
        public async Task<ActionResult<Order>> GetOrderSummary(int id)
        {
            var order = await context.Orders.Include(o => o.CartItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            context.Orders.Remove(order);
            await context.SaveChangesAsync();

            return NoContent(); // Indicates a successful deletion
        }
    }
}
