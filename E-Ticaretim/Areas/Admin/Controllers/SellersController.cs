using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Drawing;
using E_Ticaretim.Models;


namespace E_Ticaretim.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SellersController : Controller
    {
        private readonly E_TicaretimContext _context;
        Authorization authorization = new Authorization();

        public SellersController(E_TicaretimContext context)
        {
            _context = context;
        }

        // GET: Admin/Sellers
        public async Task<IActionResult> Index()
        {
            if (authorization.IsAuthorized("viewSellers", this.HttpContext.Session) == false)
            {
                return Problem("You don't have authorization to view this page!!!");
            }
            var e_TicaretimContext = _context.Sellers.Where(s => s.IsDeleted == false).Include(s => s.City);
            return View(await e_TicaretimContext.ToListAsync());
        }

        // GET: Admin/Sellers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (authorization.IsAuthorized("viewSellers", this.HttpContext.Session) == false)
            {
                return Problem("You don't have authorization to view this page!!!");
            }
            if (id == null || _context.Sellers == null)
            {
                return NotFound();
            }

            var seller = await _context.Sellers
                .Include(s => s.City)
                .FirstOrDefaultAsync(m => m.SellerId == id);
            if (seller == null)
            {
                return NotFound();
            }

            return View(seller);
        }

        // GET: Admin/Sellers/Create
        public IActionResult Create()
        {
            //if (authorization.IsAuthorized("createSeller", this.HttpContext.Session) == false)
            //{
            //    return Problem("You don't have authorization to view this page!!!");
            //}
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName");
            return View();
        }

        // POST: Admin/Sellers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SellerId,SellerName,SellerPhone,SellerEMail,SellerPassword,ConfirmSellerPassword,SellerDescription,SellerBanned,IsDeleted,SellerRate,CityId")]E_Ticaretim.Models.Seller seller)
        {
            SHA256 sHA256;
            byte[] hashedPassword, sellerPassword;
            if (ModelState.IsValid)
            {
                sHA256 = SHA256.Create();
                sellerPassword = Encoding.Unicode.GetBytes(seller.SellerEMail.Trim() + seller.SellerPassword.Trim());
                hashedPassword = sHA256.ComputeHash(sellerPassword);
                seller.SellerPassword = BitConverter.ToString(hashedPassword).Replace("-", "");
                _context.Add(seller);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName", seller.CityId);
            return View(seller);
        }

        // GET: Admin/Sellers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (authorization.IsAuthorized("editSellers", this.HttpContext.Session) == false)
            {
                return Problem("You don't have authorization to view this page!!!");
            }
            if (id == null || _context.Sellers == null)
            {
                return NotFound();
            }

            var seller = await _context.Sellers.FindAsync(id);
            if (seller == null)
            {
                return NotFound();
            }
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName", seller.CityId);
            return View(seller);
        }

        // POST: Admin/Sellers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SellerId,SellerName,SellerPhone,SellerEMail,SellerPassword,ConfirmSellerPassword,SellerDescription,SellerBanned,IsDeleted,SellerRate,CityId")] E_Ticaretim.Models.Seller seller,string OldPassword,string OriginalPassword)
        {
            if (id != seller.SellerId)
            {
                return NotFound();
            }
            SHA256 sHA256;
            byte[] hashedPassword, sellerPassword;
            string oldHash;
            if (ModelState.IsValid)
            {
                sHA256 = SHA256.Create();
                sellerPassword = Encoding.Unicode.GetBytes(seller.SellerEMail.Trim() + OldPassword.Trim());
                hashedPassword = sHA256.ComputeHash(sellerPassword);
                oldHash = BitConverter.ToString(hashedPassword).Replace("-", "");
                if (oldHash == OriginalPassword)
                {
                    sellerPassword = Encoding.Unicode.GetBytes(seller.SellerEMail.Trim() + seller.SellerPassword.Trim());
                    hashedPassword = sHA256.ComputeHash(sellerPassword);
                    seller.SellerPassword = BitConverter.ToString(hashedPassword).Replace("-", "");
                    try
                    {
                        _context.Update(seller);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!SellerExists(seller.SellerId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName", seller.CityId);
            return View(seller);
        }

        // GET: Admin/Sellers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (authorization.IsAuthorized("deleteSeller", this.HttpContext.Session) == false)
            {
                return Problem("You don't have authorization to view this page!!!");
            }
            if (id == null || _context.Sellers == null)
            {
                return NotFound();
            }

            var seller = await _context.Sellers
                .Include(s => s.City)
                .FirstOrDefaultAsync(m => m.SellerId == id);
            if (seller == null)
            {
                return NotFound();
            }

            return View(seller);
        }

        // POST: Admin/Sellers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Sellers == null)
            {
                return Problem("Entity set 'E_TicaretimContext.Sellers'  is null.");
            }
            var seller = await _context.Sellers.FindAsync(id);
            if (seller != null)
            {
                seller.IsDeleted = true;
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SellerExists(int id)
        {
          return (_context.Sellers?.Any(e => e.SellerId == id)).GetValueOrDefault();
        }
    }
}
