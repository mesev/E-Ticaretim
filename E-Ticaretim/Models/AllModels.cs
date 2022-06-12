using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace E_Ticaretim.Models
{
   
    public class Brand
    {
        public short BrandId { get; set; }
        [Required]
        [Column(TypeName = "nchar(50)")]
        public string BrandName { get; set; }
    }
    public class City
    {
        public short CityId { get; set; }
        [Required]
        [Column(TypeName = "nchar(20)")]
        public string CityName { get; set; }

    }
    public class Seller
    {
        public int SellerId { get; set; }
        [Required]
        [Column(TypeName = "nchar(50)")]
        public string SellerName { get; set; }
        [Required]
    
        [DataType(DataType.PhoneNumber)]
        public long  SellerPhone { get; set; }
        [Required]
        [EmailAddress]
        [Column(TypeName ="char(100)")]
        public string SellerEMail { get; set; }

        [Required]
        [Column(TypeName ="char(64)")]
        [DataType(DataType.Password)]
        public string SellerPassword { get; set; }
        [NotMapped]
        [Required]
        [DataType(DataType.Password)]
        [Compare("SellerPassword",ErrorMessage ="Şifreler Uyuşmuyor")]
        public string ConfirmSellerPassword { get; set; }
        [Column(TypeName = "nchar(200)")]
        public string? SellerDescription { get; set; }
        [Required]
        public bool SellerBanned { get; set; }
        [Required]
        public bool IsDeleted { get; set; }

        public float? SellerRate { get; set; }
        [Required]
        public short CityId { get; set; }
        public City? City { get; set; }
        public List<Product>? Products { get; set; }


    }
    public class Category
    {
        public short CategoryId { get; set; }
        [Required]
        [Column(TypeName ="nchar(50)")]
        public string CategoryName { get; set; }
        [Required]
        public bool IsDeleted { get; set; }
    }
    public class Product
    {
        public long ProductId { get; set; }
        [Required]
        [Column(TypeName = "nchar(100)")]
        public string ProductName { get; set; }
        [Required]
        public float ProductPrice { get; set; }
        [NotMapped]
        public IFormFile[]? Image { get; set; }
      
        [Column(TypeName = "nchar(200)")]
        public string? Description { get; set; }
        [Required]
        public bool IsDeleted { get; set; }
        public float? ProductRate { get; set; }
        [Required]
        public short CategoryId { get; set; }
        [Required]
        public short BrandId { get; set; }
        [Required]
        public int SellerId{ get; set; }
        public Seller? Seller { get; set; }
        public Category? Category { get; set; }
        public Brand? Brand { get; set; }
    }
    public class Customer
    {
        public long CustomerId { get; set; }
        [Required]
        [Column(TypeName ="nchar(50)")]
        public string CustomerName { get; set; }
        [Required]
        [Column(TypeName = "nchar(50)")]
        public string CustomerSurname { get; set; }
        [Required]
        [Column(TypeName = "nchar(100)")]
        [EmailAddress]
        public string CustomerEmail { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string CustomerPhone { get; set; }
        [Column(TypeName = "nchar(64)")]
        [DataType(DataType.Password)]
        public string? CustomerPassword { get; set; }
        [NotMapped]
        [DataType(DataType.Password)]
        [Compare("CustomerPassword", ErrorMessage = "Şifreler Uyuşmuyor")]
        public string ConfirmCustomerPassword { get; set; } 
        [Required]
        [Column(TypeName = "nchar(200)")]
        public string CustomerAddress { get; set; }
        [Required]
        public bool IsDeleted { get; set; }
        public List<Order>? Orders { get; set; }

    }
    public class PaymentMethod
    {
        public short PaymentMethodId { get; set; }
        [Required]
        [Column(TypeName ="nchar(30)")]
        public string PaymentMethodName { get; set; }
    }
    public class Order
    {
        public long OrderId { get; set; }
        [Required]
        public float OrderPrice { get; set; }
        [Required]
        public DateTime TimeStamp { get; set; }
        [Required]
        public long CustomerId { get; set; }
        [Required]
        public bool AllDelivered { get; set; }
        [Required]
        public bool Cancelled { get; set; }
     
        public short? PaymentMethodId { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        [Required]
        public bool PaymentComplete { get; set; }    
        public bool IsCart { get; set; }
        public Customer? Customer { get; set; }
        public List<OrderDetail>? OrderDetails { get; set; }
    }
    public class ItemStatus
    {
        public short ItemStatusId { get; set; }
        [Required]
        [Column(TypeName ="nchar(50)")]
        public string ItemStatusName { get; set; }
    }
    public class OrderDetailStatus
    {
        public long OrderDetailStatusId { get; set; }
        [Required]
        public long OrderDetailId { get; set; }
        public OrderDetail? OrderDetail { get; set; }
        [Required]
        public DateTime ChangeItemStatus { get; set; }
        [Required]
        public short ItemStatusId { get; set; }
        public ItemStatus? ItemStatus { get; set; }
    }
    public class OrderDetail
    {
        public long OrderDetailId { get; set; }
        [Required]
        public long OrderId { get; set; }
        public Order? Order { get; set; }
        [Required]
        public long ProductId { get; set; }
        public Product? Product { get; set; }
        [Required]
        public byte Count { get; set; }
        [Required]
        public float Price { get; set; }
        public List<OrderDetailStatus>? OrderDetailStatuses { get; set; } 
    }
}
