using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using E_Ticaretim.Models;


namespace E_Ticaretim.Areas.Seller.Controllers
{
    [Area("Seller")]
    public class HomeController : Controller
    {
        private readonly E_TicaretimContext _context;

        public HomeController(E_TicaretimContext context)
        {
            _context = context;
        }

        // GET: Seller/Home
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: Seller/Home/Details/5
        public IActionResult LogIn([Bind("SellerEMail,SellerPassword")]Models.Seller seller)
        {
            var dbSeller = _context.Sellers.FirstOrDefault(m => m.SellerEMail == seller.SellerEMail);
            if (dbSeller != null)
            {

                string hashed;
                SHA256 sHA256;
                byte[] hashedPassword, sellerPassword;
                sHA256 = SHA256.Create();
                sellerPassword = Encoding.Unicode.GetBytes(seller.SellerEMail.Trim() + seller.SellerPassword.Trim());
                hashedPassword = sHA256.ComputeHash(sellerPassword);
                hashed = BitConverter.ToString(hashedPassword).Replace("-", "");
                if (hashed == dbSeller.SellerPassword)
                {
                    this.HttpContext.Session.SetInt32("merchant", dbSeller.SellerId);
                    return RedirectToAction("Index", "Products");
                }
            }
            return RedirectToAction("Index");
        }
      
    }
}
