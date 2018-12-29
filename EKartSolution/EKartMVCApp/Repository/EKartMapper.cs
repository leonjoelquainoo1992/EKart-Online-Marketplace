using AutoMapper;
using EKartDataAccessLayer;
namespace EKartMVCApp.Repository
{
    public class EKartMapper<Source, Destination>
        where Source : class
        where Destination : class
    {
        public EKartMapper()
        {
            //Model -> Database
            Mapper.CreateMap<Models.Product, Product>();
            Mapper.CreateMap<Models.Category, Category>();
            Mapper.CreateMap<PurchaseDetail, Models.Purchase>();
            Mapper.CreateMap<ShoppingCart, Models.ShoppingCart>();
            Mapper.CreateMap<CardDetail, Models.CreditCard>();

            //Database -> Model
            Mapper.CreateMap<Product, Models.Product>();
            Mapper.CreateMap<Category, Models.Category>();
            Mapper.CreateMap<Models.Purchase, PurchaseDetail>();
            Mapper.CreateMap<Models.ShoppingCart, ShoppingCart>();
            Mapper.CreateMap<Models.CreditCard, CardDetail>();
        }

        public Destination Translate(Source obj)
        {
            return Mapper.Map<Source, Destination>(obj);
        }
    }
}