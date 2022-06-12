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

namespace E_Ticaretim.Controllers
{
    public class CustomersController : Controller
    {
        private readonly E_TicaretimContext _context;

        public CustomersController(E_TicaretimContext context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
              return _context.Customers != null ? 
                          View(await _context.Customers.ToListAsync()) :
                          Problem("Entity set 'E_TicaretimContext.Customers'  is null.");
        }
        public IActionResult LogIn(string currenturl)
        {
            ViewData["currentUrl"] = currenturl;
            return View();
        }
        public void ProcessLogIn([Bind("CustomerEmail", "CustomerPassword")]Models.Customer customer,string currentUrl)
        {
            SHA256 sHA256;
            byte[] hashedPassword, customerPassword;
            var dbCustomer = _context.Customers.FirstOrDefault(o => o.CustomerEmail == customer.CustomerEmail);
           if (dbCustomer != null)
            {
                sHA256 = SHA256.Create();
                customerPassword = Encoding.Unicode.GetBytes(customer.CustomerEmail.Trim() + customer.CustomerPassword.Trim());
                hashedPassword = sHA256.ComputeHash(customerPassword);
                customer.CustomerPassword = BitConverter.ToString(hashedPassword).Replace("-", "");
                if (customer.CustomerPassword == dbCustomer.CustomerPassword)
                {
                    this.HttpContext.Session.SetString("customer", dbCustomer.CustomerId.ToString());
                    TransferCart(dbCustomer.CustomerId, _context, this.HttpContext);
                    Response.Redirect(currentUrl);
                    return;
                }
            }
            Response.Redirect("LogIn");           
        }
        public void TransferCart(long customerId,E_TicaretimContext ticaretimContext,HttpContext httpContext,string newCart=null)
        {
            Models.Product product;
            string cart ;
            string[] cartItems;
            string[] itemDetails;          
            string cartItem;
            OrderDetail orderDetail;
            short productId;
            CookieOptions cookieOptions = new CookieOptions();
            if (newCart == null)
            {
               cart= Request.Cookies["cart"];
            }
            else
            {
                cart = newCart;
            }
            if(cart == "")
            {
                cart = null;
            }
            Order order;

            if (this.HttpContext.Session.GetString("order") == null)
            {
                order = new Order();
                order.AllDelivered = false;
                order.IsCart = true;
                order.Cancelled = false;
                order.CustomerId = customerId;
                order.PaymentComplete = false;
                order.TimeStamp = DateTime.Now;
            }
            else
            {
                order = ticaretimContext.Orders.FirstOrDefault(o => o.OrderId == Convert.ToInt64(this.HttpContext.Session.GetString("order")));

            }
            order.OrderDetails = new List<OrderDetail>();
            order.OrderPrice = 0;

            if (cart != null)
            {
                cartItems = cart.Split(',');// 9:4,37:1,56:2,18:9

                for (short i = 0; i < cartItems.Length; i++)
                {
                    orderDetail = new OrderDetail();
                    cartItem = cartItems[i];
                    itemDetails = cartItem.Split(':');//9[0] 4[1]
                    productId = Convert.ToInt16(itemDetails[0]);
                    product = ticaretimContext.Products.FirstOrDefault(m => m.ProductId == productId);
                    orderDetail.Count = Convert.ToByte(itemDetails[1]);
                    order.OrderPrice = product.ProductPrice * orderDetail.Count;
                    orderDetail.Product = product;
                    order.OrderPrice += orderDetail.Price;
                    order.OrderDetails.Add(orderDetail);
                }
                if (this.HttpContext.Session.GetString("order") == null)
                {
                    _context.Add(order);
                    ticaretimContext.SaveChanges();
                    this.HttpContext.Session.SetString("order", order.OrderId.ToString());
                }
                else
                {
                    _context.Update(order);
                    ticaretimContext.SaveChanges();
                }

            }
            else
            {
                if (this.HttpContext.Session.GetString("order") != null)
                {
                    _context.Remove(order);
                    ticaretimContext.SaveChanges();
                    this.HttpContext.Session.Remove("order");
                }
            }
        }
       
        // GET: Customers/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create(string currentUrl,bool noPassword=false)
        {
            ViewData["currentUrl"]=currentUrl;
            ViewData["noPassword"] = noPassword;
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,CustomerName,CustomerSurname,CustomerEmail,CustomerPhone,CustomerPassword,ConfirmCustomerPassword,CustomerAddress,IsDeleted")] Customer customer,string currentUrl)
        {
            SHA256 sHA256;
            byte[] hashedPassword, customerPassword;

            if (ModelState.IsValid)
            {
                sHA256 = SHA256.Create();
                customerPassword = Encoding.Unicode.GetBytes(customer.CustomerEmail.Trim() + customer.CustomerPassword.Trim());
                hashedPassword = sHA256.ComputeHash(customerPassword);
                customer.CustomerPassword = BitConverter.ToString(hashedPassword).Replace("-", "");
                _context.Add(customer);
                await _context.SaveChangesAsync();
                this.HttpContext.Session.SetString("customer", customer.CustomerId.ToString());
                TransferCart(customer.CustomerId, _context, this.HttpContext);
                return Redirect(currentUrl);
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("CustomerId,CustomerName,CustomerSurname,CustomerEmail,CustomerPhone,CustomerPassword,CustomerAddress,IsDeleted")] Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.Customers == null)
            {
                return Problem("Entity set 'E_TicaretimContext.Customers'  is null.");
            }
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(long id)
        {
          return (_context.Customers?.Any(e => e.CustomerId == id)).GetValueOrDefault();
        }
    }
}
