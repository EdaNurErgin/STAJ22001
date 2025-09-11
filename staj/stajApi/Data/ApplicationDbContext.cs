using stajApi.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;


namespace stajApi.Data
{
    public class ApplicationDbContext : DbContext  // DbContext'ten türemeli
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) // DbContext'e base constructor'ı çağırıyoruz
        { }

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }





        // ApplicationDbContext.cs
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Web tarafındaki kullanıcı ve mobil tarafındaki müşterinin siparişlerini ilişkisel hale getir
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.NoAction); // Cascade delete yerine Restrict kullan

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete yerine Restrict kullan

            // OrderDetail ile Order ve Product arasındaki ilişkiyi tanımlıyoruz
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Restrict);  // Order silindiğinde OrderDetail silinmesin

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Restrict);  // Product silindiğinde OrderDetail silinmesin
        }


    }
}


