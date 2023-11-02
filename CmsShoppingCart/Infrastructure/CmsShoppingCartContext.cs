using CmsShoppingCart.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CmsShoppingCart.Infrastructure
{
    public class CmsShoppingCartContext: IdentityDbContext<AppUser>
    {
        public CmsShoppingCartContext(DbContextOptions<CmsShoppingCartContext> options)
            : base(options)
        {
        }
        public DbSet<Page> Pages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Rate> Rates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
