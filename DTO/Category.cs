using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeManagement.DTO
{
    public class Category
    {
        public Category(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public Category(DataRow row)
        {
            this.Id = (int)row["id"];
            this.Name = row["name"].ToString();
        }

        private int id;
        private string name;

        public int Id { get; private set; }
        public string Name { get; private set; }
    }
}
