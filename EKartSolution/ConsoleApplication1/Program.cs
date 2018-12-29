using EKartDataAccessLayer;
using System;

namespace ConsoleApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            EKartRepository dal = new EKartRepository();
            var items = dal.RetrieveCustomersCard("leon@gmail.com");
            foreach(var item in items)
            {
                Console.WriteLine(item.EmailId);
                Console.WriteLine(item.CardNumber);
                Console.WriteLine(item.CVVNumber);
            }
        }
    }
}
