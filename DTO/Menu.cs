using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeManagement.DTO
{
    public class Menu
    {
        public Menu(string FoodName, int Count, float Price, float TotalPrice)
        {
            this.FoodName = FoodName;
            this.Count = Count;
            this.Price = Price;
            this.TotalPrice = TotalPrice;
        }

        public Menu(DataRow dataRow)
        {
            this.FoodName = dataRow["name"].ToString();
            this.Count = (int)dataRow["count"];
            this.Price = (float)Convert.ToDouble(dataRow["price"].ToString());
            this.TotalPrice = (float)Convert.ToDouble(dataRow["totalPrice"].ToString());
        }

        public string FoodName { get; private set; }
        public int Count { get; private set; }
        public float Price { get; private set; }
        public float TotalPrice { get; private set; }
    }
}
