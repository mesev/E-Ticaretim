using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace E_Ticaretim.Controllers
{
    public class CartController : Controller
    {
        public struct CartProduct
        {
            public Models.Product Product;
            public short Count;
            public float Total;
        }
        public string Add(long id)
        {
            string? cart = Request.Cookies["cart"];
            string[] cartItems;
            string[] itemDetails;
            short itemCount;
            string newCart = "";
            string cartItem;
            short totalCount = 0;
            bool itemExists = false;
            CookieOptions cookieOptions = new CookieOptions();
            DbContextOptions<Models.E_TicaretimContext> options = new DbContextOptions<Models.E_TicaretimContext>();
            Models.E_TicaretimContext ticaretimContext=new Models.E_TicaretimContext(options);
            CustomersController customersController = new CustomersController(ticaretimContext);
            if (cart == null)
            {
                newCart = id.ToString() + ":1";
                totalCount = 1;
            }
            else
            {
                cartItems = cart.Split(',');
                for (short i = 0; i < cartItems.Length; i++)
                {
                    cartItem = cartItems[i];
                    itemDetails = cartItem.Split(':');
                    itemCount = Convert.ToInt16(itemDetails[1]);
                    if (itemDetails[0] == id.ToString())
                    {
                        itemCount++;
                        itemExists = true;
                    }
                    totalCount += itemCount;
                    newCart = newCart + itemDetails[0] + ":" + itemCount.ToString();
                    if (i < cartItems.Length - 1)
                    {
                        newCart = newCart + ",";
                    }
                }
                if (itemExists == false)
                {
                    newCart = newCart + "," + id.ToString() + ":1";
                    totalCount++;
                }
            }
            cookieOptions.Path = "/";
            cookieOptions.Expires = DateTime.MaxValue;
            Response.Cookies.Append("cart", newCart, cookieOptions);
            if (this.HttpContext.Session.GetString("customer") != null)
            {
                customersController.TransferCart(Convert.ToInt64(this.HttpContext.Session.GetString("customer")),ticaretimContext,this.HttpContext,"");
            }
            return totalCount.ToString();
        }
        public IActionResult Index()
        {
            DbContextOptions<Models.E_TicaretimContext> options = new DbContextOptions<Models.E_TicaretimContext>();
            Models.E_TicaretimContext e_TicaretimContext = new Models.E_TicaretimContext(options);
            Areas.Seller.Controllers.ProductsController productsController = new Areas.Seller.Controllers.ProductsController(e_TicaretimContext); ;
            Models.Product product;
            long productId;
            byte i = 0;
            string? cart = Request.Cookies["cart"];
            string[] cartItems, itemDetails;
            string cartItem;
            List<CartProduct> cartProducts = new List<CartProduct>();
            float cartTotal = 0;
            if (cart != null)
            {
                cartItems = cart.Split(',');
                for (i = 0; i < cartItems.Length; i++)
                {
                    cartItem = cartItems[i];
                    itemDetails = cartItem.Split(':');
                    CartProduct cartProduct = new CartProduct();
                    productId = Convert.ToInt16(itemDetails[0]);
                    product = productsController.Product(productId);
                    cartProduct.Product = product;
                    cartProduct.Count = Convert.ToInt16(itemDetails[1]);
                    cartProduct.Total = cartProduct.Count * product.ProductPrice;
                    cartProducts.Add(cartProduct);
                    cartTotal += cartProduct.Total;
                }
            }

            ViewData["product"] = cartProducts;
            ViewData["cartTotal"] = cartTotal;
            return View();
        }
        public string CalculateTotal(long id, byte count)
        {
            DbContextOptions<Models.E_TicaretimContext> options = new DbContextOptions<Models.E_TicaretimContext>();
            Models.E_TicaretimContext e_TicaretimContext = new Models.E_TicaretimContext(options);
            Areas.Seller.Controllers.ProductsController productsController = new Areas.Seller.Controllers.ProductsController(e_TicaretimContext); ;
            Models.Product product = productsController.Product(id);
            float subTotal = 0;
            subTotal = product.ProductPrice * count;
            ChangeCookie(id, count);
            return subTotal.ToString();
        }
        private void ChangeCookie(long id, byte count)
        {
            string? cart = Request.Cookies["cart"];
            string[] cartItems;
            string[] itemDetails;
            short itemCount;
            string newCart = "";
            string cartItem;
            short totalCount = 0;
            CookieOptions cookieOptions = new CookieOptions();

            cartItems = cart.Split(',');
            for (short i = 0; i < cartItems.Length; i++)
            {
                cartItem = cartItems[i];
                itemDetails = cartItem.Split(':');
                itemCount = Convert.ToInt16(itemDetails[1]);
                if (itemDetails[0] == id.ToString())
                {
                    itemCount = count;
                }
                if (itemCount == 0)
                {
                    continue;
                }
                totalCount += itemCount;
                newCart = newCart + itemDetails[0] + ":" + itemCount.ToString();
                if (i < cartItems.Length - 1)
                {
                    newCart = newCart + ",";
                }
            }
            if (newCart != "")
            {
                if (newCart.Substring(newCart.Length - 1) == ",")
                {
                    newCart = newCart.Substring(0, newCart.Length - 1);
                }
            }
            else
            {
                Response.Cookies.Delete("cart");
                return;
            }
            cookieOptions.Path = "/";
            cookieOptions.Expires = DateTime.MaxValue;
            Response.Cookies.Append("cart", newCart, cookieOptions);
            ViewData["totalCount"] = totalCount;
        }
        public void EmptyBasket()
        {
            DbContextOptions<Models.E_TicaretimContext> options = new DbContextOptions<Models.E_TicaretimContext>();
            Models.E_TicaretimContext ticaretimContext = new Models.E_TicaretimContext(options);
            CustomersController customersController = new CustomersController(ticaretimContext);
            Response.Cookies.Delete("cart");
            if (this.HttpContext.Session.GetString("customer") != null)
            {
                customersController.TransferCart(Convert.ToInt64(this.HttpContext.Session.GetString("customer")), ticaretimContext, this.HttpContext, "");
            }
            Response.Redirect("Index");
        }
    }
}
