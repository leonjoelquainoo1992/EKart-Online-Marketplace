using EKartDataAccessLayer;
using EKartMVCApp.Repository;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EKartMVCApp.Controllers
{
    public class CustomerController : Controller
    {
        public ActionResult UpdateCustomerDetails(Models.UpdateCustomerDetails models)
        {
            try
            {
                EKartRepository dal = new EKartRepository();
                bool status = dal.UpdateCustomerDetails(models.EmailId, models.FirstName, models.LastName, models.PhoneNumber, models.ZipCode, models.AddressLine1, models.City, models.State, models.Country);
                ViewBag.CustomerID = Session["email"];
                if (status)
                {
                    return Redirect("/Category/ViewCategories" + Session["username"]);
                }
                return View();
            }
            catch (Exception)
            {
                return View("ErrorPage");
            }

        }

        public ActionResult GetCart()
        {
            List<Models.ShoppingCart> lstAllProductsInCart = new List<Models.ShoppingCart>();
            EKartMapper<ShoppingCart, Models.ShoppingCart> mapobj = new EKartMapper<ShoppingCart, Models.ShoppingCart>();
            EKartRepository dal = new EKartRepository();
            var lstEntityProducts = dal.GetProductsInCartUsingLinq();
            foreach (var product in lstEntityProducts)
            {
                lstAllProductsInCart.Add(mapobj.Translate(product));
            }
            return View(lstAllProductsInCart);
        }

        public ActionResult AddToCart(string id)
        {
            EKartRepository dal = new EKartRepository();
            bool status = dal.AddToShoppingCart(id);
            if (status)
            {
                return Redirect("/Product/GetCart");
            }
            return View();
        }

        public ActionResult LogOut()
        {
            EKartRepository dal = new EKartRepository();
            bool status = dal.RemoveShoppingCartContent();
            if (status)
            {
                Session.Abandon();
                return Redirect("/User/LogIn");
            }
            else
                return View("ErrorPage");

        }

        public ActionResult Contact(Models.Contact contact)
        {
            if (ModelState.IsValid)
            {
                return View("_LayoutMessageSent");
            }
            return View("Contact");
        }

        public ActionResult ChangePassword(Models.Users models)
        {
            try
            {
                EKartRepository dal = new EKartRepository();
                bool status = dal.ChangePassword(models.EmailId, models.OldPassword, models.UserPassword, models.ConfirmPassword);
                ViewBag.Email = Session["email"];
                if (status)
                {
                    return Redirect("/Category/ViewCategories?username=" + Session["username"]);
                }
                return View();
            }
            catch (Exception)
            {
                return View("ErrorPage");
            }
        }

        public ActionResult GetRecentPurchases()
        {
            string email = Session["email"] + "";
            EKartRepository dal = new EKartRepository();
            List<string> products = new List<string>();
            List<PurchaseDetail> listPurchases = dal.RecentPurchasesUsingLinq(email);
            foreach (var item in listPurchases)
            {
                products.Add(item.Product.ProductName);
            }
            ViewBag.TopPurchases = products;
            return View();
        }

        public ActionResult RetrieveCustomersCards()
        {
            List<Models.CreditCard> creditCardList = new List<Models.CreditCard>();
            EKartMapper<CardDetail, Models.CreditCard> mapobj = new EKartMapper<CardDetail, Models.CreditCard>();
            EKartRepository dal = new EKartRepository();
            string email = Session["email"] + "";
            var lstEntityCreditCard = dal.RetrieveCustomersCard(email);
            foreach (var card in lstEntityCreditCard)
            {
                creditCardList.Add(mapobj.Translate(card));
            }
            return View(creditCardList);
        }

        public ActionResult CardDetails(string cardNumber)
        {
            List<Models.CreditCard> cardDetailsList = new List<Models.CreditCard>();
            EKartMapper<CardDetail, Models.CreditCard> mapobj = new EKartMapper<CardDetail, Models.CreditCard>();
            EKartRepository dal = new EKartRepository();
            var lstEntityCardDetails = dal.CardDetails(cardNumber);
            foreach (var cardDetail in lstEntityCardDetails)
            {
                cardDetailsList.Add(mapobj.Translate(cardDetail));
            }
            return View(cardDetailsList);
        }

        public ActionResult DeleteCard(string cardNumber)
        {
            return View();
        }
    }
}
