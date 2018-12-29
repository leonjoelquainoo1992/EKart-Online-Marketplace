using EKartDataAccessLayer;
using EKartMVCApp.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace EKartMVCApp.Controllers
{
    public class ProductController : Controller
    {
        public ActionResult ViewProducts()
        {
            List<Models.Product> lstModelProducts = new List<Models.Product>();
            EKartMapper<Product, Models.Product> mapobj = new EKartMapper<Product, Models.Product>();
            EKartRepository dal = new EKartRepository();
            var lstEntityProducts = dal.FetchProductUsingLinq();
            foreach (var product in lstEntityProducts)
            {
                lstModelProducts.Add(mapobj.Translate(product));
            }
            return View(lstModelProducts);
        }

        public ActionResult ProductDetails(string id)
        {
            List<Models.Product> lstProductsDetails = new List<Models.Product>();
            EKartMapper<Product, Models.Product> mapobj = new EKartMapper<Product, Models.Product>();
            EKartRepository dal = new EKartRepository();
            var lstEntityProducts = dal.GetProductDetails(id);
            foreach (var product in lstEntityProducts)
            {
                lstProductsDetails.Add(mapobj.Translate(product));
            }
            return View(lstProductsDetails);
        }

        public ActionResult AddToCart(string id)
        {
            EKartRepository dal = new EKartRepository();
            bool status = dal.AddToShoppingCart(id);
            if (status)
            {
                return Redirect("/Product/GetCart");
            }
            return View("_LayoutQuantityExceeded");
        }

        public ActionResult GetCart()
        {
            List<Models.ShoppingCart> lstAllProductsInCart = new List<Models.ShoppingCart>();
            EKartMapper<ShoppingCart, Models.ShoppingCart> mapobj = new EKartMapper<ShoppingCart, Models.ShoppingCart>();
            EKartRepository dal = new EKartRepository();
            var lstEntityProducts = dal.GetProductsInCartUsingLinq();
            ViewBag.TotalPrice = dal.TotalPriceOfCart();
            foreach (var product in lstEntityProducts)
            {
                lstAllProductsInCart.Add(mapobj.Translate(product));
            }
            return View(lstAllProductsInCart);
        }

        public ActionResult DeleteFromCart(string id)
        {
            EKartRepository dal = new EKartRepository();
            bool status = dal.DeleteProductFromCart(id);
            if (status)
            {
                return Redirect("/Product/GetCart");
            }
            return View();
        }

        public ActionResult EditQuantity(string id)
        {
            EKartRepository dal = new EKartRepository();
            bool status = dal.UpdateQuantity(id);
            if (status)
            {
                return Redirect("/Product/GetCart");
            }
            return View("_LayoutQuantityExceeded");
        }

        public ActionResult SearchProduct(byte? categoryId)
        {
            EKartRepository dal = new EKartRepository();
            ViewBag.CategoryList = dal.FetchCategoryUsingLinq();
            if (categoryId != null)
            {
                Session["categoryId"] = categoryId;
            }
            else
            {
                categoryId = Convert.ToByte(Session["categoryId"]);
            }
            ViewBag.SelectedCategory = dal.FetchCategoryUsingLinq().Where(x => x.CategoryId == categoryId).Select(x => x.CategoryName).FirstOrDefault();
            if (ViewBag.SelectedCategory == null)
                ViewBag.SelectedCategory = "--Select--";
            var productList = dal.FetchProductUsingLinq();
            var mapObj = new EKartMapper<Product, Models.Product>();
            var products = new List<Models.Product>();
            foreach (var product in productList)
            {
                products.Add(mapObj.Translate(product));
            }
            var filteredProducts = products.Where(model => model.CategoryId == categoryId);
            return View(filteredProducts);
        }
    }
}
