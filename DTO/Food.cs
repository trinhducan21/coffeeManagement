using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeManagement.DTO
{
    public class Food
    {
        public Food(int id, string name, int idCategory, float price)
        {
            this.ID = id;
            this.Name = name;
            this.IdCategory = idCategory;
            this.Price = price;
        }

        public Food(DataRow row)
        {
            this.ID = (int)row["id"];
            this.Name = row["name"].ToString();
            this.IdCategory = (int)row["idCategory"];
            this.Price = (float)Convert.ToDouble(row["price"].ToString());
        }

        private int id;
        private string name;
        private int idCategory;
        private float price;

        public int ID { get; private set; }
        public string Name { get; private set; }
        public int IdCategory { get; private set; }
        public float Price { get; private set; }
    }
}
