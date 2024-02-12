using FurnitureStore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FurnitureStore.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }


        public DbSet<Client> Client {  get; set; }
        public DbSet<Order> Order {  get; set; }
        public DbSet<Product> Product {  get; set; }
        public DbSet<ProductCategory> ProductCategory {  get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-IHUPKES\\SQLEXPRESS;Database=forniture_store;Integrated Security=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OrderDetails>().HasKey(od => new
            {
                od.ProductId,od.OrderId
            });
        }
    }
}
