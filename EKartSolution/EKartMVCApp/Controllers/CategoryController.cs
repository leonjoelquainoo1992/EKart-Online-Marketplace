using EKartDataAccessLayer;
using EKartMVCApp.Repository;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EKartMVCApp.Controllers
{
    public class CategoryController : Controller
    {
        public ActionResult ViewCategories()
        {
            List<Models.Category> lstModelCategories = new List<Models.Category>();
            EKartMapper<Category, Models.Category> mapobj = new EKartMapper<Category, Models.Category>();
            EKartRepository dal = new EKartRepository();
            var lstEntityCategories = dal.FetchCategoryUsingLinq();
            foreach (var category in lstEntityCategories)
            {
                lstModelCategories.Add(mapobj.Translate(category));
            }
            return View(lstModelCategories);
        }
        
        public ActionResult CategoryDetails(int id)
        {
            List<Models.Product> lstModelProducts = new List<Models.Product>();
            EKartMapper<Product, Models.Product> mapobj = new EKartMapper<Product, Models.Product>();
            EKartRepository dal = new EKartRepository();
            var lstEntityProducts = dal.GetSpecificCategory(id);
            foreach (var product in lstEntityProducts)
            {
                lstModelProducts.Add(mapobj.Translate(product));
            }
            return View(lstModelProducts);
        }
    }
}
