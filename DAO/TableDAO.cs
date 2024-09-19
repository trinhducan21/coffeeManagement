using CoffeeManagement.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeManagement.DAO
{
    public class TableDAO
    {
        private static TableDAO instance;

        public static TableDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new TableDAO();
                return TableDAO.instance;
            }
            private set
            {
                TableDAO.instance = value;
            }
        }

        private TableDAO() { }

        public static int TableWidth = 90;
        public static int TableHeight = 90;

        public List<Table> LoadTableList()
        {
            List<Table> tableList = new List<Table>();

            DataTable data = DataProvider.Instance.ExecuteQuery("USP_GetTableList");

            foreach (DataRow item in data.Rows)
            {
                Table table = new Table(item);
                tableList.Add(table);
            }

            return tableList;
        }

        public DataTable GetDataTableTable()
        {
            string query = "SELECT id AS [ID], name AS [Name], status AS [Status] FROM TableFood";
            return DataProvider.Instance.ExecuteQuery(query);
        }

        public bool CheckTableName(string name)
        {
            string query = string.Format("SELECT * FROM TableFood WHERE name = N'{0}'", name);
            DataTable result = DataProvider.Instance.ExecuteQuery(query);

            return result.Rows.Count > 0;
        }

        public bool InsertTable(string name)
        {
            string query = string.Format("INSERT TableFood (name) VALUES (N'{0}')", name);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool DeleteTable(int id)
        {
            string query = string.Format("DELETE TableFood WHERE id = {0}", id);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool UpdateTable(int id, string name)
        {
            string query = string.Format("UPDATE TableFood SET name = N'{0}' WHERE id = {1}", name, id);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool ChangeTableStatus(int id, string status)
        {
            string query = string.Format("UPDATE TableFood SET status = N'{0}' WHERE id = {1}", status, id);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }
    }
}
