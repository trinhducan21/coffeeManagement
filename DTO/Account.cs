using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeManagement.DTO
{
    public class Account
    {
        public Account(string userName, string displayName, int type, string passWord = null)
        {
            this.UserName = userName;
            this.DisplayName = displayName;
            this.Type = type;
            this.Password = passWord;
        }

        public Account(DataRow row)
        {
            this.UserName = row["userName"].ToString();
            this.DisplayName = row["displayName"].ToString();
            this.Type = (int)row["type"];
            this.Password = row["passWord"].ToString();
        }

        private string userName;
        private string displayName;
        private int type;
        private string passWord;

        public string UserName { get; private set; }
        public string DisplayName { get; private set; }
        public int Type { get; private set; }
        public string Password { get; private set; }
    }
}
