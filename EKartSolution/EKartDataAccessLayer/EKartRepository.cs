using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;

namespace EKartDataAccessLayer
{
    public class EKartRepository
    {
        private EKartDBContext Context;
        private User currentUser;
        public EKartRepository()
        {
            Context = new EKartDBContext();
        }

        //returns current user
        public User GetCurrentUser() {
            return currentUser;
        }

        //User registration
        public bool RegisterNewUser(string firstName, string LastName, string emailId, string password, string phoneNumber, string zipcode, string address, string city, string state, string country)
        {
            bool status = false;
            User newUser = new User();
            try
            {
                newUser.FirstName = firstName;
                newUser.LastName = LastName;
                newUser.EmailId = emailId;
                newUser.AddressLine = address;
                newUser.UserPassword = password;
                newUser.State = state;
                newUser.City = city;
                newUser.PhoneNumber = phoneNumber;
                newUser.ZipCode = zipcode;
                newUser.RoleId = 2;
                newUser.Country = country;
                try
                {
                    Context.Users.Add(newUser); //Adding to db
                    Context.SaveChanges(); //Saving to db model
                    status = true;
                }
                catch (Exception)
                {
                    status = false;
                }
            }
            catch (Exception)
            {
                status = false;
            }
            return status;
        }

        //Validate User Login credentials
        public bool CheckLoginCredentials(string emailId, string password)
        {
            bool status = false;
            try
            {
                //retrieves user from db model
                var user = Context.Users.Where(db => db.EmailId == emailId && db.UserPassword == password).Select(db => db).FirstOrDefault();
                if (user != null)
                {
                    currentUser = user;
                    status = true;
                }
            }
            catch (Exception)
            {
                status = false;
            }
            return status;
        }

        //Validate users email address for password reset
        public bool ForgotPassword(string emailId)
        {
            try
            {
                var user = Context.Users.Where(db => db.EmailId == emailId).Select(db => db).FirstOrDefault();
                if (user != null)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //Reset Password and saves it to db
        public string ResetPassword(string email)
        {
            Random random = new Random();
            string generatedPassword = null;
            var validUser = Context.Users.Where(db => db.EmailId == email).Select(db => db).FirstOrDefault(); //retrive user from db if exists
            Console.WriteLine();
            try
            {
                if (validUser != null)
                {
                    generatedPassword = "User@" + random.Next(10) + random.Next(10) + random.Next(10) + random.Next(10);
                    validUser.UserPassword = generatedPassword;
                    Context.SaveChanges();
                    return generatedPassword;
                }
                else
                {
                    validUser.UserPassword = "User@1000";
                    Context.SaveChanges();
                }

            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine(ex.Message);
                validUser.UserPassword = "User@1000";
                Context.SaveChanges();
            }
            return generatedPassword;
        }

        //Change the Password and saves it to db
        public bool ChangePassword(string email, string oldPassword, string newPassword, string confirmNewPassword)
        {
            Regex passwordFormat = new Regex((@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,20}$"));
            bool passwordChanged = false;
            var userFound = Context.Users.Where(db => db.EmailId == email).Select(db => db).FirstOrDefault();
            if (userFound != null)
            {
                if (userFound.UserPassword.Equals(oldPassword) && newPassword.Equals(confirmNewPassword) && passwordFormat.IsMatch(newPassword) && !newPassword.Equals(oldPassword))
                {
                    userFound.UserPassword = newPassword;
                    Context.SaveChanges();
                    passwordChanged = true;
                }
                else
                    passwordChanged = false;
            }
            return passwordChanged;
        }
        //returns a list of all products in Products table in db
        public List<Product> FetchProductUsingLinq()
        {
            return Context.Products.ToList<Product>();
        }

        //returns a list of all category in Category table in db
        public List<Category> FetchCategoryUsingLinq()
        {
            return Context.Categories.ToList<Category>();
        }

        //returns a list of all products in Products table in db with specific category id
        public List<Product> GetSpecificCategory(int id)
        {
            List<Product> listProductsInSpecificCategory = new List<Product>();
            listProductsInSpecificCategory = Context.Products.Where(db => db.CategoryId == id).Select(db => db).ToList<Product>();
            return listProductsInSpecificCategory;
        }

        //Retrieves product details
        public List<Product> GetProductDetails(string id)
        {
            List<Product> listProductDetails = new List<Product>();
            listProductDetails = Context.Products.Where(db => db.ProductId == id).Select(db => db).ToList<Product>();
            return listProductDetails;
        }
        
        //Retrieves recent purchases for a user with specified email address
        public List<PurchaseDetail> RecentPurchasesUsingLinq(string email)
        {
            return Context.PurchaseDetails.Where(db => db.EmailId == email).Select(db => db).ToList<PurchaseDetail>();     
        }

        //Returns a List of items in a shopping cart
        public List<ShoppingCart> GetProductsInCartUsingLinq()
        {
            return Context.ShoppingCarts.ToList<ShoppingCart>();
        }

        //Adds Product to Cart
        public bool AddToShoppingCart(string id)
        {
            ShoppingCart cartItem = new ShoppingCart();
            bool status = false;
            try
            {
                var item = Context.Products.Where(db => db.ProductId == id).Select(db => db).FirstOrDefault();
                var itemFoundInCart = Context.ShoppingCarts.Where(db => db.ProductId == id).Select(db => db).FirstOrDefault();
                if (item.QuantityAvailable > 0)
                {
                    if (itemFoundInCart == null)
                    {
                        cartItem.CategoryId = Convert.ToByte(item.CategoryId);
                        cartItem.ProductId = item.ProductId;
                        cartItem.ProductName = item.ProductName;
                        cartItem.Quantity = 1;
                        item.QuantityAvailable -= 1; //Decrease quantity available by default - 1
                        cartItem.Price = item.Price;
                        cartItem.PurchaseDate = DateTime.Now;
                        Context.ShoppingCarts.Add(cartItem);
                        Context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        //if item is found in shopping cart increase the quantity
                        itemFoundInCart.Quantity += 1;
                        item.QuantityAvailable -= 1;
                        Context.SaveChanges();
                        return true;
                    }
                }
                else
                {
                    status = false;
                }
            }
            catch (Exception)
            {
                status = false;
            }
            return status;
        }

        public bool UpdateCustomerDetails(string emailId, string firstName, string LastName, string phoneNumber, string zipcode, string address, string city, string state, string country)
        {
            bool status = false;
            try
            {
                var user = Context.Users.Where(db => db.EmailId == emailId).Select(db => db).FirstOrDefault();
                if (user != null)
                {
                    user.FirstName = firstName;
                    user.LastName = LastName;
                    user.AddressLine = address;
                    user.State = state;
                    user.City = city;
                    user.PhoneNumber = phoneNumber;
                    user.ZipCode = zipcode;
                    user.Country = country;
                    try
                    {
                        Context.SaveChanges();
                        status = true;
                    }
                    catch (Exception)
                    {
                        status = false;
                    }
                }
                else
                    status = false;
            }
            catch (Exception)
            {
                status = false;
            }
            return status;
        }

        //Update quantity of product in Cart
        public bool UpdateQuantity(string id)
        {
            bool status = false;
            try
            {
                var item = Context.Products.Where(db => db.ProductId == id).Select(db => db).FirstOrDefault();
                var productInCart = Context.ShoppingCarts.Where(db => db.ProductId == id).Select(db => db).FirstOrDefault();
                if (productInCart != null && item.QuantityAvailable > 0)
                {
                    item.QuantityAvailable -= 1;
                    productInCart.Quantity += 1;
                    Context.SaveChanges();
                    status = true;
                }
                else
                {
                    status = false;
                }

            }
            catch (Exception)
            {
                status = false;
            }
            return status;
        }

        //Removes product from cart
        public bool DeleteProductFromCart(string id)
        {
            bool status = false;
            try
            {
                var item = Context.Products.Where(db => db.ProductId == id).Select(db => db).FirstOrDefault();
                var removeProductFromCart = Context.ShoppingCarts.Where(db => db.ProductId == id).Select(db => db).FirstOrDefault();
                if (removeProductFromCart != null)
                {
                    item.QuantityAvailable += removeProductFromCart.Quantity; //Restores original product quantity
                    Context.ShoppingCarts.Remove(removeProductFromCart);
                    Context.SaveChanges();
                    status = true;
                }
                else
                    status = false;
            }
            catch (Exception)
            {
                status = false;
            }
            return status;
        }

        public bool AddCreditCardDetails(string email, string cardNumber, string nameOnCard, string cardType, decimal CVVNumber, DateTime expiryDate)
        {
            CardDetail creditCard = new CardDetail();
            try
            {
                creditCard.EmailId = email;
                creditCard.CardNumber = cardNumber;
                creditCard.NameOnCard = nameOnCard;
                creditCard.CardType = cardType;
                creditCard.ExpiryDate = expiryDate;
                creditCard.CVVNumber = CVVNumber;
                creditCard.Balance = Convert.ToDecimal(NextDouble(new Random(), 2000000, 700000));                   
                Context.CardDetails.Add(creditCard);
                Context.SaveChanges();
                return true;
            }
            catch (SqlException)
            {
                return false;
            }
        }

        //Empties cart when customer log out of account
        public bool RemoveShoppingCartContent()
        {
            try
            {
                var listItemsInCart = Context.ShoppingCarts.ToList<ShoppingCart>();
                if (listItemsInCart == null)
                {
                    return true;
                }
                else
                {
                    foreach (var item in listItemsInCart)
                    {
                        DeleteProductFromCart(item.ProductId);
                    }
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        //Generate balance for added card details
        public double NextDouble(Random rnd, double min, double max)
        {
            return rnd.NextDouble() * (max - min) + min;
        }

        //Retrieve total amount of product in cart
        public decimal TotalPriceOfCart()
        {
            decimal totalPrice = 0;
            try
            {
                var listItemsInCart = Context.ShoppingCarts.ToList();
                foreach(var item in listItemsInCart)
                {
                    Console.WriteLine(item.Price);
                    totalPrice = totalPrice + (item.Price *  item.Quantity);
                }
            }
            catch (SqlException)
            {
                totalPrice = 0;
            }
            return totalPrice;
        }

        public int AddToPurchaseDetails(string email)
        {
            EKartRepository dal = new EKartRepository();
            PurchaseDetail purchase = new PurchaseDetail();
            var itemsInCart = GetProductsInCartUsingLinq();
            if (itemsInCart != null)
            {
                foreach (var item in itemsInCart)
                {
                    purchase.EmailId = email;
                    purchase.ProductId = item.ProductId;
                    purchase.QuantityPurchased = Convert.ToInt16(item.Quantity);
                    purchase.DateOfPurchase = item.PurchaseDate;
                    purchase.ProductName = item.ProductName;
                    Context.PurchaseDetails.Add(purchase);
                    Context.SaveChanges();
                    return 1;
                }
            }
            return -1;
        }

        //Overloading the method above
        public List<PurchaseDetail> GetPurchaseDetails(long purchaseId)
        {
            List<PurchaseDetail> listProductDetails = new List<PurchaseDetail>();
            listProductDetails = Context.PurchaseDetails.Where(db => db.PurchaseId == purchaseId).Select(db => db).ToList<PurchaseDetail>();
            return listProductDetails;
        }

        //Removes from PurchaseDetails using the purchaseId
        public bool DeletePurchaseDetails(long id)
        {
            var purchaseDetail = Context.PurchaseDetails.Where(db => db.PurchaseId == id).Select(db => db).FirstOrDefault<PurchaseDetail>();
            if(purchaseDetail != null)
            {
                Context.PurchaseDetails.Remove(purchaseDetail);
                Context.SaveChanges();
                return true;
            }
            return false;
        }

        //Returns balance on the card
        public decimal GetCardBalance(string cardNumber)
        {
            decimal balance = 0;
            var cardDetails = Context.CardDetails.Where(db => db.CardNumber == cardNumber).Select(db => db).FirstOrDefault<CardDetail>();
            if(cardDetails != null)
            {
                balance = Convert.ToDecimal(cardDetails.Balance);
            }
            return balance;
        }

        //Update the balance on the credit card
        public bool UpdateCardBalance(string creditCardNumber, decimal amount)
        {
            var cardDetails = Context.CardDetails.Where(db => db.CardNumber == creditCardNumber).Select(db => db).FirstOrDefault<CardDetail>();
            if (cardDetails != null)
            {
                cardDetails.Balance = Convert.ToDecimal(cardDetails.Balance) - amount;
                Context.SaveChanges();
                return true;
            }
            return false;
        }

        //Delete Permanently from Cart and Products Table/Entity
        public bool ParmanentDeleteShoppingCartAndProducts()
        {
            try
            {
                var listItemsInCart = Context.ShoppingCarts.ToList<ShoppingCart>();
                if (listItemsInCart == null)
                {
                    return true;
                }
                else
                {
                    foreach (var item in listItemsInCart)
                    {
                        Context.ShoppingCarts.Remove(item);
                        Context.SaveChanges();
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }
        
        public string GenerateOrderNumber()
        {
            Random random = new Random();
            return "OR-" + DateTime.Now.ToShortDateString() + "/03-" + random.Next(1, 10).ToString() + random.Next(1, 10).ToString() + random.Next(1, 10).ToString() + random.Next(1, 10).ToString();
        }

        public List<CardDetail> RetrieveCustomersCard(string email)
        {
            return Context.CardDetails.Where(db => db.EmailId == email).ToList<CardDetail>();
        }

        public bool Transaction(string emailId, string cardNumber, string nameOnCard, string cardType, decimal CVVNumber, DateTime expiryDate)
        {
            var creditCard = Context.CardDetails.Where(db => db.CardNumber == cardNumber).Select(db => db).FirstOrDefault<CardDetail>();
            if(creditCard == null)
            {
               bool status = AddCreditCardDetails(emailId, cardNumber, nameOnCard, cardType, CVVNumber, expiryDate);
                if (status)
                {
                    var card = Context.CardDetails.Where(db => db.CardNumber == cardNumber).Select(db => db).FirstOrDefault<CardDetail>();
                    if(card.Balance >= TotalPriceOfCart())
                    {
                        card.Balance = card.Balance - TotalPriceOfCart();
                        Context.SaveChanges();
                        return true;
                    }
                    else
                        return false;
                }
                return false;
            }
            else
            {
                if (creditCard.Balance >= TotalPriceOfCart())
                {
                    creditCard.Balance = creditCard.Balance - TotalPriceOfCart();
                    Context.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public List<CardDetail> CardDetails(string cardNumber)
        {
            return Context.CardDetails.Where(db => db.CardNumber == cardNumber).ToList<CardDetail>();
        }
    }
}
