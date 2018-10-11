using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MaoriSouvenirShopping.Models;

namespace MaoriSouvenirShopping.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        public DbSet<Souvenir> Souvenirs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<MaoriSouvenirShopping.Models.ApplicationUser> ApplicationUser { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            builder.Entity<Souvenir>().ToTable("Souvenir");
            //builder.Entity<Souvenir>().HasOne(s => s.Category).WithMany(c => c.Souvenirs).OnDelete(DeleteBehavior.SetNull);
            //builder.Entity<Souvenir>().HasOne(s => s.Supplier).WithMany(s => s.Souvenirs).OnDelete(DeleteBehavior.SetNull);
            builder.Entity<ApplicationUser>().ToTable("ApplicationUser");
            builder.Entity<Category>().ToTable("Category");
            builder.Entity<Supplier>().ToTable("Supplier");
            builder.Entity<CartItem>().ToTable("CartItem");
            builder.Entity<CartItem>().HasOne(c => c.Souvenir).WithMany(s => s.CartItems).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Order>().ToTable("Order");
            //builder.Entity<Order>().HasOne(o => o.User).WithMany(u => u.Orders).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<OrderDetail>().ToTable("OrderDetail");
            builder.Entity<OrderDetail>().HasOne(p => p.Order).WithMany(o => o.OrderDetails).OnDelete(DeleteBehavior.Cascade);
        }
        public DbSet<MaoriSouvenirShopping.Models.ShoppingCart> ShoppingCart { get; set; }
    }
}
