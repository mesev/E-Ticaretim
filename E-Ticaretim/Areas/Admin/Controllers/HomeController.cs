using System;

using Microsoft.AspNetCore.Mvc;

using E_Ticaretim.Areas.Admin.Models;
using System.Security.Cryptography;
using System.Text;

namespace E_Ticaretim.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly UserContext _context;

        public HomeController(UserContext context)
        {
            _context = context;
        }

        // GET: Admin/Home
        public  IActionResult Index()
        {
            return View();
        }
        public IActionResult LogIn([Bind("UserEmail,UserPassword")]User user)
        {
            var dbUser=_context.Users.FirstOrDefault(m=>m.UserEmail==user.UserEmail);
            if (dbUser != null)
            {
                string hashed;
                SHA256 sHA256;
                byte[] hashedPassword, userPassword;
                sHA256 = SHA256.Create();
                userPassword = Encoding.Unicode.GetBytes(user.UserEmail.Trim() + user.UserPassword.Trim());
                hashedPassword = sHA256.ComputeHash(userPassword);
                hashed = BitConverter.ToString(hashedPassword).Replace("-", "");
                if(hashed == dbUser.UserPassword)
                {
                    this.HttpContext.Session.SetString("guest", dbUser.UserId.ToString());
                    this.HttpContext.Session.SetString("viewUsers", dbUser.ViewUsers.ToString());
                    this.HttpContext.Session.SetString("createUser", dbUser.CreateUser.ToString());
                    this.HttpContext.Session.SetString("deleteUsers", dbUser.DeleteUser.ToString());
                    this.HttpContext.Session.SetString("editUsers", dbUser.EditUser.ToString());

                    this.HttpContext.Session.SetString("viewSellers", dbUser.ViewSellers.ToString());
                    this.HttpContext.Session.SetString("createSeller", dbUser.CreateSeller.ToString());
                    this.HttpContext.Session.SetString("deleteSeller", dbUser.DeleteSeller.ToString());
                    this.HttpContext.Session.SetString("editSellers", dbUser.EditSeller.ToString());

                    this.HttpContext.Session.SetString("viewCategories",dbUser.WiewCategories.ToString());
                    this.HttpContext.Session.SetString("createCategories", dbUser.CreateCategory.ToString());
                    this.HttpContext.Session.SetString("deleteCategories", dbUser.DeleteCategory.ToString());
                    this.HttpContext.Session.SetString("editCategories", dbUser.EditCategory.ToString());
                    this.HttpContext.Session.SetString("editProduct", dbUser.EditProduct.ToString());
                    this.HttpContext.Session.SetString("deleteProduct", dbUser.DeleteProduct.ToString());
                    return RedirectToAction("Index", "Users");
                    //Response.Redirect("~/Admin/Users/Index");
                }
                
            }
            return RedirectToAction("Index");
        }

        
    }
}
