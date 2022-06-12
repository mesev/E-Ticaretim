using Microsoft.EntityFrameworkCore;
using E_Ticaretim.Models;


namespace E_Ticaretim.Models
{
    public class E_TicaretimContext:DbContext
    {
        public E_TicaretimContext(DbContextOptions<E_TicaretimContext> options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=DESKTOP-Q0IKKRN\\SQLEXPRESS;Initial Catalog=E-Ticaret;User Id=sa;Password=1234");
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Seller> Sellers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<OrderDetailStatus> OrderDetailStatuses { get; set; }
        public DbSet<ItemStatus> ItemStatuses { get; set; }
    }
}
