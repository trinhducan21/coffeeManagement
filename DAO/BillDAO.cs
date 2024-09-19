using CoffeeManagement.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeManagement.DAO
{
    public class BillDAO
    {
        private static BillDAO instance;

        public static BillDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new BillDAO();
                return BillDAO.instance;
            }
            private set
            {
                BillDAO.instance = value;
            }
        }

        private BillDAO() { }

        public int GetUncheckBillIDByTableID(int id)
        {
            string query = "SELECT * FROM dbo.Bill WHERE idTable = " + id + " AND status = 0";

            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            if (data.Rows.Count > 0)
            {
                Bill bill = new Bill(data.Rows[0]);
                return bill.ID;
            }

            return -1;
        }

        public int GetDiscountByID(int id)
        {
            string query = "SELECT discount FROM dbo.Bill WHERE id = " + id + " AND status = 0";
            return (int)DataProvider.Instance.ExecuteScalar(query);
        }

        public void InsertBill(int id)
        {
            string query = "EXEC USP_InsertBill @idTable";
            DataProvider.Instance.ExecuteNonQuery(query, new object[] { id });
        }

        public int GetMaxIDBill()
        {
            try
            {
                return (int)DataProvider.Instance.ExecuteScalar("SELECT MAX(id) FROM dbo.Bill");
            }
            catch
            {
                return 1;
            }
        }

        public void UpdateDiscount(int id, int discount)
        {
            string query = "UPDATE dbo.Bill SET discount = " + discount + " WHERE id = " + id;
            DataProvider.Instance.ExecuteNonQuery(query);
        }

        public void CheckOut(int id, float totalPrice)
        {
            string query = "UPDATE dbo.Bill SET dateCheckOut = GETDATE(), status = 1, " + " totalPrice = " + totalPrice + " WHERE id = " + id;
            DataProvider.Instance.ExecuteNonQuery(query);
        }

        public DataTable GetBillListByDate(DateTime checkIn, DateTime checkOut)
        {
            return DataProvider.Instance.ExecuteQuery("EXEC USP_GetListBillByDate @checkIn , @checkOut", new object[] { checkIn, checkOut });
        }

        public DataTable GetBillListByDateAndPage(DateTime checkIn, DateTime checkOut, int pageNum)
        {
            string query = "EXEC USP_GetListBillByDateAndPage @checkIn , @checkOut , @pageNum";
            return DataProvider.Instance.ExecuteQuery(query, new object[] { checkIn, checkOut, pageNum });
        }

        public int GetNumBillListByDate(DateTime checkIn, DateTime checkOut)
        {
            string query = "EXEC USP_GetNumBillByDate @checkIn , @checkOut";
            return (int)DataProvider.Instance.ExecuteScalar(query, new object[] { checkIn, checkOut });
        }
    }
}
