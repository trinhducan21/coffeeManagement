using CoffeeManagement.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeManagement.DAO
{
    public class BillInfoDAO
    {
        private static BillInfoDAO instance;

        public static BillInfoDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new BillInfoDAO();
                return BillInfoDAO.instance;
            }
            private set
            {
                BillInfoDAO.instance = value;
            }
        }

        private BillInfoDAO() { }


        public void InsertBillInfo(int idBill, int idFood, int count)
        {
            string query = "EXEC USP_InsertBillInfo @idBill , @idFood , @count";
            DataProvider.Instance.ExecuteNonQuery(query, new object[] { idBill, idFood, count });
        }

        public void DeleteBillInfo(int idBill, int idFood)
        {
            string query = "DELETE BillInfo WHERE idBill = " + idBill + " AND idFood = " + idFood;
            DataProvider.Instance.ExecuteNonQuery(query);
        }
    }
}
