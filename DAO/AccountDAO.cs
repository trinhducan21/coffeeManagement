using CoffeeManagement.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeManagement.DAO
{
    public class AccountDAO
    {
        private static AccountDAO instance;

        public static AccountDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AccountDAO();
                }
                return AccountDAO.instance;
            }
            private set
            {
                AccountDAO.instance = value;
            }
        }

        private AccountDAO() { }

        public bool Login(string userName, string passWord)
        {
            byte[] temp = ASCIIEncoding.ASCII.GetBytes(passWord);
            byte[] hashData = new MD5CryptoServiceProvider().ComputeHash(temp);

            string hashPass = "";

            foreach (byte item in hashData)
            {
                hashPass += item;
            }

            // var list = hashData.ToString();
            // list.Reverse();

            string query = "USP_Login @userName , @passWord";

            DataTable result = DataProvider.Instance.ExecuteQuery(query, new object[] { userName, hashPass });

            return result.Rows.Count > 0;
        }

        public Account getAccountByUserName(string userName)
        {
            string query = "SELECT * FROM Account WHERE userName = '" + userName + "'";

            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {
                return new Account(item);
            }

            return null;
        }

        public DataTable GetListAccount()
        {
            string query = "SELECT userName, displayName, type FROM Account";

            return DataProvider.Instance.ExecuteQuery(query);
        }

        public bool UpdateAccount(string userName, string displayName, string pass, string newPass)
        {
            byte [] temp = ASCIIEncoding.ASCII.GetBytes(pass);
            byte [] hashData = new MD5CryptoServiceProvider().ComputeHash(temp);

            string hashPass = "";
            foreach (byte item in hashData)
            {
                hashPass += item;
            }

            byte[] newTemp = ASCIIEncoding.ASCII.GetBytes(newPass);
            byte[] newHashData = new MD5CryptoServiceProvider().ComputeHash(newTemp);

            string newHashPass = "";
            foreach (byte item in newHashData)
            {
                newHashPass += item;
            }

            string query = string.Format("EXEC USP_UpdateAccount @userName = N'{0}', @displayName = N'{1}', @password = N'{2}', @newPassword = N'{3}'", userName, displayName, hashPass, newHashPass);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool checkAccountName(string userName)
        {
            string query = string.Format("SELECT * FROM Account WHERE userName = N'{0}'", userName);
            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            return data.Rows.Count > 0;
        }

        public bool InsertAccount(string userName, string displayName, int type)
        {
            string query = string.Format("INSERT Account (userName, displayName, type) VALUES (N'{0}', N'{1}', {2})", userName, displayName, type);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool EditAccount(string userName, string displayName, int type)
        {
            string query = string.Format("UPDATE Account SET displayName = N'{1}', type = {2} WHERE userName = N'{0}'", userName, displayName, type);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool DeleteAccount(string userName)
        {
            string query = string.Format("DELETE Account WHERE userName = N'{0}'", userName);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool ResetPassword(string userName)
        {
            string query = string.Format("UPDATE Account SET password = N'18833213210117723916811824913021616923162239' WHERE userName = N'{0}'", userName);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }
    }
}

