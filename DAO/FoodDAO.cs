using CoffeeManagement.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeManagement.DAO
{
    public class FoodDAO
    {
        private static FoodDAO instance;

        public static FoodDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FoodDAO();
                }
                return FoodDAO.instance;
            }
            private set
            {
                FoodDAO.instance = value;
            }
        }

        private FoodDAO() { }

        public List<Food> GetFoodByCategoryID(int id)
        {
            List<Food> list = new List<Food>();

            string query = "SELECT * FROM Food WHERE idCategory = " + id;

            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {
                Food food = new Food(item);
                list.Add(food);
            }

            return list;
        }

        public List<Food> GetListFood()
        {
            List<Food> list = new List<Food>();

            string query = "SELECT * FROM Food";

            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {
                Food food = new Food(item);
                list.Add(food);
            }

            return list;
        }

        public List<Food> SearchFoodByName(string name)
        {
            List<Food> list = new List<Food>();

            string query = string.Format("SELECT * FROM Food WHERE name LIKE N'%{0}%'", name);

            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {
                Food food = new Food(item);
                list.Add(food);
            }

            return list;
        }

        public bool checkFoodName(string name)
        {
            string query = string.Format("SELECT * FROM Food WHERE name = N'{0}'", name);
            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            return data.Rows.Count > 0;
        }

        public bool InsertFood(string name, int id, float price)
        {
            string query = string.Format("INSERT Food (name, idCategory, price) VALUES (N'{0}', {1}, {2})", name, id, price);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }

        public int UpdateFood(int idFood, string name, int id, float price)
        {
            string query = string.Format("EXEC USP_UpdateFood {0}, N'{1}', {2}, {3}", idFood, name, id, price);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result;
        }

        public int DeleteFood(int idFood)
        {
            string query = string.Format("EXEC USP_DeleteFood {0}", idFood);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result;
        }

        public int GetIDFoodByName(string name)
        {
            string query = string.Format("SELECT id FROM Food WHERE name = N'{0}'", name);
            return (int)DataProvider.Instance.ExecuteScalar(query);
        }
    }
}
