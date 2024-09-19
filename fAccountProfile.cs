using CoffeeManagement.DAO;
using CoffeeManagement.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoffeeManagement
{
    public partial class fAccountProfile : Form
    {
        public fAccountProfile(Account acc)
        {
            InitializeComponent();

            LoginAccount = acc;
        }

        private EventHandler<AccountEvent> updateAccount;
        public event EventHandler<AccountEvent> UpdateAccount
        {
            add { updateAccount += value; }
            remove { updateAccount -= value; }
        }

        private Account loginAccount;

        public Account LoginAccount
        {
            get { return loginAccount; }
            set
            {
                loginAccount = value;
                ChangeAccount(loginAccount);
            }
        }

        void ChangeAccount(Account acc)
        {
            txbUsername.Text = LoginAccount.UserName;
            txbDisplayName.Text = LoginAccount.DisplayName;
        }

        void UpdateAccountInfo()
        {
            string userName = txbUsername.Text;
            string displayName = txbDisplayName.Text;
            string oldPassword = txbOldPassword.Text;
            string newPassword = Regex.Replace(txbNewPassword.Text, @"\s+", "");
            string retypePassword = Regex.Replace(txbRetypePassword.Text, @"\s+", "");

            if (!newPassword.Equals(retypePassword))
            {
                MessageBox.Show("Please retype the new password correctly");
            }
            else
            {
                if (AccountDAO.Instance.UpdateAccount(userName, displayName, oldPassword, newPassword))
                {
                    MessageBox.Show("Update successfully");
                    txbOldPassword.Text = "";
                    txbNewPassword.Text = "";
                    txbRetypePassword.Text = "";
                    if (updateAccount != null)
                    {
                        updateAccount(this, new AccountEvent(AccountDAO.Instance.getAccountByUserName(userName)));
                    }
                }
                else
                {
                    MessageBox.Show("Wrong password");
                }
            }

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            UpdateAccountInfo();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

public class AccountEvent : EventArgs
{
    private Account acc;

    public Account Acc
    {
        get { return acc; }
        set { acc = value; }
    }

    public AccountEvent(Account acc)
    {
        this.Acc = acc;
    }
}

