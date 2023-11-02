using CmsShoppingCart.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CmsShoppingCart.Models;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics.CodeAnalysis;

namespace CmsShoppingCart.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly CmsShoppingCartContext context;

        public ProductsController(CmsShoppingCartContext context)
        {
            this.context = context;
        }
        
        // GET /products
        public async Task<IActionResult> Index(int p = 1)
        {
            int pageSize = 6;
            var products = context.Products.OrderByDescending(x => x.Id)
                                            .Skip((p - 1) * pageSize)
                                            .Take(pageSize);

            ViewBag.PageNumber = p;
            ViewBag.PageRange = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((decimal)context.Products.Count() / pageSize);

            return View(await products.ToListAsync());
        }
        public async Task<IActionResult> ProductsByCategory(string categorySlug, int p = 1, string filter = "")
        {
            Category category = await context.Categories.Where(x => x.Slug == categorySlug).FirstOrDefaultAsync();
            if (category == null) return RedirectToAction("Index");

            int pageSize = 6;
            var productsQuery = context.Products
                .Where(x => x.CategoryId == category.Id);

            if (!string.IsNullOrEmpty(filter))
            {
                // Apply the filter to the product name
                productsQuery = productsQuery.Where(x => x.Name.Contains(filter));
            }

            var products = productsQuery
                .OrderByDescending(x => x.Id)
                .Skip((p - 1) * pageSize)
                .Take(pageSize);

            ViewBag.PageNumber = p;
            ViewBag.PageRange = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((decimal)productsQuery.Count() / pageSize);
            ViewBag.CategoryName = category.Name;
            ViewBag.CategorySlug = categorySlug;

            return View(await products.ToListAsync());
        }

        public IActionResult Search(string query)
        {
            var results = context.Products.Where(p => p.Name.Contains(query)).ToList();
            if(results == null) return RedirectToAction("Index");
            else return View(results);
        }
        public IActionResult filter(string query)
        {
            var results = context.Products.Where(p => p.Name.Contains(query)).ToList();
            return PartialView("_SearchResults",results);
        }
        [HttpPost]
        public IActionResult FilterByCategory(string categorySlug, string filter)
        {
            return RedirectToAction("ProductsByCategory", new { categorySlug, filter });
        }
        //public IActionResult SortByPrice(bool asc = true)
        //{
        //    var products = asc ? context.Products.OrderBy(x => x.Price) : context.Products.OrderByDescending(x => x.Price);
        //    return PartialView("_ProductList", products.ToList());
        //}
        
        public async Task<IActionResult> SortByPrice(string sortOrder, int p = 1)
        {
            int pageSize = 6;
            var productsQuery = context.Products.OrderBy(x => x.Id);

            // Determine the sorting order based on the "sortOrder" parameter
            switch (sortOrder)
            {
                case "price_asc":
                    productsQuery = productsQuery.OrderBy(x => x.Price);
                    break;
                case "price_desc":
                    productsQuery = productsQuery.OrderByDescending(x => x.Price);
                    break;
                default:
                    productsQuery = productsQuery.OrderByDescending(x => x.Id);
                    break;
            }

            // Calculate the total count before pagination
            var totalCount = await productsQuery.CountAsync();

            // Apply pagination after sorting
            var products = productsQuery
                .Skip((p - 1) * pageSize)
                .Take(pageSize);

            ViewBag.PageNumber = p;
            ViewBag.PageRange = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);

            // Pass the selected sorting order to the pagination links
            ViewBag.SortOrder = sortOrder;

            return View( products.ToList());
        }

        public async Task<IActionResult> SortProductsByPrice(string categorySlug, string sortOrder, int p = 1)
        {
            int pageSize = 6;
            var category = await context.Categories.SingleOrDefaultAsync(x => x.Slug == categorySlug);

            if (category == null)
            {
                return RedirectToAction("Index");
            }

            var productsQuery = context.Products.Where(x => x.CategoryId == category.Id);

            // Determine the sorting order based on the "sortOrder" parameter
            switch (sortOrder)
            {
                case "price_asc":
                    productsQuery = productsQuery.OrderBy(x => x.Price);
                    break;
                case "price_desc":
                    productsQuery = productsQuery.OrderByDescending(x => x.Price);
                    break;
                default:
                    // Default to sorting by ID if sortOrder is not provided or invalid
                    productsQuery = productsQuery.OrderByDescending(x => x.Id);
                    break;
            }

            // Calculate the total count before pagination
            var totalCount = await productsQuery.CountAsync();

            // Apply pagination after sorting
            var products = productsQuery
                .Skip((p - 1) * pageSize)
                .Take(pageSize);

            ViewBag.PageNumber = p;
            ViewBag.PageRange = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);

            // Pass the selected sorting order to the pagination links
            ViewBag.SortOrder = sortOrder;

            ViewBag.CategoryName = category.Name;
            ViewBag.CategorySlug = categorySlug;

            return View("ProductsByCategory", await products.ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            Product product = await context.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        [HttpPost]
        public async Task<IActionResult> RateProduct(int productId, int rating)
        {
            var product = await context.Products.FindAsync(productId);

            if (product == null)
            {
                return NotFound();
            }

            product.Rating = rating;

            context.SaveChanges(); 

            return RedirectToAction("Details", new { id = productId });
        }
        [HttpPost]
        public async Task<IActionResult> SubmitReview(int productId, string reviewText)
        {
            var product = await context.Products.FindAsync(productId);

            if (product == null)
            {
                return NotFound();
            }

            var review = new Review
            {
                ProductId = productId,
                review = reviewText,
            };

            product.Reviews.Add(review);

            context.SaveChanges(); 
            return RedirectToAction("Details", new { id = productId });
        }

    }
}
