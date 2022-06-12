using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace E_Ticaretim.Areas.Admin.Models
{
    public class User
    {
        public short UserId { get; set; }
        [Required]
        [Column(TypeName = "char(100)")]
        [EmailAddress]
        public string UserEmail { get; set; }
        [Required]
        [Column(TypeName = "char(64)")]
        [DataType(DataType.Password)]
        public string UserPassword { get; set; }
        [NotMapped]
        [Compare("UserPassword", ErrorMessage = "Password and Confirmation Password must match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        [Required]
        public bool IsDeleted { get; set; }
        [Required]
        public bool ViewUsers { get; set; }
        [Required]
        public bool CreateUser { get; set; }
        [Required]
        public bool DeleteUser { get; set; }
        [Required]
        public bool EditUser { get; set; }

        [Required]
        public bool ViewSellers { get; set; }
        [Required]
        public bool CreateSeller { get; set; }
        [Required]
        public bool DeleteSeller { get; set; }
        [Required]
        public bool EditSeller { get; set; }

        [Required]
        public bool WiewCategories { get; set; }
        [Required]
        public bool CreateCategory { get; set; }
        [Required]
        public bool DeleteCategory { get; set; }
        [Required]
        public bool EditCategory { get; set; }

        [Required]
        public bool DeleteProduct { get; set; }
        [Required]
        public bool EditProduct { get; set; }



    }
}
