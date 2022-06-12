using Microsoft.EntityFrameworkCore;
using E_Ticaretim.Models;
namespace E_Ticaretim.Areas.Admin.Models
{
    public class UserContext:DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
    }
}
