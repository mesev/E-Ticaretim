﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using Iyzipay;
using Iyzipay.Request;
using Iyzipay.Model;
using E_Ticaretim.Models;

namespace E_Ticaretim.Controllers
{
    public class PaymentController : Controller
    {
        private readonly E_TicaretimContext _context;

        public PaymentController(E_TicaretimContext context)
        {
            _context = context;
        }

        public void CheckLogIn()
        {
            string? customerId = HttpContext.Session.GetString("customer");
            if (customerId == null)
            {
                Response.Redirect("/Payment/Decision");
                return;
            }
            Response.Redirect("/Payment/MakePayment");
        }
        public IActionResult Decision()
        {
            return View();
        }
        public async Task<IActionResult> MakePayment()
        {
            var paymentMethods = _context.PaymentMethods;
            return View(await paymentMethods.ToListAsync());
        }
        public IActionResult Method(short id)
        {
            string paymentMethod = "none";
            switch (id)
            {
                case 1:
                    paymentMethod = "CreditCard";
                    break;
                case 2:
                    paymentMethod = "Havale";
                    break;
                case 3:
                    paymentMethod = "BTC";
                    break;

            }
            return PartialView(paymentMethod);

        }
        public IActionResult CheckOut(string cardname,string cardnumber,byte expmonth,short expyear,string cvv)
        {
            try
            {
                long cardNo = Convert.ToInt64(cardnumber);
                if (cardNo.ToString() != cardnumber)
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
            if (cardnumber.Length < 13)
            {
                return null;
            }
            if (cardnumber.Length > 19)
            {
                return null;
            }
            if (expmonth < 1)
            {
                return null;
            }
            if(expmonth > 12)
            {
                return null;
            }
            if (expyear < DateTime.Today.Year)
            {
                return null;
            }
            if (expyear > DateTime.Today.Year + 10)
            {
                return null;
            }
            try
            {
                short cvvNo = Convert.ToInt16(cvv);
                if (cvvNo.ToString() != cvv)
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
            if (cvv.Length != 3)
            {
                return null;
            }
                ViewData["orderId"] = "Siparişiniz Onaylanmadı";
            Order order = _context.Orders.Where(o => o.OrderId == Convert.ToInt64(HttpContext.Session.GetString("order"))).Include(o => o.Customer).First();
            Options options = new Options();
            options.ApiKey = "sandbox-3bS9Dp6k2mpKQc4SupSVQgYV6cwbzTkx";
            options.SecretKey = "sandbox-JofMv0HDsS0iCl752AWaz5uLk6HjwuNc";
            options.BaseUrl = "https://sandbox-api.iyzipay.com";

            CreatePaymentRequest request = new CreatePaymentRequest();
            request.Locale = Locale.TR.ToString();
            request.ConversationId = "123456789";
            request.Price = "1";
            request.PaidPrice = "1.2";
            request.Currency = Currency.TRY.ToString();
            request.Installment = 1;
            request.BasketId = "B67832";
            request.PaymentChannel = PaymentChannel.WEB.ToString();
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString();

            PaymentCard paymentCard = new PaymentCard();
            paymentCard.CardHolderName = "John Doe";
            paymentCard.CardNumber = "5528790000000008";
            paymentCard.ExpireMonth = "12";
            paymentCard.ExpireYear = "2030";
            paymentCard.Cvc = "123";
            paymentCard.RegisterCard = 0;
            request.PaymentCard = paymentCard;

            Buyer buyer = new Buyer();
            buyer.Id = "BY789";
            buyer.Name = "John";
            buyer.Surname = "Doe";
            buyer.GsmNumber = "+905350000000";
            buyer.Email = "email@email.com";
            buyer.IdentityNumber = "74300864791";
            buyer.LastLoginDate = "2015-10-05 12:43:35";
            buyer.RegistrationDate = "2013-04-21 15:12:09";
            buyer.RegistrationAddress = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            buyer.Ip = "85.34.78.112";
            buyer.City = "Istanbul";
            buyer.Country = "Turkey";
            buyer.ZipCode = "34732";
            request.Buyer = buyer;


            Address shippingAddress = new Address();
            shippingAddress.ContactName = "Jane Doe";
            shippingAddress.City = "Istanbul";
            shippingAddress.Country = "Turkey";
            shippingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            shippingAddress.ZipCode = "34742";
            request.ShippingAddress = shippingAddress;

            Address billingAddress = new Address();
            billingAddress.ContactName = "Jane Doe";
            billingAddress.City = "Istanbul";
            billingAddress.Country = "Turkey";
            billingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            billingAddress.ZipCode = "34742";
            request.BillingAddress = billingAddress;

            List<BasketItem> basketItems = new List<BasketItem>();
            BasketItem firstBasketItem = new BasketItem();
            firstBasketItem.Id = "BI101";
            firstBasketItem.Name = "Binocular";
            firstBasketItem.Category1 = "Collectibles";
            firstBasketItem.Category2 = "Accessories";
            firstBasketItem.ItemType = BasketItemType.PHYSICAL.ToString();
            firstBasketItem.Price = "0.3";
            basketItems.Add(firstBasketItem);

            BasketItem secondBasketItem = new BasketItem();
            secondBasketItem.Id = "BI102";
            secondBasketItem.Name = "Game code";
            secondBasketItem.Category1 = "Game";
            secondBasketItem.Category2 = "Online Game Items";
            secondBasketItem.ItemType = BasketItemType.VIRTUAL.ToString();
            secondBasketItem.Price = "0.5";
            basketItems.Add(secondBasketItem);

            BasketItem thirdBasketItem = new BasketItem();
            thirdBasketItem.Id = "BI103";
            thirdBasketItem.Name = "Usb";
            thirdBasketItem.Category1 = "Electronics";
            thirdBasketItem.Category2 = "Usb / Cable";
            thirdBasketItem.ItemType = BasketItemType.PHYSICAL.ToString();
            thirdBasketItem.Price = "0.2";
            basketItems.Add(thirdBasketItem);
            request.BasketItems = basketItems;

            Payment payment = Payment.Create(request, options);
            //Ödeme Kurulusuna gönder ve onay al...
            //Onay geldiyse
            if (payment.Status == "success")
            {
                order.TimeStamp = DateTime.Now;
                order.PaymentMethodId = 1;
                order.PaymentComplete = true;
                order.IsCart = false;
                _context.Update(order);
                _context.SaveChanges();

                MailMessage msg = new MailMessage(); //Mesaj gövdesini tanımlıyoruz...
                msg.Subject = "Şiparişiniz hk";
                msg.From = new MailAddress("info@ecommerce.com", "AMYecommerce");
                msg.To.Add(new MailAddress(order.Customer.CustomerEmail, order.Customer.CustomerName));
                //Mesaj içeriğinde HTML karakterler yer alıyor ise aşağıdaki alan TRUE olarak gönderilmeli ki HTML olarak yorumlansın. Yoksa düz yazı olarak gönderilir...
                msg.IsBodyHtml = true;
                msg.Body = "Siparişiniz onaylandı.Sipariş numaranız:" + order.OrderId.ToString();
                //Mesaj önceliği (BELİRTMEK ZORUNLU DEĞİL!)
                msg.Priority = MailPriority.Low;
                //SMTP/Gönderici bilgilerinin yer aldığı erişim/doğrulama bilgileri
                SmtpClient smtp = new SmtpClient("smtp.siteadi.com", 587); //Bu alanda gönderim yapacak hizmetin smtp adresini ve size verilen portu girmelisiniz.
                NetworkCredential AccountInfo = new NetworkCredential("info@ecommerce.com", "1234");
                smtp.UseDefaultCredentials = false; //Standart doğrulama kullanılsın mı? -> Yalnızca gönderici özellikle istiyor ise TRUE işaretlenir.
                smtp.Credentials = AccountInfo;
                smtp.EnableSsl = false; //SSL kullanılarak mı gönderilsin...
                try
                {
                    smtp.Send(msg);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("Gönderim Hatası: {0}", ex.Message));
                }
                ViewData["orderId"] = order.OrderId;
            }
            return View();
        }        
    }
}
