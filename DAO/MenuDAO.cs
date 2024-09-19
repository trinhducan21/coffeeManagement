using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeManagement.DAO
{
    public class MenuDAO
    {
        private static MenuDAO instance;

        public static MenuDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new MenuDAO();
                return MenuDAO.instance;
            }
            private set
            {
                MenuDAO.instance = value;
            }
        }

        private MenuDAO() { }

        public List<DTO.Menu> GetListMenuByTable(int id)
        {
            List<DTO.Menu> listMenu = new List<DTO.Menu>();

            string query = "SELECT f.name, bi.count, f.price, f.price*bi.count AS totalPrice FROM dbo.Bill AS b, dbo.BillInfo AS bi, dbo.Food AS f WHERE b.id = bi.idBill AND bi.idFood = f.id AND b.status = 0 AND b.idTable = " + id;

            System.Data.DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {
                DTO.Menu menu = new DTO.Menu(item);
                listMenu.Add(menu);
            }

            return listMenu;
        }
    }
}
