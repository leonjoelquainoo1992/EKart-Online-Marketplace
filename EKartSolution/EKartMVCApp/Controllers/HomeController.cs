using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EKartMVCApp.Controllers
{
    public class HomeController : Controller
    { 
        //Not Implemented
        public ActionResult GiveFeedBack()
        {
            return View();
        }

        //Generates coupons for each category
        public JsonResult GetCoupons()
        {
            Random random = new Random();
            Dictionary<string, string> data = new Dictionary<string, string>();
            string[] value = new String[5];
            string[] key = { "Arts", "Electronics", "Fashion", "Home", "Toys" };
            for (int i = 0; i < 5; i++)
            {
                string number = "RUSH" + random.Next(1, 10).ToString() + random.Next(1, 10).ToString() + random.Next(1, 10).ToString();
                value[i] = number;
            }
            for (int i = 0; i < 5; i++)
            {
                data.Add(key[i], value[i]);
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Contact(Models.Contact contact)
        {
            if (ModelState.IsValid)
            {
                return View("_LayoutMessageSents");
            }
            return View("Contact");
        }

        public FilePathResult DownloadTermsAndConditions()
        {
            return File(@"..\Downloads\TermsAndConditions.pdf", "pdf");
        }

        //Still Under Construction
        public ActionResult FAQ()
        {
            return View();
        }
    }
}