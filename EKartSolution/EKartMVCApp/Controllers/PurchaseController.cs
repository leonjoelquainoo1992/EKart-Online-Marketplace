using EKartDataAccessLayer;
using EKartMVCApp.Models;
using EKartMVCApp.Repository;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EKartMVCApp.Controllers
{
    public class PurchaseController : Controller
    {
        public ActionResult CheckOut(Models.CreditCard model)
        {
            try
            {
                string cardNumber = model.CreditCardNumber;
                ViewBag.EmailID = Session["email"];
                string emailId = Session["email"] + "";
                EKartRepository dal = new EKartRepository();
                bool status = dal.Transaction(emailId, model.CreditCardNumber, model.NameOnCard, model.CardType, model.CVV, model.ExpiryDate);
                if (status)
                {
                    if (dal.AddToPurchaseDetails(emailId) == 1)
                    {
                        if (dal.ParmanentDeleteShoppingCartAndProducts())
                        {
                            ViewBag.OrderNumber = dal.GenerateOrderNumber();
                            return View("_LayoutCheckOut");
                        }
                        //cant delete from shopping cart
                    }
                    //cant add to purchasedetails
                }
                //invalid credit card
            }
            catch (Exception)
            {
                return View();
            }
            return View();
        }

        public ActionResult GetRecentPurchases()
        {
            List<Models.Purchase> listPurchasedItems = new List<Models.Purchase>();
            EKartMapper<PurchaseDetail, Models.Purchase> mapobj = new EKartMapper<PurchaseDetail, Models.Purchase>();
            EKartRepository dal = new EKartRepository();
            string email = Session["email"] + "";
            var lstEntityProducts = dal.RecentPurchasesUsingLinq(email);
            foreach (var product in lstEntityProducts)
            {
                listPurchasedItems.Add(mapobj.Translate(product));
            }
            return View(listPurchasedItems);
        }

        public ActionResult RemovePurchaseDetail(long id)
        {
            EKartRepository dal = new EKartRepository();
            bool status = dal.DeletePurchaseDetails(id);
            if (status)
            {
                return Redirect("/Purchase/GetRecentPurchases");
            }
            return View();
        }

        public ActionResult OrderDetails(long id)
        {
            List<Models.Purchase> listPurchasedItems = new List<Models.Purchase>();
            EKartMapper<PurchaseDetail, Models.Purchase> mapobj = new EKartMapper<PurchaseDetail, Models.Purchase>();
            EKartRepository dal = new EKartRepository();
            var lstEntityProducts = dal.GetPurchaseDetails(id);
            foreach (var product in lstEntityProducts)
            {
                listPurchasedItems.Add(mapobj.Translate(product));
            }
            return View(listPurchasedItems);
        }
    }
}