using EKartDataAccessLayer;
using System;
using System.Web;
using System.Web.Mvc;

namespace EKartMVCApp.Controllers
{
    public class UserController : Controller
    {
        public ActionResult RegisterNewuser(Models.Users model)
        {
            try
            {
                EKartRepository dal = new EKartRepository();
                bool status = dal.RegisterNewUser(model.FirstName, model.LastName, model.EmailId, model.UserPassword, model.PhoneNumber, model.ZipCode, model.AddressLine1, model.City, model.State, model.Country);
                if (status)
                {
                    return Redirect("/User/LogIn");
                }
                return View();
            }
            catch (Exception)
            {
                return View("RegistrationError");
            }
        }

        public ActionResult LogIn(Models.Login models)
        {
            try
            {
                EKartRepository dal = new EKartRepository();
                string checkbox = models.RememberMe;
                string userId = models.EmailId;
                string password = models.UserPassword;
                if (checkbox != "false")
                {
                    HttpCookie cookieObj = new HttpCookie("User");
                    cookieObj.Values.Add("UserId", userId);
                    cookieObj.Values.Add("Password", password);
                    cookieObj.Expires = DateTime.Now.AddDays(1);
                    Response.Cookies.Add(cookieObj);
                }
                var status = dal.CheckLoginCredentials(models.EmailId, models.UserPassword);
                if (status)
                {
                    Session["username"] = dal.GetCurrentUser().LastName;
                    Session["email"] = dal.GetCurrentUser().EmailId;
                    Session["firstName"] = dal.GetCurrentUser().FirstName;
                    Session["lastName"] = dal.GetCurrentUser().LastName;
                    Session["phone"] = dal.GetCurrentUser().PhoneNumber;
                    return Redirect("/Category/ViewCategories?username=" + Session["username"]);
                }
                else
                    return View("LogIn");
            }
            catch (Exception)
            {
                return View("ErrorPage");
            }
        }

        public ActionResult ForgetPassword(Models.Users model) 
        {
            EKartRepository dal = new EKartRepository();
            try
            {
                string email = model.EmailId;
                bool status = dal.ForgotPassword(email);
                if (status)
                {
                    TempData["Password"] = dal.ResetPassword(email);
                    return Redirect(Request.UrlReferrer.PathAndQuery);
                }
                else
                    return View();

            }
            catch (Exception)
            {
                return View("ErrorPage");
            }
        }
    }
}
