using CmsShoppingCart.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CmsShoppingCart.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<AppUser> userManager;

        public UsersController(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            return View(userManager.Users);
        }

        // GET /admin/users/delete
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            AppUser user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            IdentityResult result = await userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                TempData["Success"] = "The user has been deleted!";
            }
            else
            {
                TempData["Error"] = "An error occurred while deleting the user.";
            }

            return RedirectToAction("Index");
        }
    }
}
